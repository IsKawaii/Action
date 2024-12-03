#if UNITY_EDITOR

using ScenarioFlow.Scripts.Editor;
using System.Linq;
using UnityEditor;

namespace ScenarioFlow.Scripts.Composite.Editor
{
	[CustomEditor(typeof(CompositeScenarioScript))]
    public class CompositeScenarioScriptEditor : UnityEditor.Editor
    {
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var compositeScript = (CompositeScenarioScript)target;
			if (compositeScript.SearchDuplicatedLabelAll().Count() > 0)
			{
				foreach (var label in compositeScript.SearchDuplicatedLabelAll())
				{
					EditorGUILayout.HelpBox($"Duplicated label: {label}", MessageType.Error);
				}
			}
			else
			{
				ScenarioScriptViewer.ShowScenarioScript(compositeScript);
			}
		}
	}
}

#endif