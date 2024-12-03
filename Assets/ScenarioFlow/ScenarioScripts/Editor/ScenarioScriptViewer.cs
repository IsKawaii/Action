#if UNITY_EDITOR

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ScenarioFlow.Scripts.Editor
{
	public static class ScenarioScriptViewer
    {
        private static readonly string BracketsColor = "FFD500";
        private static readonly string ArgumentColor = "D2A6FF";
        private static readonly string LabelColor = "95E6CB";

        public static void ShowScenarioScript(IScenarioScript scenarioScript)
        {
            var richLabelStyle = new GUIStyle(EditorStyles.label);
            richLabelStyle.wordWrap = true;
            richLabelStyle.richText = true;

            var labelDictionary = scenarioScript.LabelDictionary;

            var lineList = scenarioScript.Lines
                .Select(line => string.Join(" ", line.Select(element => $"{AddColor("{", BracketsColor)}{AddColor(AddEscape(element), ArgumentColor)}{AddColor("}", BracketsColor)}"))).ToList();

            foreach (var label in labelDictionary
                .OrderByDescending(pair => pair.Value)
                .ThenByDescending(pair => pair.Key))
            {
                lineList.Insert(label.Value, $"{AddColor(AddEscape(label.Key), LabelColor)}:");
            }

            GUI.enabled = true;
            var lineIndexWidth = Mathf.CeilToInt(Mathf.Log10(lineList.Count)) * 10.0f;
            var offSet = 0;
            foreach (var (line, index) in lineList.Select((line, index) => (line, index)))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (line.EndsWith(":"))
                    {
                        GUILayout.Label("", GUILayout.Width(lineIndexWidth));
                        offSet++;
                    }
                    else
                    {
                        GUILayout.Label((index - offSet + 1).ToString(), GUILayout.Width(lineIndexWidth));
                    }
                    GUILayout.Label(line, richLabelStyle);
                }
            }
            GUI.enabled = false;

            string AddEscape(string text)
            {
                return text.Replace("<", "<\u200B");
            }

            string AddColor(string text, string color)
            {
                return $"<color=#{color}>{text}</color>";
            }
        }
    }
}

#endif