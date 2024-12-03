#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using ScenarioFlow.Tasks;
using ScenarioFlow.Scripts.Editor;

namespace ScenarioFlow.Scripts.SFText.Editor
{
    [CustomEditor(typeof(SFText))]
    public class SFTextAssetEditor : UnityEditor.Editor
    {
        private static readonly Regex ValidLineRegex = new Regex(@"^(.*?)\|(.*?)\|(.*)$");
        private static readonly Regex DialogueArgumentRegex = new Regex(@"^(-->)(.*)$");
        private static readonly Regex TokenCodeRegex = new Regex(@"^\$(.*)$");
        private static readonly Regex ArgumentRegex = new Regex(@"({)(.*?)(})");

        private static readonly string MacroColor = "D2A6FF";
        private static readonly string CommentColor = "888888";
        private static readonly string SerialColor = "95E6CB";
        private static readonly string ParallelColor = "E6B673";
        private static readonly string StandardColor = "39BAE6";
        private static readonly string ForcedColor = "FFB454";
        private static readonly string PromisedColor = "F07178";
        private static readonly string SyncColor = "CCCCCC";
        private static readonly string ArgumentColor = "D2A6FF";
        private static readonly string DialogueColor = "AAD94C";
        private static readonly string BracketsColor = "FFD800";
        private static readonly string ErrorColor = "FF5555";


        private float editorWidthCache;
        private int toolBarIndex = 0;
        private string[] toolBarContents = new string[] { "SFText", "Scenario Script" };

        private enum Scope
        {
            Comment,
            Empty,
            Macro,
            Command,
            Dialogue,
        }

        public override void OnInspectorGUI()
        {
            var sftext = (SFText)target;

            GUI.enabled = true;

            toolBarIndex = GUILayout.Toolbar(toolBarIndex, toolBarContents);

            if (toolBarIndex == 0)
            {
                ShowSFText();
            }
            else
            {
                ShowScenarioScript();
            }

            GUI.enabled = false;

            void ShowScenarioScript()
            {
                ScenarioScriptViewer.ShowScenarioScript(sftext);
            }

            void ShowSFText()
            {
                //Error messages ------------
                foreach (var message in sftext.ErrorMessages)
                {
                    EditorGUILayout.HelpBox(message.Message, MessageType.Error);
                }

                //Text ------------
                var richLabelStyle = new GUIStyle(EditorStyles.label);
                richLabelStyle.richText = true;
                richLabelStyle.wordWrap = true;

                var inspectorRect = EditorGUILayout.GetControlRect(false, 0);
                var editorWidth = inspectorRect.width;
                if (Mathf.Approximately(editorWidth, 1.0f))
                {
                    editorWidth = editorWidthCache;
                }
                else
                {
                    editorWidthCache = editorWidth;
                }


                var lineIndexWidth = editorWidth * 1.0f / 10.0f;
                var leftWidth = editorWidth * 1.5f / 10.0f;
                var centerwidth = editorWidth * 4.0f / 10.0f;
                var rightWidth = editorWidth * 2.5f / 10.0f;
                var scopeIndexWidth = editorWidth * 1.0f / 10.0f;

                var lineScope = Scope.Empty;
                var scopeIndex = -1;
                var errorScopes = sftext.ErrorMessages.Select(message => message.ScopeIndex).ToArray();

                foreach (var (line, index) in GetAllLines(sftext.Text)
                    //Escape rich text
                    .Select(line => line.Replace("<", "<\u200B"))
                    .Select((line, index) => (line, index)))
                {
                    var lineMatch = ValidLineRegex.Match(line);
                    if (lineMatch.Success)
                    {
                        var lineIndexPart = (index + 1).ToString();
                        var leftPart = lineMatch.Groups[1].Value.Trim();
                        var centerPart = lineMatch.Groups[2].Value.Trim();
                        var rightPart = lineMatch.Groups[3].Value.Trim();
                        var scopeIndexPart = string.Empty;
                        var doesIncScope = false;

                        //Comment
                        if (leftPart.StartsWith("//"))
                        {
                            if (lineScope != Scope.Comment)
                            {
                                ShowHorizontalLine();
                            }
                            lineScope = Scope.Comment;
                            leftPart = AddColor(leftPart, CommentColor);
                            centerPart = AddColor(centerPart, CommentColor);
                            rightPart = AddColor(rightPart, CommentColor);
                            scopeIndexPart = string.Empty;
                        }
                        //Start macro scope
                        else if (leftPart.StartsWith("#"))
                        {
                            ShowHorizontalLine();
                            lineScope = Scope.Macro;
                            leftPart = AddColor(leftPart, MacroColor);
                            centerPart = AddArgumentColor(centerPart);
                            doesIncScope = true;
                        }
                        //Start Command scope
                        else if (leftPart.StartsWith("$"))
                        {
                            ShowHorizontalLine();
                            lineScope = Scope.Command;
                            var lineColor = string.Empty;
                            var tokenCodeMatch = TokenCodeRegex.Match(leftPart);
                            var tokenCode = tokenCodeMatch.Groups[1].Value.Trim();
                            if (SpecialTokenCodes.IsStandard(tokenCode))
                            {
                                lineColor = StandardColor;
                            }
                            else if (SpecialTokenCodes.IsForced(tokenCode))
                            {
                                lineColor = ForcedColor;
                            }
                            else if (SpecialTokenCodes.IsPromised(tokenCode))
                            {
                                lineColor = PromisedColor;
                            }
                            else if (tokenCode == SFText.SerialTokenCode)
                            {
                                lineColor = SerialColor;
                            }
                            else if (tokenCode == SFText.ParallelTokenCode)
                            {
                                lineColor = ParallelColor;
                            }
                            else if (tokenCode == SFText.SyncTokenCode)
                            {
                                lineColor = SyncColor;
                            }
                            else
                            {
                                lineColor = SyncColor;
                            }
                            leftPart = AddColor(leftPart, lineColor);
                            centerPart = AddColor(centerPart, lineColor);
                            doesIncScope = true;
                        }
                        //Start Dialogue scope
                        else if (!string.IsNullOrWhiteSpace(leftPart))
                        {
                            ShowHorizontalLine();
                            lineScope = Scope.Dialogue;
                            leftPart = AddColor(leftPart, DialogueColor);
                            centerPart = AddColor(centerPart, DialogueColor);
                            doesIncScope = true;
                        }
                        //Empty line
                        else if (string.IsNullOrWhiteSpace(centerPart) && lineScope != Scope.Comment)
                        {
                            lineScope = Scope.Empty;
                            scopeIndexPart = string.Empty;
                        }
                        //Arguments
                        else
                        {
                            if (lineScope == Scope.Macro)
                            {
                                centerPart = AddArgumentColor(centerPart);
                            }
                            else if (lineScope == Scope.Command)
                            {
                                centerPart = AddArgumentColor(centerPart);
                            }
                            else if (lineScope == Scope.Dialogue)
                            {
                                var dialogueArgumentsMatch = DialogueArgumentRegex.Match(centerPart);
                                if (dialogueArgumentsMatch.Success)
                                {
                                    centerPart = $"{AddColor(dialogueArgumentsMatch.Groups[1].Value, ArgumentColor)}{AddArgumentColor(dialogueArgumentsMatch.Groups[2].Value)}";
                                }
                                else
                                {
                                    centerPart = AddColor(centerPart, DialogueColor);
                                }
                            }
                            else
                            {
                                if (lineScope == Scope.Empty)
                                {
                                    ShowHorizontalLine();
                                }
                                centerPart = AddColor(centerPart, CommentColor);
                                lineScope = Scope.Comment;
                            }
                            scopeIndexPart = string.Empty;
                        }

                        //Comment line
                        rightPart = AddColor(rightPart, CommentColor);

                        //New scope
                        if (doesIncScope)
                        {
                            scopeIndex++;
                            scopeIndexPart = ((scopeIndex) + 1).ToString();
                        }

                        //Error line
                        if (errorScopes.Contains(scopeIndex) && lineScope != Scope.Comment)
                        {
                            lineIndexPart = AddColor(lineIndexPart, ErrorColor);
                            scopeIndexPart = AddColor(scopeIndexPart, ErrorColor);
                        }

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.Label(lineIndexPart, richLabelStyle, GUILayout.Width(lineIndexWidth));
                            GUILayout.Label(leftPart, richLabelStyle, GUILayout.Width(leftWidth));
                            GUILayout.Label(centerPart, richLabelStyle, GUILayout.Width(centerwidth));
                            GUILayout.Label(rightPart, richLabelStyle, GUILayout.Width(rightWidth));
                            GUILayout.Label(scopeIndexPart, richLabelStyle, GUILayout.Width(scopeIndexWidth));
                        }
                    }
                    else
                    {
                        GUILayout.Label((index + 1).ToString(), richLabelStyle, GUILayout.Width(lineIndexWidth));
                        GUILayout.Label(AddColor(line, CommentColor), richLabelStyle, GUILayout.Width(leftWidth + centerwidth + rightWidth));
                    }
                }
            }

            void ShowHorizontalLine()
            {
                GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));
            }
        }

        private IEnumerable<string> GetAllLines(string text)
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

        private string AddArgumentColor(string text)
        {
            var result =  ArgumentRegex.Replace(text, $"{AddColor("$1", BracketsColor)}{AddColor("$2", ArgumentColor)}{AddColor("$3", BracketsColor)}");
            return AddColor(result, CommentColor);
        }

        private string AddColor(string text, string color)
        {
            return $"<color=#{color}>{text}</color>";
        }
    }
}
#endif