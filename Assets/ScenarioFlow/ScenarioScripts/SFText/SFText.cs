using ScenarioFlow.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ScenarioFlow.Scripts.SFText
{
    public class SFText : ScenarioScript
    {
        [SerializeField]
        private Line[] lines;
        [SerializeField]
        private Label[] labels;
        [SerializeField]
        private ErrorMessage[] errorMessages;
        [SerializeField]
        private string text;


        public override IEnumerable<IEnumerable<string>> Lines => lines?.Select(line => line.Elements);
        public override IReadOnlyDictionary<string, int> LabelDictionary => labels?.ToDictionary(label => label.Name, label => label.Index);
        public string Text => text;
        public IEnumerable<ErrorMessage> ErrorMessages => errorMessages;

        private static readonly Regex ValidLineRegex = new Regex(@"^(.*?)\|(.*?)\|(.*)$");
        private static readonly Regex DialogueArgumentRegex = new Regex(@"^-->(.*)$");
        private static readonly Regex MacroCodeRegex = new Regex(@"^#(.*)$");
        private static readonly Regex TokenCodeRegex = new Regex(@"^\$(.*)$");
        private static readonly Regex ArgumentRegex = new Regex(@"{(.*?)}");

        public static readonly string LineBreakSymbol = "<bk>";
        public static readonly string SyncTokenCode = "sync";
        public static readonly string SerialTokenCode = "serial";
        public static readonly string ParallelTokenCode = "parallel";

        private enum Scope
        {
            Comment,
            Macro,
            Command,
            Dialogue,
        }
        
        public void BuildScript(string text)
        {
            //Register
            this.text = text;

            //Result
            var lineList = new List<string[]>();
            var labelScopeIndexDictionary = new Dictionary<string, int>();
            var labelDictionary = new Dictionary<string, int>();
            var errorMessageList = new List<ErrorMessage>();

            //Memorized data
            var dialogueCommand = string.Empty;
            var exDialogueCommand = string.Empty;
            var dialogueToken = string.Empty;
            var tokenCode = string.Empty;
            var macroCode = string.Empty;
            var macroArgumentList = new List<string>();
            var defineDictionary = new Dictionary<string, string>();
            var lineScope = Scope.Comment;
            var scopeIndex = -1;
            var lineElementList = new List<string>();
            //Key: scenario script index, Value: scope index
            var specialTokenCodeScopeDictionary = new Dictionary<int, int>();

            var textLines = GetLines(text).Select((t, i) => (t, i)).ToArray();

            foreach ((string line, int index) in textLines)
            {
                var matches = ValidLineRegex.Match(line);
                var doesResetScope = false;
                //Ignore invalid lines
                if (!matches.Success)
                {
                    continue;
                }
                //Split
                var leftPart = ReplaceVBarEscapes(matches.Groups[1].Value.Trim());
                var centerPart = ReplaceVBarEscapes(matches.Groups[2].Value.Trim());
                var rightPart = ReplaceVBarEscapes(matches.Groups[3].Value.Trim());
                //Left part is empty
                if (string.IsNullOrWhiteSpace(leftPart))
                {
                    if (lineScope == Scope.Comment)
                    {
                        continue;
                    }
                    else if (string.IsNullOrWhiteSpace(centerPart))
                    {
                        ChangeScope(Scope.Comment);
                    }
                    else
                    {
                        //Push macro arguments
                        if (lineScope == Scope.Macro)
                        {
                            foreach (var argument in ExtractArguments(centerPart))
                            {
                                macroArgumentList.Add(argument);
                            }
                        }
                        //Push command arguments
                        else if (lineScope == Scope.Command)
                        {
                            foreach (var argument in ExtractArguments(centerPart).Select(s => TryReplaceByDefine(s)))
                            {
                                lineElementList.Add(argument);
                            }
                        }

                        //Dialogue scope
                        else
                        {
                            var dialogueArgumentsMatch = DialogueArgumentRegex.Match(centerPart);
                            //Push dialogue arguments
                            if (dialogueArgumentsMatch.Success)
                            {
                                foreach (var argument in ExtractArguments(dialogueArgumentsMatch.Groups[1].Value).Select(s => TryReplaceByDefine(s)))
                                {
                                    lineElementList.Add(argument);
                                }
                            }
                            //Combine dialogue text
                            else
                            {
                                lineElementList[2] = $"{lineElementList[2]}{LineBreakSymbol}{centerPart}";
                            }
                        }
                    }
                }
                //Start Comment scope
                else if (leftPart.StartsWith("//"))
                {
                    ChangeScope(Scope.Comment);
                }
                //Start Macro scope
                else if (leftPart.StartsWith("#"))
                {
                    ChangeScope(Scope.Macro);
                    macroCode = MacroCodeRegex.Match(leftPart).Groups[1].Value;
                    //Push arguments
                    foreach (var argument in ExtractArguments(centerPart))
                    {
                        macroArgumentList.Add(argument);
                    }
                }
                //Start command scope ------------
                else if (leftPart.StartsWith("$"))
                {
                    ChangeScope(Scope.Command);
                    //Memorize token code
                    tokenCode = TokenCodeRegex.Match(leftPart).Groups[1].Value;
                    if (string.IsNullOrWhiteSpace(tokenCode))
                    {
                        AddErrorMessage("The command token code is empty.");
                        doesResetScope = true;
                    }
                    //Push command name
                    lineElementList.Add(centerPart);
                    if (string.IsNullOrWhiteSpace(centerPart))
                    {
                        AddErrorMessage("The command name is empty.");
                        doesResetScope = true;
                    }
                }
                //Start Dialogue scope ------------
                else
                {
                    ChangeScope(Scope.Dialogue);
                    //Memorize token code
                    if (string.IsNullOrWhiteSpace(dialogueToken))
                    {
                        AddErrorMessage("Dialogue token is not specified.");
                        doesResetScope= true;
                    }
                    else
                    {
                        tokenCode = dialogueToken;
                    }
                    //Push temporary dialogue command name
                    lineElementList.Add(string.Empty);
                    //Push character's name
                    //It can be replaced with defined value
                    lineElementList.Add(TryReplaceByDefine(leftPart));
                    //Push dialogue text
                    lineElementList.Add(centerPart);
                }

                //Reset scope
                if (doesResetScope)
                {
                    ResetScope();
                    continue;
                }


                //The end of file
                if (index == textLines.Length - 1 && lineScope != Scope.Comment)
                {
                    ChangeScope(Scope.Comment);
                }

                void ChangeScope(Scope scope)
                {
                    //Push command line
                    if (lineScope == Scope.Command || lineScope == Scope.Dialogue)
                    {
                        if (lineElementList.Count > 0)
                        {
                            if (lineScope == Scope.Dialogue)
                            {
                                //Trim empty lines
                                lineElementList[2] = lineElementList[2].Trim();
                                //Parameterless dialogue command
                                if (lineElementList.Count <= 3)
                                {
                                    if (dialogueCommand == string.Empty)
                                    {
                                        AddErrorMessage("No dialogue command name is specified.");
                                    }
                                    lineElementList[0] = dialogueCommand;
                                }
                                //Dialogue command with extra arguments
                                else
                                {
                                    if (exDialogueCommand == string.Empty)
                                    {
                                        AddErrorMessage("No ex-dialouge command name is specified.");
                                    }
                                    lineElementList[0] = exDialogueCommand;
                                }
                            }
                            //Add token code
                            if (tokenCode != SyncTokenCode)
                            {
                                lineElementList.Add(tokenCode);
                            }
                            //Record the async command scope index
                            if (SpecialTokenCodes.IsSpecial(tokenCode) || tokenCode == SerialTokenCode || tokenCode == ParallelTokenCode)
                            {
                                specialTokenCodeScopeDictionary.Add(lineList.Count, scopeIndex);
                            }
                            //Push scope
                            lineList.Add(lineElementList.ToArray());
                            lineElementList.Clear();
                            tokenCode = string.Empty;
                        }
                    }
                    //Reflect macro
                    else if (lineScope == Scope.Macro)
                    {
                        //Dialogue command
                        if (macroCode == "command")
                        {
                            if (macroArgumentList.Count == 1)
                            {
                                var commandName = macroArgumentList[0];
                                if (string.IsNullOrWhiteSpace(commandName))
                                {
                                    AddErrorMessage("Command name must not be empty.");
                                }
                                else
                                {
                                    dialogueCommand = macroArgumentList[0];
                                }
                            }
                            else
                            {
                                if (macroArgumentList.Count > 1)
                                {
                                    AddErrorMessage($"Excessive arguments for 'command' macro scope. It requires a command name to be called as a dialogue command.");
                                }
                                else
                                {
                                    AddErrorMessage($"Deficient arguments for 'command' macro scope. It requires a command name to be called as a dialogue command.");
                                }
                            }
                        }
                        else if (macroCode == "xcommand")
                        {
							if (macroArgumentList.Count == 1)
							{
								var commandName = macroArgumentList[0];
								if (string.IsNullOrWhiteSpace(commandName))
								{
									AddErrorMessage("Command name must not be empty.");
								}
								else
								{
									exDialogueCommand = macroArgumentList[0];
								}
							}
							else
							{
								if (macroArgumentList.Count > 1)
								{
									AddErrorMessage($"Excessive arguments for 'xcommand' macro scope. It requires a command name to be called as a ex-dialogue command.");
								}
								else
								{
									AddErrorMessage($"Deficient arguments for 'xcommand' macro scope. It requires a command name to be called as a ex-dialogue command.");
								}
							}

						}
						else if (macroCode == "token")
                        {
                            if (macroArgumentList.Count == 1)
                            {
                                var tokenMatch = TokenCodeRegex.Match(macroArgumentList[0]);
                                if (tokenMatch.Success)
                                {
                                    var tokenCode = tokenMatch.Groups[1].Value;
                                    if (string.IsNullOrWhiteSpace(tokenCode))
                                    {
                                        AddErrorMessage("Token code must not be empty.");
                                    }
                                    else
                                    {
                                        dialogueToken = tokenMatch.Groups[1].Value;
                                    }
                                }
                                else
                                {
                                    AddErrorMessage("Pass token code with the following format: '$TOKEN_CODE'.");
                                }
                            }
                            else
                            {
                                if (macroArgumentList.Count > 1)
                                {
                                    AddErrorMessage($"Excessive arguments for 'token' macro scope. It requires a token code to be given to a dialogue command.");
                                }
                                else
                                {
                                    AddErrorMessage($"Deficient arguments for 'token' macro scope. It requires a token code to be given to a dialogue command.");
                                }
                            }
                        }
                        else if (macroCode == "define")
                        {
                            if (macroArgumentList.Count == 2)
                            {
                                defineDictionary[macroArgumentList[0]] = macroArgumentList[1];
                            }
                            else
                            {
                                if (macroArgumentList.Count > 2)
                                {
                                    AddErrorMessage($"Excessive arguments for 'define' macro scope. It requires a symbol to be replaced and a value to replace it with.");
                                }
                                else
                                {
                                    AddErrorMessage($"Deficient arguments for 'define' macro scope. It requires a symbol to be replaced and a value to replace it with.");
                                }
                            }
                        }
                        else if (macroCode == "label")
                        {
                            if (macroArgumentList.Count == 1)
                            {
                                var name = macroArgumentList[0];
                                //Empty check
                                if (string.IsNullOrWhiteSpace(name))
                                {
                                    AddErrorMessage("Label must not be empty.");
                                }
                                //Add label
                                else if (labelDictionary.ContainsKey(name))
                                {
                                    AddErrorMessage($"Label '{name}' exists already.");
                                }
                                else
                                {
                                    labelDictionary.Add(name, lineList.Count);
                                    labelScopeIndexDictionary.Add(name, scopeIndex);
                                }
                            }
                            else
                            {
                                if (macroArgumentList.Count > 1)
                                {
                                    AddErrorMessage($"Excessive arguments for 'label' macro scope. It requires a label name.");
                                }
                                else
                                {
                                    AddErrorMessage($"Deficient arguments for 'label' macro scope. It requires a label name.");
                                }
                            }
                        }
                        else
                        {
                            AddErrorMessage($"Unknown macro scope '{macroCode}'.");
                        }
                        //Clear
                        macroCode = string.Empty;
                        macroArgumentList.Clear();
                    }

                    //Change scope
                    lineScope = scope;
                    if (lineScope != Scope.Comment)
                    {
                        scopeIndex++;
                    }
                }

                void AddErrorMessage(string message)
                {
                    AddScopeErrorMessage(scopeIndex, message);
                }

                void ResetScope()
                {
                    macroArgumentList.Clear();
                    lineElementList.Clear();
                    lineScope = Scope.Comment;
                    macroCode = string.Empty;
                    tokenCode = string.Empty;
                }
            }
            //------------ foreach

            //Replace serial and parallel token codes
            var serialTokenCode = string.Empty;
            var parallelTokenCode = string.Empty;
            foreach (var index in specialTokenCodeScopeDictionary.Keys.Reverse())
            {
                var code = lineList[index].Last();
                if (code != SerialTokenCode && code != ParallelTokenCode)
                {
                    if (SpecialTokenCodes.IsStandard(code))
                    {
                        serialTokenCode = SpecialTokenCodes.SerialStandardTokenCode;
                        parallelTokenCode = SpecialTokenCodes.ParallelStandardTokenCode;
                    }
                    else if (SpecialTokenCodes.IsForced(code))
                    {
                        serialTokenCode = SpecialTokenCodes.SerialForcedTokenCode;
                        parallelTokenCode = SpecialTokenCodes.ParallelForcedTokenCode;
                    }
                    else
                    {
                        serialTokenCode = SpecialTokenCodes.SerialPromisedTokenCode;
                        parallelTokenCode = SpecialTokenCodes.ParallelPromisedTokenCode;
                    }
                }
                else
                {
                    if (serialTokenCode == string.Empty)
                    {
                        AddScopeErrorMessage(specialTokenCodeScopeDictionary[index], "The passed token code is 'serial' or 'paral' but there is not the following scope that has plain or fluent token code.");
                    }
                    else if (code == SerialTokenCode)
                    {
                        lineList[index][lineList[index].Length - 1] = serialTokenCode;
                    }
                    else
                    {
                        lineList[index][lineList[index].Length - 1] = parallelTokenCode;
                    }
                }
            }

            //Set results
            lines = lineList.Select(elementList => new Line(elementList)).ToArray();
            labels = labelDictionary.Select(p => new Label(p.Key, p.Value)).ToArray();
            errorMessages = errorMessageList.ToArray();

            IEnumerable<string> ExtractArguments(string text)
            {
                return ArgumentRegex.Matches(text)
                    .Select(match => match.Groups[1].Value)
                    .Select(text => ReplaceCurlyBracketEscapes(text));
            }

            string TryReplaceByDefine(string value)
            {
                return defineDictionary.TryGetValue(value, out var result) ? result : value;
            }

            void AddScopeErrorMessage(int scopeIndex, string message)
            {
                errorMessageList.Add(new ErrorMessage(scopeIndex, $"Error at scope {scopeIndex + 1}.\n{message}"));
            }
        }

        private IEnumerable<string> GetLines(string text)
        {
            using (var reader = new StringReader(text))
            {
                while (reader.Peek() != -1)
                {
                    var line = reader.ReadLine();
                    yield return ValidLineRegex.IsMatch(line) ? line : "||";
                }
            }
        }

        private string ReplaceVBarEscapes(string text)
        {
            return text.Replace("\\|", "|");
        }

        private string ReplaceCurlyBracketEscapes(string text)
        {
            return text.Replace("\\{", "{").Replace("\\}", "}");
        }

        [Serializable]
        public class ErrorMessage
        {
            [SerializeField]
            private int scopeIndex;
            [SerializeField]
            private string message;

            public int ScopeIndex => scopeIndex;
            public string Message => message;

            public ErrorMessage(int scopeIndex, string message)
            {
                this.scopeIndex = scopeIndex;
                this.message = message;
            }
        }
    }
}