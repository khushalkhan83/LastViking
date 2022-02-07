﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace UltimateSurvival.Editor
{
    [CustomPropertyDrawer(typeof(TransformOffset))]
    public class TransformOffsetDrawer : PropertyDrawer
    {
        private static TransformOffset m_CopiedObject;
        private static SerializedProperty m_InspectedProperty;


        [MenuItem("CONTEXT/TransformOffset/Copy Offset")]
        private static void Copy(MenuCommand command)
        {
            if (m_InspectedProperty != null)
                m_CopiedObject = m_InspectedProperty.GetValue<TransformOffset>().GetClone();
        }

        [MenuItem("CONTEXT/TransformOffset/Paste Offset")]
        private static void Paste(MenuCommand command)
        {
            if (m_InspectedProperty != null && m_CopiedObject != null)
            {
                Undo.RecordObject(m_InspectedProperty.serializedObject.targetObject, "TransformOffsetHolder123");
                m_InspectedProperty.serializedObject.Update();
                m_InspectedProperty.SetValue<TransformOffset>(m_CopiedObject.GetClone());
                m_InspectedProperty.serializedObject.ApplyModifiedProperties();

                EditorUtility.SetDirty(m_InspectedProperty.serializedObject.targetObject);
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var current = Event.current;
            if (current.type == EventType.ContextClick)
            {
                var mousePos = current.mousePosition;
                if (position.Contains(mousePos))
                {
                    m_InspectedProperty = property.Copy();
                    property.serializedObject.Update();
                    EditorUtility.DisplayPopupMenu(new Rect(mousePos.x, mousePos.y, 0, 0), "CONTEXT/TransformOffset/", null);
                }
            }
            else
                EditorGUI.PropertyField(position, property, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = property.isExpanded ? 20f : 16f;
            if (property.isExpanded)
            {
                height += property.CountInProperty() * 16f;
                height += 20f;
            }

            return height;
        }
    }
}

#endif