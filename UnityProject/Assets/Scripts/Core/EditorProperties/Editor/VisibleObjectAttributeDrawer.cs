using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomPropertyDrawer(typeof(VisibleObjectAttribute), true)]
public class VisibleObjectAttributeDrawer : PropertyDrawer
{
    const float WIDTH = 32;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.objectReferenceValue is GameObject)
        {
            var go = (GameObject)property.objectReferenceValue;
            go.SetActive(GUI.Toggle(new Rect(position.x, position.y, WIDTH, position.height), go.activeSelf, GUIContent.none));
        }
        else if (property.objectReferenceValue is Component)
        {
            var go = ((Behaviour)property.objectReferenceValue);
            go.enabled = (GUI.Toggle(new Rect(position.x, position.y, WIDTH, position.height), go.enabled, GUIContent.none));
        }

        EditorGUI.PropertyField(new Rect(WIDTH, position.y, position.width - WIDTH, position.height), property, label);
    }
}
