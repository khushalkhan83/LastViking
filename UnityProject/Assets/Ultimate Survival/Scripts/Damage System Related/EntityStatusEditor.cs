#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace UltimateSurvival.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GenericVitals), true)]
    public class EntityStatusEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.Space();

                float healthValue = (serializedObject.targetObject as GenericVitals).Entity.Health.Value;
                EditorGUILayout.HelpBox(string.Format("Health: {0}", healthValue), MessageType.Info);
                Repaint();
            }

            base.OnInspectorGUI();
        }
    }
}

#endif