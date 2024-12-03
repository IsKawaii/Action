#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ScenarioFlow.Scripts.SFText.Editor
{
    [InitializeOnLoad]
    public class SFTextSnippetsBuilder : EditorWindow
    {
        private static readonly string PathDataPath = $"Assets/ScenarioFlow/ScenarioScripts/SFText/Editor/data/paths.json";
        private static readonly string SnippetDataPath = $"Assets/ScenarioFlow/ScenarioScripts/SFText/Editor/data/snippets.json";

        private bool isEditMode = false;

        private List<TextAsset> jsonTextAssetList = new List<TextAsset>();
        private MethodInfo[] commandMethodInfos;
        private Dictionary<string, bool> commandEnablementDictionary;

        //0: by Category, 1: by Class
        private int commandToolBarIndex = 0;
        private static readonly string[] CommandToolBarSelections = new string[] { "Category", "Class" };
        private Vector2 commandScrollPosition = Vector2.zero;

        [MenuItem("Window/ScenarioFlow/SFText Snippets Builder")]
        private static void CreateWindow()
        {
            GetWindow<SFTextSnippetsBuilder>("SFText Snippets Builder");
        }

        [MenuItem("Assets/Create/ScenarioFlow/SFText Snippets", priority = -1)]
        public static void CreateNewJsonFile()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(SnippetDataPath, "SFTextSnippets.json");
        }

        private void OnEnable()
        {
            LoadData();
        }

        private void OnGUI()
        {
            GUI.enabled = !isEditMode;

            using (new EditorGUILayout.VerticalScope())
            {
                GUILayout.Label("Operations");
                ShowHorizontalLine();
                ShowHorizontalLine();

                if (GUILayout.Button("Copy JSON to Clipboard"))
                {
                    try
                    {
                        EditorGUIUtility.systemCopyBuffer = GetJsonText();
                        Debug.Log("The snippets JSON text have been successfully copied to the clipboard.");
                    }
                    catch (IOException ex)
                    {
                        Debug.LogException(ex);
                    }
                    catch (InvalidOperationException ex)
                    {
                        Debug.LogException(ex);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                        throw;
                    }
                }

                ShowEmptyLine();

                if (GUILayout.Button("Update JSON Files"))
                {
                    try
                    {
                        var json = GetJsonText();
                        foreach (var jsonTextAsset in jsonTextAssetList.Where(t => t != null))
                        {
                            var path = AssetDatabase.GetAssetPath(jsonTextAsset);
                            if (File.Exists(path))
                            {
                                File.WriteAllText(path, json);
                            }
                        }
                        Debug.Log("All the JSON files have been successfully updated.");
                        AssetDatabase.Refresh();
                    }
                    catch (IOException ex)
                    {
                        Debug.LogException(ex);
                    }
                    catch (InvalidOperationException ex)
                    {
                        Debug.LogException(ex);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                        throw;
                    }
                }
            }

            GUI.enabled = true;
            ShowEmptyLine();

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Settings");

                if (isEditMode)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Save"))
                        {
                            isEditMode = false;
                            SaveData();
                        }
                        if (GUILayout.Button("Revert"))
                        {
                            isEditMode = false;
                            LoadData();
                        }
                    }
                }
                else
                {
                    if (GUILayout.Button("Edit"))
                    {
                        isEditMode = true;
                    }
                }
            }

            ShowHorizontalLine();
            ShowHorizontalLine();

            using (new EditorGUILayout.VerticalScope("Box"))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("JSON Files to Be Updated");
                    GUI.enabled = isEditMode;
                    if (GUILayout.Button("Add JSON Text"))
                    {
                        jsonTextAssetList.Add(null);
                    }
                }

                ShowHorizontalLine();
                if (jsonTextAssetList.Count == 0)
                {
                    GUILayout.Label("No JSON files are registered.");
                }

                foreach (var index in Enumerable.Range(0, jsonTextAssetList.Count))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        jsonTextAssetList[index] = (TextAsset)EditorGUILayout.ObjectField("", jsonTextAssetList[index], typeof(TextAsset), false);
                        if (GUILayout.Button("Remove"))
                        {
                            jsonTextAssetList.RemoveAt(index);
                            break;
                        }
                    }
                }

                GUI.enabled = true;
            }

            using (new EditorGUILayout.VerticalScope("Box"))
            {
                GUILayout.Label("Command List");
                ShowHorizontalLine();
                ShowHorizontalLine();

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("Group by");
                    commandToolBarIndex = GUILayout.Toolbar(commandToolBarIndex, CommandToolBarSelections);
                }
                ShowHorizontalLine();

                using (var commandScrollScope = new EditorGUILayout.ScrollViewScope(commandScrollPosition))
                {
                    commandScrollPosition = commandScrollScope.scrollPosition;

                    if (commandToolBarIndex == 0)
                    {
                        foreach (var group in GetCommandMethodInfos()
                            .GroupBy(method => method.GetCustomAttribute<CategoryAttribute>()?.Name)
                            .OrderBy(group => group.Key))
                        {
                            using (new EditorGUILayout.VerticalScope("Box"))
                            {
                                ShowGroupToggle(group, "No Category", "(Category)");
                                ShowHorizontalLine();
                                using (new EditorGUILayout.VerticalScope("Box"))
                                {
                                    foreach (var method in group.OrderBy(info => info.GetCustomAttribute<CommandMethodAttribute>().Name))
                                    {
                                        ShowCommand(method);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var groupByNamespace in GetCommandMethodInfos().GroupBy(method => method.DeclaringType.Namespace))
                        {
                            ShowGroupToggle(groupByNamespace, "No Namespace", "(Namespace)");
                            ShowHorizontalLine();
                            ShowHorizontalLine();
                            foreach (var groupByClass in groupByNamespace
                                .GroupBy(method => method.DeclaringType.Name)
                                .OrderBy(group => group.Key))
                            {
                                using (new EditorGUILayout.VerticalScope("Box"))
                                {
                                    ShowGroupToggle(groupByClass, "No Class", "(Class)");
                                    ShowHorizontalLine();
                                    using (new EditorGUILayout.VerticalScope("Box"))
                                    {
                                        foreach (var method in groupByClass.OrderBy(info => info.GetCustomAttribute<CommandMethodAttribute>().Name))
                                        {
                                            ShowCommand(method);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            void ShowCommand(MethodInfo methodInfo)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUI.enabled = isEditMode;
                    var toggleStyle = new GUIStyle(EditorStyles.toggle);
                    toggleStyle.richText = true;
                    commandEnablementDictionary[GetMethodInfoId(methodInfo)] = GUILayout.Toggle(commandEnablementDictionary[GetMethodInfoId(methodInfo)], $"<color=#cccccc>{methodInfo.GetCustomAttribute<CommandMethodAttribute>().Name}</color>", toggleStyle);
                    GUI.enabled = true;
                }
            }

            void ShowGroupToggle(IGrouping<string, MethodInfo> group, string defaultKey, string extraText)
            {
                GUI.enabled = isEditMode;
                var isEnabledAny = group.Any(method => IsCommandEnabled(method));
                var toggleStyle = new GUIStyle(EditorStyles.toggle);
                toggleStyle.richText = true;
                toggleStyle.fontStyle = FontStyle.BoldAndItalic;
                var toggle = GUILayout.Toggle(isEnabledAny, $"<color=#eeeeee>{group.Key ?? defaultKey} {extraText}</color>", toggleStyle);
                if (toggle && !isEnabledAny)
                {
                    foreach (var method in group)
                    {
                        ToggleCommand(method, true);
                    }
                }
                else if (!toggle && isEnabledAny)
                {
                    foreach (var method in group)
                    {
                        ToggleCommand(method, false);
                    }
                }
                GUI.enabled = true;
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

        private string GetJsonText()
        {
            //Extract all classes that implement the 'IReflectable' interface
            //Extract all methods that has the 'ScenarioMethod' attribute
            var enableMethodInfos = commandMethodInfos.Where(info => commandEnablementDictionary[GetMethodInfoId(info)]);

            //Check dupulicate command names
            var duplicateNameList = new List<string>();
            var commandNameList = new List<string>(enableMethodInfos.Count());
            foreach (var method in enableMethodInfos)
            {
                var commandName = method.GetCustomAttribute<CommandMethodAttribute>().Name;
                if (commandNameList.Contains(commandName))
                {
                    if (!duplicateNameList.Contains(commandName))
                    {
                        duplicateNameList.Add(commandName);
                    }
                }
                else
                {
                    commandNameList.Add(commandName);
                }
            }
            if (duplicateNameList.Count > 0)
            {
                var duplicateCommandMessages = enableMethodInfos
                    .Where(info => duplicateNameList.Contains(info.GetCustomAttribute<CommandMethodAttribute>().Name))
                    .Select(info => $"{info.GetCustomAttribute<CommandMethodAttribute>().Name} - {info.DeclaringType.FullName} {info}");
                var errorMessage = string.Join("\n", duplicateCommandMessages);
                throw new InvalidOperationException($"Duplicate command names were detected. Disable them appropriately and make sure that every enable commands has an unique name.\n\n{errorMessage}\n");
            }

            var snippetMaterialSet = new SnippetMaterialSet(enableMethodInfos);

            return JsonUtility.ToJson(snippetMaterialSet);
        }

        private void SaveData()
        {
            var config = new Config
            {
                JsonPaths = jsonTextAssetList.Select(asset => AssetDatabase.GetAssetPath(asset)).ToArray(),
                CommandConfigs = commandEnablementDictionary.Select(pair => new CommandConfig
                {
                    Id = pair.Key,
                    IsValid = pair.Value,
                }).ToArray()
            };
            var json = JsonUtility.ToJson(config);
            File.WriteAllText(PathDataPath, json);
            AssetDatabase.Refresh();
        }

        private void LoadData()
        {
            if (File.Exists(PathDataPath))
            {
                var json = File.ReadAllText(PathDataPath);
                var config = JsonUtility.FromJson<Config>(json);
                jsonTextAssetList = config.JsonPaths.Select(path => AssetDatabase.LoadAssetAtPath<TextAsset>(path)).Where(asset => asset != null).ToList();
                commandMethodInfos = GetCommandMethodInfos().ToArray();
                commandEnablementDictionary = commandMethodInfos.ToDictionary(info => GetMethodInfoId(info), info => true);
                if (config != null && config.CommandConfigs != null && config.JsonPaths != null)
                {
                    foreach (var commandConfig in config.CommandConfigs.Where(config => commandEnablementDictionary.ContainsKey(config.Id)))
                    {
                        commandEnablementDictionary[commandConfig.Id] = commandConfig.IsValid;
                    }
                }
            }
            else
            {
                commandMethodInfos = GetCommandMethodInfos().ToArray();
                commandEnablementDictionary = commandMethodInfos.ToDictionary(info => GetMethodInfoId(info), info => true);
            }
        }

        private string GetMethodInfoId(MethodInfo methodInfo)
        {
            return $"{methodInfo.DeclaringType.FullName} {methodInfo.ReturnType.FullName} {methodInfo.Name} {string.Join(' ', methodInfo.GetParameters().Select(p => p.ParameterType.FullName))}";
        }

        private void ToggleCommand(MethodInfo methodInfo, bool isEnable)
        {
            commandEnablementDictionary[GetMethodInfoId(methodInfo)] = isEnable;
        }

        private bool IsCommandEnabled(MethodInfo methodInfo)
        {
            return commandEnablementDictionary[GetMethodInfoId(methodInfo)];
        }

        private IEnumerable<MethodInfo> GetCommandMethodInfos()
        {
            return SFTextCommandList.CommandMethodInfos;
        }

        [Serializable]
        private class Config
        {
            public string[] JsonPaths;
            public CommandConfig[] CommandConfigs;
        }

        [Serializable]
        private class CommandConfig
        {
            public string Id;

            public bool IsValid;
        }

        [Serializable]
        private class SnippetMaterial
        {
            [SerializeField]
            private string name;
            [SerializeField]
            private string category;
            [SerializeField]
            private bool isAsync;
            [SerializeField]
            private string[] descriptions;
            [SerializeField]
            private string[] snippets;
            [SerializeField]
            private string[] dialogueSnippets;

            public SnippetMaterial(MethodInfo methodInfo)
            {
                name = methodInfo.GetCustomAttribute<CommandMethodAttribute>().Name;
                category = methodInfo.GetCustomAttribute<CategoryAttribute>()?.Name;
                isAsync = methodInfo.ReturnType == typeof(UniTask);
                descriptions = methodInfo.GetCustomAttributes<DescriptionAttribute>().Select(a => a.Text).ToArray();
                snippets = methodInfo.GetCustomAttributes<SnippetAttribute>().Select(a => a.Text).ToArray();
                dialogueSnippets = methodInfo.GetCustomAttributes<DialogueSnippetAttribute>().Select(a => a.Text).ToArray();
            }
        }

        [Serializable]
        private class SnippetMaterialSet
        {
            [SerializeField]
            private SnippetMaterial[] snippetMaterials;

            public SnippetMaterialSet(IEnumerable<MethodInfo> methodInfos)
            {
                snippetMaterials = methodInfos.Select(info => new SnippetMaterial(info)).ToArray();
            }
        }
    }
}
#endif