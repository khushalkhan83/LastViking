using UnityEditor;
using UnityEngine;

namespace Game.Models
{
    [CustomPropertyDrawer(typeof(ShelterCost))]
    public class ShelterCostPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var name = property.FindPropertyRelative("_name");
            var cost = property.FindPropertyRelative("_count");

            var namePosition = position;
            namePosition.xMax -= 100;

            var costPosition = position;
            costPosition.xMin = namePosition.xMax;
            costPosition.width = 100;

            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            EditorGUI.PropertyField(namePosition, name, GUIContent.none);
            EditorGUI.PropertyField(costPosition, cost, GUIContent.none);

            EditorGUI.indentLevel = indentLevel;
        }
    }
}
