using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ScenarioFlow
{
    /// <summary>
    /// Provides functionis to create a new scenario book from a scenario cript.
    /// </summary>
    public class ScenarioBookPublisher : IScenarioBookPublisher
    {
        //CommandMethod name - CommandMethod MethodInfo
        private readonly Dictionary<string, MethodInfo> commandMethodInfoDictionary = new Dictionary<string, MethodInfo>();

        //Type that declares CommandMethod - instance
        private readonly Dictionary<Type, object> instanceDictionary = new Dictionary<Type, object>();

        //Parameter type - DecoderMethod MethodInfo
        private readonly Dictionary<Type, MethodInfo> decoderMethodInfoDictionary = new Dictionary<Type, MethodInfo>();

        public ScenarioBookPublisher(IEnumerable<IReflectable> reflectables)
        {
            foreach (object obj in reflectables)
            {
                //Register instance
                var objType = obj.GetType();
                if (instanceDictionary.ContainsKey(objType))
                {
                    throw new ArgumentException($"Duplicate type: {objType.Name}");
                }
                else
                {
                    instanceDictionary.Add(objType, obj);
                }

                //Extract public and concrete methods
                var filteredMethodInfos = obj.GetType().GetMethods()
                    .Where(m => m.IsPublic)
                    .Where(m => !m.IsAbstract)
                    .ToArray();

                //============ Extract CommandMethods
                var commandMethodInfos = filteredMethodInfos
                    .Where(m => m.GetCustomAttributes<CommandMethodAttribute>().Count() > 0);

                //Register CommandMethods
                foreach (var methodInfo in commandMethodInfos)
                {
                    foreach (var methodAttribute in methodInfo.GetCustomAttributes<CommandMethodAttribute>())
                    {
                        var methodName = methodAttribute.Name;
                        //Check if the same name already exists
                        if (commandMethodInfoDictionary.ContainsKey(methodName))
                        {
                            throw new ArgumentException($"Duplicate command name: '{methodName}' in '{objType.Name}'.");
                        }
                        //Register method info
                        commandMethodInfoDictionary.Add(methodName, methodInfo);
                    }
                }
                //============

                //============ Extract DecoderMethods
                var DecoderMethods = filteredMethodInfos
                    .Where(m => m.GetCustomAttribute<DecoderMethodAttribute>() != null);

                //Register DecoderMethods
                foreach (var decoderMethod in DecoderMethods)
                {
                    var returnType = decoderMethod.ReturnType;
                    //Check method parameter
                    if (decoderMethod.GetParameters().Length != 1)
                    {
                        ThrowDecoderException("Decoder must have only single 'string' parameter.");
                    }
                    var paramType = decoderMethod.GetParameters()[0].ParameterType;
                    if (paramType != typeof(string))
                    {
                        ThrowDecoderException("Decoder must have only single 'string' parameter.");
                    }
                    //Check if DecoderMethod for the same type already exists
                    if (decoderMethodInfoDictionary.ContainsKey(returnType))
                    {
                        ThrowDecoderException($"Duplicate decoder for '{returnType}'.");
                    }
                    //Register
                    decoderMethodInfoDictionary.Add(returnType, decoderMethod);

                    void ThrowDecoderException(string message)
                    {
                        throw new ArgumentException($"Decoder for '{returnType} in '{decoderMethod.DeclaringType.FullName}' is invalid.\n{message}");
                    }
                }
                //============
            }
            //Check if all the parameter types in the CommandMethods have DecoderMethod
            var requiredDecoderMethodType = commandMethodInfoDictionary.Values
                .SelectMany(m => m.GetParameters())
                .Select(p => p.ParameterType)
                .Distinct();
            foreach(var type in requiredDecoderMethodType)
            {
                if (!decoderMethodInfoDictionary.ContainsKey(type))
                {
                    throw new ArgumentException($"Command that has '{type}' parameter exists but any decoders for it don't exist.");
                }
            }
        }

        public ScenarioBook Publish(IScenarioScript scenarioScript)
        {
            //Analyze scenario codes
            var commandMethods = scenarioScript.Lines.Select<IEnumerable<string>, Func<object>>((code, index) =>
            {
                var elements = code.ToArray();
                //Check if CommandMethod is specified
                if (elements.Length == 0 || elements[0] == string.Empty)
                {
                    ThrowError("Command name is not specified.");
                }
                //Check if the specified command exists
                var commandMethodName = elements[0];
                if (!commandMethodInfoDictionary.ContainsKey(commandMethodName))
                {
                    ThrowError($"The command '{commandMethodName}' does not exist.");
                }
                //Get the method info
                var CommandMethodInfo = commandMethodInfoDictionary[commandMethodName];
                //Check if it is appropriate number of elements to call command
                var paramElements = elements.Skip(1).ToArray();
                var paramInfos = CommandMethodInfo.GetParameters().ToArray();
                if (paramElements.Length < paramInfos.Length)
                {
                    ThrowError($"Deficient arguments to call '{commandMethodName}'.");
                }
                else if (paramElements.Length > paramInfos.Length)
                {
                    ThrowError($"Excessive arguments to call '{commandMethodName}'.");
                }


                return () =>
                {
                    //Prepare parameters
                    var parameters = paramInfos
                        .Select(p => p.ParameterType)
                        .Zip(paramElements, (type, elem) => new { ParamType = type, Element = elem })
                        .Select(pair =>
                        {
                            var DecoderMethodInfo = decoderMethodInfoDictionary[pair.ParamType];
                            var instance = instanceDictionary[DecoderMethodInfo.DeclaringType];
                            var parameters = new object[] { pair.Element };
                            return DecoderMethodInfo.Invoke(instance, parameters);
                        }).ToArray();
                    //Call CommandMethod
                    var instance = instanceDictionary[CommandMethodInfo.DeclaringType];
                    return CommandMethodInfo.Invoke(instance, parameters);
                };

                //Common Error message
                void ThrowError(string message)
                {
                    throw new ArgumentException($"Error at line {index + 1} '{string.Join(',', elements.Select(e => $"{{ {e} }}"))}'.\n{message}\n{scenarioScript}");
                }
            });

            //Create and return ScenarioBook
            return new ScenarioBook(commandMethods, scenarioScript.LabelDictionary);
        }
    }
}