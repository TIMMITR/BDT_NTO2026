#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Generators
{
    public class FeatureGenerator : EditorWindow
    {
        private string _packName = "Name";
        private string _targetFolder = "Assets/Scripts/Features";

        private bool _createRequests;
        private bool _createEvents;

        [MenuItem("Assets/Create/System Pack", false, 81)]
        public static void OpenFromProject()
        {
            var window = GetWindow<FeatureGenerator>("New Feature Pack");
            window._targetFolder = GetCurrentSelectedFolderPath();
            window.minSize = new Vector2(360, 220);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Creation of Feature Pack (Flat)", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Folder:", _targetFolder);
            _packName = EditorGUILayout.TextField("Feature Name:", _packName);

            EditorGUILayout.Space();
            GUILayout.Label("Optional Modules:", EditorStyles.boldLabel);
            _createRequests = EditorGUILayout.Toggle("Include Request", _createRequests);
            _createEvents = EditorGUILayout.Toggle("Include Event", _createEvents);

            EditorGUILayout.Space();

            string preview = $"Will be created:\n" +
                             $"• Folder: {_targetFolder}/{_packName}Pack/\n" +
                             $"  ├── {_packName}Component.cs\n" +
                             $"  ├── {_packName}System.cs\n";

            if (_createRequests) preview += $"  ├── {_packName}Request.cs\n";
            if (_createEvents)   preview += $"  └── {_packName}Event.cs\n";

            EditorGUILayout.HelpBox(preview, MessageType.Info);
            EditorGUILayout.Space();

            if (GUILayout.Button("Create Feature Pack", GUILayout.Height(30)))
            {
                if (string.IsNullOrWhiteSpace(_packName))
                {
                    EditorUtility.DisplayDialog("Error", "Name cannot be empty!", "OK");
                    return;
                }

                CreatePack();
                Close();
            }
        }

        private void CreatePack()
        {
            string rootPackDir = Path.Combine(_targetFolder, $"{_packName}Pack");
            if (!Directory.Exists(rootPackDir))
            {
                Directory.CreateDirectory(rootPackDir);
            }

            File.WriteAllText(
                Path.Combine(rootPackDir, $"{_packName}Component.cs"), 
                GetComponentTemplate(_packName)
            );

            File.WriteAllText(
                Path.Combine(rootPackDir, $"{_packName}System.cs"), 
                GetSystemTemplate(_packName)
            );

            if (_createRequests)
            {
                File.WriteAllText(
                    Path.Combine(rootPackDir, $"{_packName}Request.cs"), 
                    GetRequestTemplate(_packName)
                );
            }

            if (_createEvents)
            {
                File.WriteAllText(
                    Path.Combine(rootPackDir, $"{_packName}Event.cs"), 
                    GetEventTemplate(_packName)
                );
            }

            AssetDatabase.Refresh();

            var createdFolder = AssetDatabase.LoadAssetAtPath<Object>(rootPackDir.Replace("\\", "/"));
            if (createdFolder != null)
            {
                Selection.activeObject = createdFolder;
                EditorGUIUtility.PingObject(createdFolder);
            }

            Debug.Log($"<color=green>[Morpeh]</color> Feature Pack <b>{_packName}Pack</b> successfully generated!");
        }

        private static string GetCurrentSelectedFolderPath()
        {
            string path = "Assets";
            foreach (var obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
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

        #region Code Templates

        private static string GetComponentTemplate(string name)
        {
            return 
$@"using Scellecs.Morpeh;
using UnityEngine;

namespace Features.{name}Pack
{{
    public struct {name}Component : IComponent
    {{
        // public float Value;
    }}
}}";
        }

        private static string GetSystemTemplate(string name)
        {
            return 
$@"using Scellecs.Morpeh;
using UnityEngine;

namespace Features.{name}Pack
{{
    public sealed class {name}System : ISystem
    {{
        public World World {{ get; set; }}

        private Filter _filter;
        private Stash<{name}Component> _{name}Stash;

        public void OnAwake()
        {{
            _filter = World.Filter.With<{name}Component>().Build();
            _{name}Stash = World.GetStash<{name}Component>();
        }}

        public void OnUpdate(float deltaTime)
        {{
            foreach (var entity in _filter)
            {{
                ref var component = ref _{name}Stash.Get(entity);
            }}
        }}

        public void Dispose() {{ }}
    }}
}}";
        }

        private static string GetRequestTemplate(string name)
        {
            return 
$@"using Scellecs.Morpeh;

namespace Features.{name}Pack
{{
    public struct {name}Request : IRequestData
    {{
    }}
}}";
        }

        private static string GetEventTemplate(string name)
        {
            return 
$@"using Scellecs.Morpeh;

namespace Features.{name}Pack
{{
    public struct {name}Event : IEventData
    {{
    }}
}}";
        }

        #endregion
    }
}
#endif