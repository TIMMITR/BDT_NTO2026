using Plugins.EasyTransitions.Scripts;
using UnityEditor;

namespace Plugins.EasyTransitions.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TransitionManager))]
    public class TransitionManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
        }
    }

}
    
