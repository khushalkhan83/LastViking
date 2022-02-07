using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomPropertyDrawer(typeof(UUIDAttribute), true)]
public class UUIDAttributeDrawer : PropertyDrawer
{
    const float WIDTH = 32;

    readonly static Color buttonColor = new Color(1, 0.7f, 0.7f);
    readonly static Color labelColor = new Color(0f, 0.7f, 0.7f);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        if (string.IsNullOrEmpty(property.stringValue))
        {
            var backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = buttonColor;
            if (GUI.Button(position, "Generate UUID"))
            {
                property.stringValue = GUID.Generate().ToString();
            }
            GUI.backgroundColor = backgroundColor;
        }
        else
        {
            var positionLabel = position;
            positionLabel.xMax -= 16;
            var positionButton = position;
            positionButton.xMin = positionButton.xMax - 16;

            var contentColor = GUI.contentColor;
            GUI.contentColor = labelColor;
            GUI.Label(positionLabel, $"{property.name}:[{property.stringValue}]");
            if (GUI.Button(positionButton, "*"))
            {
                property.stringValue = GUID.Generate().ToString();
            }
            GUI.contentColor = contentColor;
        }
    }
}
