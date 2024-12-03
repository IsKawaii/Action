#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace ScenarioFlow.Scripts.SFText.Editor
{
    public class SFTextCommandList : EditorWindow
    {
        public static IEnumerable<MethodInfo> CommandMethodInfos => commandMethodInfos;

        private static MethodInfo[] commandMethodInfos;
        private static MethodInfo[] decoderMethodInfos;

        //0: Commands, 1: Decoders
        private int methodBarIndex = 0;
        private static readonly string[] MethodBarSelections = new string[] { "Commnads", "Decoders" };

        //0: Category, 1: Class
        private int groupBarIndex = 0;
        private static readonly string[] GroupBarSelections = new string[] { "Category", "Class" };
        private Vector2 commandScrollPosition;
        private MethodInfo selectedMethodInfo;

        private static readonly Regex ParameterRegex = new Regex(@"{\${\d+:(.*?)}}|{\${(.*?)}}");

        static SFTextCommandList()
        {
            //Extract all classes that implement the 'IReflectable' interface
            var reflectableTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.FullName.StartsWith("Unity"))
                .Where(assembly => !assembly.FullName.StartsWith("System"))
                .Where(assembly => !assembly.FullName.StartsWith("Mono"))
                .Where(assembly => !assembly.FullName.StartsWith("UniTask"))
                .Where(assembly => !assembly.FullName.StartsWith("Bee"))
                .Where(assembly => !assembly.FullName.StartsWith("netstandard"))
                .Where(assembly => !assembly.FullName.StartsWith("mscorlib"))
                .Where(assembly => !assembly.FullName.StartsWith("log4net"))
                .Where(assembly => !assembly.FullName.StartsWith("unityplastic"))
                .Where(assembly => !assembly.FullName.StartsWith("nunit.framework"))
                .Where(assembly => !assembly.FullName.StartsWith("ExcelDataReader"))
                .Where(assembly => !assembly.FullName.StartsWith("PsdPlugin"))
                .Where(assembly => !assembly.FullName.StartsWith("ScenarioFlow"))
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IReflectable).IsAssignableFrom(type))
                .ToArray();
            //Extract command methods
            commandMethodInfos = reflectableTypes
                .SelectMany(type => type.GetMethods())
                .Where(method => method.GetCustomAttribute<CommandMethodAttribute>() != null)
                .ToArray();
            //Extract decoder methods
            decoderMethodInfos = reflectableTypes
                .SelectMany(type => type.GetMethods())
                .Where(method => method.GetCustomAttribute<DecoderMethodAttribute>() != null)
                .ToArray();
        }

        [MenuItem("Window/ScenarioFlow/SFText Command List")]
        private static void CreateWindow()
        {
            GetWindow<SFTextCommandList>("SFText Command List");
        }

        private void OnGUI()
        {
            methodBarIndex = GUILayout.Toolbar(methodBarIndex, MethodBarSelections);

            ShowEmptyLine();

            using (new EditorGUILayout.VerticalScope("Box"))
            {

                if (selectedMethodInfo == null)
                {
                    GUILayout.Label("Command/Decoder Information");
                    ShowHorizontalLine();

                    EditorGUILayout.HelpBox("Click a label to see the detail.", MessageType.Info);
                }
                else
                {
                    ShowMethodDetail(selectedMethodInfo);
                }
            }

            //Command Methods

            using (new EditorGUILayout.VerticalScope("Box"))
            {
                GUILayout.Label(methodBarIndex == 0 ? "Commands" : "Decoders");
                ShowHorizontalLine();
                ShowHorizontalLine();
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("Group by");
                    groupBarIndex = GUILayout.Toolbar(groupBarIndex, GroupBarSelections);
                }
                ShowHorizontalLine();
                using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(commandScrollPosition))
                {
                    commandScrollPosition = scrollViewScope.scrollPosition;

                    var titleStyle = new GUIStyle(EditorStyles.label);
                    titleStyle.richText = true;
                    titleStyle.fontStyle = FontStyle.BoldAndItalic;

                    var methodInfos = methodBarIndex == 0 ? commandMethodInfos : decoderMethodInfos;

                    //Group by category
                    if (groupBarIndex == 0)
                    {
                        foreach (var group in methodInfos
                            .GroupBy(info => info.GetCustomAttribute<CategoryAttribute>()?.Name)
                            .OrderBy(group => group.Key))
                        {
                            var categoryText = group.Key ?? "No Category";
                            categoryText = $"<color=#eeeeee>{categoryText} (Category)</color>";
                            using (new EditorGUILayout.VerticalScope("Box"))
                            {
                                GUILayout.Label(categoryText, titleStyle);
                                ShowHorizontalLine();

                                var sortedGroup = methodBarIndex == 0 ? group.OrderBy(info => info.GetCustomAttribute<CommandMethodAttribute>().Name) :
                                    group.OrderBy(info => info.ReturnType.Name);
                                foreach (var methodInfo in sortedGroup)
                                {
                                    using (new GUILayout.VerticalScope("Box"))
                                    {
                                        ShowMethod(methodInfo);
                                    }
                                }
                            }
                        }
                    }
                    //Group by class
                    else
                    {
                        foreach (var groupByNamespace in methodInfos
                            .GroupBy(info => info.DeclaringType.Namespace)
                            .OrderBy(group => group.Key))
                        {
                            var namespaceText = groupByNamespace.Key ?? "No Namespace";
                            namespaceText = $"<color=#eeeeee>{groupByNamespace.Key ?? "No Namespace"} (Namespace)</color>";
                            GUILayout.Label(namespaceText, titleStyle);
                            ShowHorizontalLine();
                            ShowHorizontalLine();

                            foreach (var groupByClass in groupByNamespace
                                .GroupBy(info => info.DeclaringType.Name)
                                .OrderBy(group => group.Key))
                            {
                                using (new GUILayout.VerticalScope("Box"))
                                {
                                    GUILayout.Label($"<color=#eeeeee>{groupByClass.Key} (Class)</color>", titleStyle);
                                    ShowHorizontalLine();
                                    foreach (var methodInfo in groupByClass)
                                    {
                                        using (new EditorGUILayout.VerticalScope("Box"))
                                        {
                                            ShowMethod(methodInfo);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            void ShowMethod(MethodInfo methodInfo)
            {
                var richLabelStyle = new GUIStyle(EditorStyles.label);
                richLabelStyle.richText = true;
                var colorCode = selectedMethodInfo == methodInfo ? "ffff80" : "cccccc";
                var text = methodBarIndex == 0 ? $"<color=#{colorCode}>{methodInfo.GetCustomAttribute<CommandMethodAttribute>().Name}</color>" :
                    $"<color=#{colorCode}>{methodInfo.ReturnType.Name}</color>";
                if (GUILayout.Button(text, richLabelStyle))
                {
                    selectedMethodInfo = methodInfo;
                }
            }

            void ShowMethodDetail(MethodInfo methodInfo)
            {
                var richLabelStyle = new GUIStyle(EditorStyles.label);
                richLabelStyle.richText = true;
                richLabelStyle.wordWrap = true;
                //Command detail
                if (methodInfo.GetCustomAttribute<DecoderMethodAttribute>() == null)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        var isAsyncCommand = methodInfo.ReturnType == typeof(UniTask);
                        GUILayout.Label($"<b><i><color=#{(isAsyncCommand ? "faa755" : "fffa99")}>{methodInfo.GetCustomAttribute<CommandMethodAttribute>().Name} ({(isAsyncCommand ? "Async" : "Sync")} Command)</color></i></b>", richLabelStyle);
                        if (GUILayout.Button("Close"))
                        {
                            selectedMethodInfo = null;
                        }
                    }
                    ShowHorizontalLine();
                    using (new EditorGUILayout.VerticalScope("Box"))
                    {
                        GUILayout.Label("<b><color=#eeeeee>Parameters</color></b>", richLabelStyle);
                        ShowHorizontalLine();
                        var parameterInfos = methodInfo.GetParameters().Where(info => info.ParameterType != typeof(CancellationToken));
                        if (parameterInfos.Count() > 0)
                        {
                            foreach (var info in parameterInfos)
                            {
                                GUILayout.Label($"<color=#6e6eff>{info.ParameterType.Name}</color> <color=#a0d8ef>{info.Name}</color>", richLabelStyle);
                            }
                        }
                        else
                        {
                            GUILayout.Label("Parameter-less");
                        }
                    }
                    using (new EditorGUILayout.VerticalScope("Box"))
                    {
                        GUILayout.Label("<b><color=#eeeeee>Description</color></b>", richLabelStyle);
                        ShowHorizontalLine();
                        var descriptionAttributes = methodInfo.GetCustomAttributes<DescriptionAttribute>();
                        var descriptionInfo = descriptionAttributes.Count() > 0 ? string.Join("\n", descriptionAttributes.Select(a => a.Text)) : "No description.";
                        GUILayout.Label(descriptionInfo, richLabelStyle);
                    }
                    using (new EditorGUILayout.VerticalScope("Box"))
                    {
                        GUILayout.Label("<b><color=#eeeeee>Snippet</color></b>", richLabelStyle);
                        ShowHorizontalLine();
                        var snippetAttributes = methodInfo.GetCustomAttributes<SnippetAttribute>();
                        var snippetInfo = snippetAttributes.Count() > 0 ? string.Join("\n", snippetAttributes.Select(a => a.Text)) : "No snippet.";
                        snippetInfo = ParameterRegex.Replace(snippetInfo, "<color=#c77eb5>$1$2</color>");
                        GUILayout.Label(snippetInfo, richLabelStyle);
                    }
                    if (methodInfo.GetCustomAttributes<DialogueSnippetAttribute>().Count() > 0)
                    {
                        using (new EditorGUILayout.VerticalScope("Box"))
                        {
                            GUILayout.Label("<b><color=#eeeeee>Dialogue Snippet</color></b>", richLabelStyle);
                            ShowHorizontalLine();
                            var snippetAttributes = methodInfo.GetCustomAttributes<DialogueSnippetAttribute>();
                            var snippetInfo = snippetAttributes.Count() > 0 ? string.Join("\n", snippetAttributes.Select(a => a.Text)) : "No snippet.";
                            snippetInfo = ParameterRegex.Replace(snippetInfo, "<color=#c77eb5>$1$2</color>");
                            GUILayout.Label(snippetInfo, richLabelStyle);
                        }
                    }
                }
                //Decoder detail
                else
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Label($"<b><color=#c77eb5>{methodInfo.ReturnType.Name} Decoder</color></b>", richLabelStyle);
                        if (GUILayout.Button("Close"))
                        {
                            selectedMethodInfo = null;
                        }
                    }
                    using (new EditorGUILayout.VerticalScope("Box"))
                    {
                        GUILayout.Label("<b><color=#eeeeee>Description</color></b>", richLabelStyle);
                        ShowHorizontalLine();
                        var descriptionAttributes = methodInfo.GetCustomAttributes<DescriptionAttribute>();
                        var descriptionInfo = descriptionAttributes.Count() > 0 ? string.Join("\n", descriptionAttributes.Select(a => a.Text)) : "No description.";
                        GUILayout.Label(descriptionInfo, richLabelStyle);
                    }
                }
            }

            void ShowHorizontalLine()
            {
                GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));
            }

            void ShowEmptyLine()
            {
                GUILayout.Label("");
            }
        }

        private string GetMethodInfoId(MethodInfo methodInfo)
        {
            return $"{methodInfo.DeclaringType.FullName} {methodInfo.ReturnType.FullName} {methodInfo.Name} {string.Join(' ', methodInfo.GetParameters().Select(p => p.ParameterType.FullName))}";
        }
    }
}
#endif