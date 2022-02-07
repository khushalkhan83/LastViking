
using System;
using UnityEditor;
namespace Extensions.Editor
{
    public static class EditorExtensions
    {
        public static void AbstractPropertyDrawer(this SerializedProperty property)
        {
            if (property == null)
                throw new ArgumentNullException("Exeption");

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                if (property.objectReferenceValue == null)
                {
                    //field is null, provide object field for user to insert instance to draw
                    EditorGUILayout.PropertyField(property);
                    return;
                }
                System.Type concreteType = property.objectReferenceValue.GetType();
                UnityEngine.Object castedObject = (UnityEngine.Object)System.Convert.ChangeType(property.objectReferenceValue, concreteType);

                UnityEditor.Editor editor = UnityEditor.Editor.CreateEditor(castedObject);

                editor.OnInspectorGUI();
            }
            else
            {
                //otherwise fallback to normal property field
                EditorGUILayout.PropertyField(property);
            }
        }
    }
}