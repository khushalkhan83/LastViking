using CodeStage.AntiCheat.ObscuredTypes;
using System;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ObscuredIDAttribute))]
    internal class ObscuredIDDrawer : ObscuredPropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            var attribute = base.attribute as ObscuredIDAttribute;
            var type = attribute.Type;
            var names = Enum.GetNames(type);

            var hiddenValue = prop.FindPropertyRelative("hiddenValue");
            SetBoldIfValueOverridePrefab(prop, hiddenValue);

            var cryptoKey = prop.FindPropertyRelative("currentCryptoKey");
            var inited = prop.FindPropertyRelative("inited");
            var fakeValue = prop.FindPropertyRelative("fakeValue");
            var fakeValueActive = prop.FindPropertyRelative("fakeValueActive");

            var currentCryptoKey = cryptoKey.intValue;
            var val = 0;

            if (!inited.boolValue)
            {
                if (currentCryptoKey == 0)
                {
                    currentCryptoKey = cryptoKey.intValue = ObscuredInt.cryptoKeyEditor;
                }
                hiddenValue.intValue = ObscuredInt.Encrypt(0, currentCryptoKey);
                inited.boolValue = true;
            }
            else
            {
                val = ObscuredInt.Decrypt(hiddenValue.intValue, currentCryptoKey);
            }

            EditorGUI.BeginChangeCheck();
            val = EditorGUI.Popup(position, label.text, val, names);
            if (EditorGUI.EndChangeCheck())
            {
                hiddenValue.intValue = ObscuredInt.Encrypt(val, currentCryptoKey);
                fakeValue.intValue = val;
                fakeValueActive.boolValue = true;
            }

            ResetBoldFont();
        }
    }
}
