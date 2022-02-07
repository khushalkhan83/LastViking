#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using GenerateID = UltimateSurvival.CellSpawnSettings.GenerateID;
using CountID = UltimateSurvival.CellSpawnSettings.CountID;

namespace UltimateSurvival.Editor
{
    //[CustomPropertyDrawer(typeof(CellSpawnSettings))]
    public class CellSpawnSettingsDrawer : PropertyDrawer
    {
        public const float HEIGHT = 16;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var _generatorType = property.FindPropertyRelative("_generatorType");
            var _items = property.FindPropertyRelative("_items");
            var _itemName = property.FindPropertyRelative("_itemName");
            var _categoryName = property.FindPropertyRelative("_categoryName");
            var _categoryNames = property.FindPropertyRelative("_categoryNames");
            var _countType = property.FindPropertyRelative("_countType");
            var _count = property.FindPropertyRelative("_count");
            var _countMin = property.FindPropertyRelative("_countMin");
            var _countMax = property.FindPropertyRelative("_countMax");
            var _countPercent = property.FindPropertyRelative("_countPercent");

            GUI.Box(position, GUIContent.none, GUI.skin.box);

            position.height = HEIGHT;
            EditorGUI.PropertyField(position, _generatorType);
            var generatorID = (GenerateID)_generatorType.enumValueIndex;
            switch (generatorID)
            {
                case GenerateID.RandomFromItems:

                    position.y = position.yMax;
                    _items.arraySize = EditorGUI.IntField(position, "Count", _items.arraySize);
                    for (int i = 0; i < _items.arraySize; i++)
                    {
                        position.y = position.yMax;
                        EditorGUI.PropertyField(position, _items.GetArrayElementAtIndex(i), GUIContent.none);
                    }

                    break;
                case GenerateID.RandomFromCategories:

                    position.y = position.yMax;
                    _categoryNames.arraySize = EditorGUI.IntField(position, "Count", _categoryNames.arraySize);
                    for (int i = 0; i < _categoryNames.arraySize; i++)
                    {
                        position.y = position.yMax;
                        EditorGUI.PropertyField(position, _categoryNames.GetArrayElementAtIndex(i), GUIContent.none);
                    }

                    break;
                case GenerateID.RandomFromCategory:

                    position.y = position.yMax;
                    EditorGUI.PropertyField(position, _categoryName);

                    break;
                case GenerateID.ConcreteItem:

                    position.y = position.yMax;
                    EditorGUI.PropertyField(position, _itemName);

                    break;
            }

            if (generatorID != GenerateID.EmptyItem)
            {
                position.y = position.yMax;
                EditorGUI.PropertyField(position, _countType);

                switch ((CountID)_countType.enumValueIndex)
                {
                    case CountID.Count:

                        position.y = position.yMax;
                        EditorGUI.PropertyField(position, _count);

                        break;
                    case CountID.RandomFromRange:

                        position.y = position.yMax;
                        EditorGUI.PropertyField(position, _countMin);

                        position.y = position.yMax;
                        EditorGUI.PropertyField(position, _countMax);

                        break;
                    case CountID.PercentOfMaxStack:

                        position.y = position.yMax;
                        EditorGUI.PropertyField(position, _countPercent);

                        break;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var _generatorType = property.FindPropertyRelative("_generatorType");
            var _countType = property.FindPropertyRelative("_countType");
            var _items = property.FindPropertyRelative("_items");
            var _categoryNames = property.FindPropertyRelative("_categoryNames");

            var generatorID = (GenerateID)_generatorType.enumValueIndex;

            var height = HEIGHT;
            switch (generatorID)
            {
                case GenerateID.RandomFromItems:
                    height += HEIGHT;
                    height += HEIGHT * _items.arraySize;
                    break;
                case GenerateID.RandomFromCategories:
                    height += HEIGHT;
                    height += HEIGHT * _categoryNames.arraySize;
                    break;
                case GenerateID.RandomFromCategory:
                    height += HEIGHT;
                    break;
                case GenerateID.ConcreteItem:
                    height += HEIGHT;
                    break;
            }

            if (generatorID != GenerateID.EmptyItem)
            {
                height += HEIGHT;
                var countID = (CountID)_countType.enumValueIndex;

                switch (countID)
                {
                    case CountID.Count:
                        height += HEIGHT;
                        break;
                    case CountID.RandomFromRange:
                        height += HEIGHT;
                        height += HEIGHT;
                        break;
                    case CountID.PercentOfMaxStack:
                        height += HEIGHT;
                        break;
                }
            }
            return height;
        }
    }
}

#endif