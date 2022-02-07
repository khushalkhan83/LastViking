using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
[CustomPropertyDrawer(typeof(EnumNamedArrayAttribute))]
public class DrawerEnumNamedArray : PropertyDrawer
{

    public override float GetPropertyHeight(SerializedProperty property,
                                   GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EnumNamedArrayAttribute enumNames = attribute as EnumNamedArrayAttribute;
        //propertyPath returns something like component_hp_max.Array.data[4]
        //so get the index from there
        int index = System.Convert.ToInt32(property.propertyPath.Substring(property.propertyPath.IndexOf("[")).Replace("[", "").Replace("]", ""));
        //change the label
        // label.text = enumNames.names[index];
        label.text = enumNames.names[enumNames.handleFirstAsNone ? index + 1: index];
        //draw field
        EditorGUI.PropertyField(position, property, label, true);
    }
}
