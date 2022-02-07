using Core.Storage;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DataBase), true)]
public class DataBaseEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var pos = position;

        pos.xMin = pos.xMax - 16;
        pos.height = 16;

        if (GUI.Button(pos, "-"))
        {
            File.Delete(Path.Combine(Application.persistentDataPath, property.FindPropertyRelative("_uuid").stringValue));
        }

        pos = position;
        EditorGUI.PropertyField(pos, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
}
