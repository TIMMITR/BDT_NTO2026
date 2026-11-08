using Plugins.EasyTransitions.Scripts;
using UnityEditor;
using UnityEngine;

namespace Plugins.EasyTransitions.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TransitionSettings))]
    public class TransitionSettingsEditor : UnityEditor.Editor
    {
        public Texture transitionManagerSettingsLogo;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (transitionManagerSettingsLogo)
            {
                float aspect = (float)transitionManagerSettingsLogo.width / transitionManagerSettingsLogo.height;
                float width = EditorGUIUtility.currentViewWidth - 30f;
                float height = width / aspect;

                var rect = GUILayoutUtility.GetRect(width, height);
                GUI.DrawTexture(rect, transitionManagerSettingsLogo, ScaleMode.ScaleToFit);
                EditorGUILayout.Space();
            }

            DrawDefaultInspector();
            serializedObject.ApplyModifiedProperties();
        }
    }
}