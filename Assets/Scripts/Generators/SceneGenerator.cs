#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Generators
{
    public class SceneGenerator : EditorWindow
    {
        private string _packName = "Name";
        private string _targetFolder = "Assets/Scripts/Scenes";

        [MenuItem("Assets/Create/Scene Pack", false, 80)]
        public static void OpenFromProject()
        {
            var window = GetWindow<SceneGenerator>("New Scene Pack");
            window._targetFolder = GetCurrentSelectedFolderPath();
            window.minSize = new Vector2(350, 160);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Creation of Scene Pack", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Folder:", _targetFolder);
            _packName = EditorGUILayout.TextField("Name:", _packName);

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                $"Will be created:\n" +
                $"• Folder: {_targetFolder}/{_packName}/\n" +
                $"• Scene: {_packName}Scene.unity\n" +
                $"• Scripts: {_packName}StartUp.cs and {_packName}Context.cs\n",
                MessageType.Info);

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Scene Pack", GUILayout.Height(30)))
            {
                if (string.IsNullOrWhiteSpace(_packName))
                {
                    EditorUtility.DisplayDialog("Error", "Name cannot be empty!", "OK");
                    return;
                }

                CreateModule();
                Close();
            }
        }

        private void CreateModule()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            string packDir = Path.Combine(_targetFolder, _packName);
            if (!Directory.Exists(packDir))
            {
                Directory.CreateDirectory(packDir);
            }

            string contextCode = GetContextTemplate(_packName);
            string contextPath = Path.Combine(packDir, $"{_packName}Context.cs");
            File.WriteAllText(contextPath, contextCode);

            string startUpCode = GetStartUpTemplate(_packName);
            string startUpPath = Path.Combine(packDir, $"{_packName}StartUp.cs");
            File.WriteAllText(startUpPath, startUpCode);

            var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            string scenePath = Path.Combine(packDir, $"{_packName}Scene.unity").Replace("\\", "/");
            EditorSceneManager.SaveScene(newScene, scenePath);
            
            EditorPrefs.SetString("Pending_ScenePath", scenePath);
            EditorPrefs.SetString("Pending_StartUpName", $"{_packName}StartUp");
            EditorPrefs.SetString("Pending_ContextName", $"{_packName}Context");

            AssetDatabase.Refresh();
        }

        private static string GetCurrentSelectedFolderPath()
        {
            string path = "Assets";
            foreach (var obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(path)) continue;

                if (Directory.Exists(path)) break;

                if (File.Exists(path))
                {
                    path = Path.GetDirectoryName(path); 
                    break;
                }
            }

            return path?.Replace("\\", "/");
        }

        private static string GetContextTemplate(string name)
        {
            return 
$@"using UnityEngine;

namespace Scenes.{name}
{{
    public class {name}Context : MonoBehaviour
    {{

    }}
}}";
        }

        private static string GetStartUpTemplate(string name)
        {
            return 
$@"using Scellecs.Morpeh;
using UnityEngine;

namespace Scenes.{name}
{{
    public class {name}StartUp : MonoBehaviour
    {{
        [SerializeField] private {name}Context context;

        private World _world;
        private SystemsGroup _updateGroup;
        private SystemsGroup _fixedGroup;

        private void Awake()
        {{
            _world = World.Default;

            _updateGroup = _world.CreateSystemsGroup();
            _updateGroup.Initialize();
            _world.AddSystemsGroup(order: 0, _updateGroup);

            _fixedGroup = _world.CreateSystemsGroup();
            _fixedGroup.Initialize();
        }}

        private void FixedUpdate()
        {{
            _fixedGroup?.Update(Time.fixedDeltaTime);
        }}

        private void OnDestroy()
        {{
            if (_updateGroup != null)
            {{
                _world?.RemoveSystemsGroup(_updateGroup);
                _updateGroup.Dispose();
                _updateGroup = null;
            }}

            if (_fixedGroup != null)
            {{
                _fixedGroup.Dispose();
                _fixedGroup = null;
            }}
        }}
    }}
}}";
        }
    }

    [InitializeOnLoad]
    public static class ScenePostProcessor
    {
        static ScenePostProcessor()
        {
            EditorApplication.delayCall += TrySetupScene;
        }

        private static void TrySetupScene()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += TrySetupScene;
                return;
            }

            if (!EditorPrefs.HasKey("Pending_ScenePath"))
                return;

            string scenePath = EditorPrefs.GetString("Pending_ScenePath");
            string startUpName = EditorPrefs.GetString("Pending_StartUpName");
            string contextName = EditorPrefs.GetString("Pending_ContextName");

            EditorPrefs.DeleteKey("Pending_ScenePath");
            EditorPrefs.DeleteKey("Pending_StartUpName");
            EditorPrefs.DeleteKey("Pending_ContextName");

            var startUpType = FindType(startUpName);
            var contextType = FindType(contextName);

            if (startUpType == null || contextType == null)
            {
                Debug.LogWarning("Can't find startup or context types");
                return;
            }

            var scene = EditorSceneManager.OpenScene(scenePath);

            var startUp = new GameObject("StartUp");
            var startUpComp = startUp.AddComponent(startUpType);
            
            var context = new GameObject("Context");
            var contextComp = context.AddComponent(contextType);

            var so = new SerializedObject(startUpComp);
            var prop = so.FindProperty("context");
            if (prop != null)
            {
                prop.objectReferenceValue = contextComp;
                so.ApplyModifiedProperties();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            Selection.activeGameObject = startUp;
            Debug.Log($"<color=green>[Morpeh]</color> Scene Pack successfully generated: <b>{scenePath}</b>");
        }

        private static Type FindType(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var type = assembly.GetTypes().FirstOrDefault(t => t.Name == typeName);
                    if (type != null) return type;
                }
                catch
                {
                    // ignored
                }
            }
            return null;
        }
    }
}
#endif