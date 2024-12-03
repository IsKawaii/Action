#if UNITY_EDITOR
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEditor;

namespace ScenarioFlow.Scripts.SFText.Editor
{
    [ScriptedImporter(1, "sftxt")]
    public class SFTextImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (ctx.assetPath.Contains("~$"))
            {
                return;
            }

            var text = File.ReadAllText(ctx.assetPath);
            var sftext = ScriptableObject.CreateInstance<SFText>();
            sftext.BuildScript(text);
            ctx.AddObjectToAsset("main obj", sftext);
            ctx.SetMainObject(sftext);
        }

        [MenuItem("Assets/Create/ScenarioFlow/SFText Script", priority = -1)]
        public static void CreateNewSFTextFile()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile($"Assets/ScenarioFlow/ScenarioScripts/SFText/Editor/data/NewSFText.sftxt", "NewSFText.sftxt");
        }
    }
}
#endif