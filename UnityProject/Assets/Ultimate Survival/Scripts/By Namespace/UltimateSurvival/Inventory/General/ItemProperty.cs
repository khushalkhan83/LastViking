using CodeStage.AntiCheat.ObscuredTypes;
using Game.Audio;
using Game.Controllers;
using Game.Models;
using Game.Providers;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateSurvival
{
    public class ItemProperty
    {
        public enum Type
        {
            None,
            Bool,
            Int,
            IntRange,
            RandomInt,
            Float,
            FloatRange,
            RandomFloat,
            String,
            Sound,
            Prefab,
            AudioID,
            ItemCategoryID,
            AbilityID,
            PlayerWeaponID,
            PrefabID,
            ConstructionCategoryID,
            EquipmentCategory,
            EquimpentSet,
        }

        [Serializable]
        public class Int
        {
            public int Current { get { return m_Current; } set { m_Current = value; } }

            /// <summary>The default value, that was set up initially, when the item was defined.</summary>
            public int Default => m_Default;

            /// <summary>This is equal to Current / Default.</summary>
            public float Ratio => (float)m_Current / m_Default;

            #region Data
#pragma warning disable 0649

            [SerializeField] private ObscuredInt m_Current;

            [SerializeField] private ObscuredInt m_Default;

#pragma warning restore 0649
            #endregion

            public override string ToString()
            {
                return m_Current.ToString();
            }

            public Int Clone()
            {
                return new Int()
                {
                    m_Current = m_Current,
                    m_Default = m_Default
                };
            }
        }

        [Serializable]
        public class IntRange
        {
            public int Current { get { return m_Current; } set { m_Current = Mathf.Clamp(value, m_Min, m_Max); } }

            /// <summary>This is equal to Current / Max.</summary>
            public float Ratio => (float)m_Current / m_Max;

            public int Min { get { return m_Min; } set { m_Min = value; } }

            public int Max { get { return m_Max; } set { m_Max = value; } }

            #region Data
#pragma warning disable 0649

            [SerializeField]
            private ObscuredInt m_Current, m_Min, m_Max;

#pragma warning restore 0649
            #endregion

            public IntRange Clone()
            {
                return new IntRange()
                {
                    m_Current = m_Current,
                    m_Min = m_Min,
                    m_Max = m_Max
                };
            }

            public override string ToString()
            {
                return string.Format("{0} / {1}", Current, Max);
            }
        }

        [Serializable]
        public class RandomInt
        {
            public int RandomValue => UnityEngine.Random.Range(m_Min, m_Max);

            #region Data
#pragma warning disable 0649

            [SerializeField] private ObscuredInt m_Min;

            [SerializeField] private ObscuredInt m_Max;

#pragma warning restore 0649
            #endregion

            public override string ToString()
            {
                return string.Format("{0} - {1}", m_Min, m_Max);
            }

            public RandomInt Clone()
            {
                return new RandomInt()
                {
                    m_Min = m_Min,
                    m_Max = m_Max
                };
            }
        }

        [Serializable]
        public class Float
        {
            public float Current { get { return m_Current; } set { m_Current = value; } }

            /// <summary>The default value, that was set up initially, when the item was defined.</summary>
            public float Default => m_Default;

            /// <summary>This is equal to Current / Default.</summary>
            public float Ratio => m_Current / m_Default;

            [SerializeField]
            private ObscuredFloat m_Current;

            [SerializeField]
            private ObscuredFloat m_Default;

            public Float Clone()
            {
                return new Float()
                {
                    m_Current = m_Current,
                    m_Default = m_Default
                };
            }

            public override string ToString()
            {
                return m_Current.ToString();
            }

        }

        [Serializable]
        public class FloatRange
        {
            public float Current { get { return m_Current; } set { m_Current = Mathf.Clamp(value, m_Min, m_Max); } }

            /// <summary>This is equal to Current / Max.</summary>
            public float Ratio => m_Current / m_Max;

            public float Min { get { return m_Min; } set { m_Min = value; } }

            public float Max { get { return m_Max; } set { m_Max = value; } }

            [SerializeField]
            private ObscuredFloat m_Current, m_Min, m_Max;

            public override string ToString()
            {
                return string.Format("{0} / {1}", Current, Max);
            }

            public FloatRange Clone()
            {
                return new FloatRange()
                {
                    m_Current = m_Current,
                    m_Min = m_Min,
                    m_Max = m_Max
                };
            }
        }

        [Serializable]
        public class RandomFloat
        {
            public float RandomValue { get { return UnityEngine.Random.Range(m_Min, m_Max); } }

            #region Data
#pragma warning disable 0649

            [SerializeField] private ObscuredFloat m_Min;

            [SerializeField] private ObscuredFloat m_Max;

#pragma warning restore 0649
            #endregion

            public override string ToString()
            {
                return string.Format("{0} - {1}", m_Min, m_Max);
            }

            public RandomFloat Clone()
            {
                return new RandomFloat()
                {
                    m_Min = m_Min,
                    m_Max = m_Max
                };
            }
        }

        [Serializable]
        public class Definition
        {
            public string Name { get { return m_Name; } }

            public Type Type { get { return (Type)(int)m_Type; } }

            #region Data
#pragma warning disable 0649

            [SerializeField] private ObscuredString m_Name;

            [ObscuredID(typeof(Type))]
            [SerializeField] private ObscuredInt m_Type;

#pragma warning restore 0649
            #endregion

            public Definition Clone()
            {
                return new Definition()
                {
                    m_Name = m_Name,
                    m_Type = m_Type,
                };
            }
        }

        [Serializable]
        public class Value
        {
            #region Data
#pragma warning disable 0649

            [SerializeField] private ObscuredString m_Name;
            [ObscuredID(typeof(Type))]
            [SerializeField] private ObscuredInt m_Type;
            [SerializeField] private ObscuredBool m_Bool;
            [SerializeField] private Int m_Int;
            [SerializeField] private IntRange m_IntRange;
            [SerializeField] private RandomInt m_RandomInt;
            [SerializeField] private Float m_Float;
            [SerializeField] private FloatRange m_FloatRange;
            [SerializeField] private RandomFloat m_RandomFloat;
            [SerializeField] private ObscuredString m_String;
            [SerializeField] private AudioClip m_Sound;
            [SerializeField] private GameObject m_Prefab;
            [SerializeField] private AudioID _audioID;
            [SerializeField] private ItemCategoryID _itemCategoryID;
            [SerializeField] private AbilityID _abilityID;
            [SerializeField] private PlayerWeaponID _playerWeaponID;
            [SerializeField] private PrefabID _prefabID;
            [SerializeField] private ConstructionCategoryID _constructionCategoryID;
            [SerializeField] private EquipmentCategory _equipmentCategory;
            [SerializeField] private EquipmentSet _equipmentSet;

#pragma warning restore 0649
            #endregion

            public Message<Value> Changed = new Message<Value>();

            public Type Type { get { return (Type)(int)m_Type;} }

            public string Name => m_Name;
            public bool Bool => m_Bool;
            public Int Int => m_Int;
            public IntRange IntRange => m_IntRange;
            public RandomInt RandomInt => m_RandomInt;
            public Float Float => m_Float;
            public FloatRange FloatRange => m_FloatRange;
            public RandomFloat RandomFloat => m_RandomFloat;
            public string String => m_String;
            public AudioClip Sound => m_Sound;
            public GameObject Prefab => m_Prefab;
            public AudioID AudioID => _audioID;
            public ItemCategoryID ItemCategoryID => _itemCategoryID;
            public PlayerWeaponID PlayerWeaponID => _playerWeaponID;
            public AbilityID AbilityID => _abilityID;
            public PrefabID PrefabID => _prefabID;
            public ConstructionCategoryID ConstructionCategoryID => _constructionCategoryID;
            public EquipmentCategory EquipmentCategory => _equipmentCategory;
            public EquipmentSet EquipmentSet => _equipmentSet;

            public void SetName(string name) => m_Name = name;

            public Value Clone()
            {
                return new Value()
                {
                    m_Type = m_Type,
                    Changed = new Message<Value>(),
                    m_Bool = m_Bool,
                    m_Float = m_Float?.Clone(),
                    m_FloatRange = m_FloatRange?.Clone(),
                    m_Int = m_Int?.Clone(),
                    m_IntRange = m_IntRange?.Clone(),
                    m_Name = m_Name,
                    m_Prefab = m_Prefab,
                    m_RandomFloat = m_RandomFloat?.Clone(),
                    m_RandomInt = m_RandomInt?.Clone(),
                    m_Sound = m_Sound,
                    m_String = m_String,
                    _audioID = _audioID,
                    _itemCategoryID = _itemCategoryID,
                    _abilityID = _abilityID,
                    _playerWeaponID = _playerWeaponID,
                    _prefabID = _prefabID,
                    _constructionCategoryID = _constructionCategoryID,
                    _equipmentCategory = _equipmentCategory,
                    _equipmentSet = _equipmentSet,
                };
            }

            public void SetValue(object value)
            {
                if (value is bool)
                    m_Bool = (bool)value;
                else if (value is Int)
                    m_Int = (Int)value;
                else if (value is Float)
                    m_Float = (Float)value;
                else if (value is FloatRange)
                    m_FloatRange = (FloatRange)value;
                else if (value is IntRange)
                    m_IntRange = (IntRange)value;
                else if (value is RandomFloat)
                    m_RandomFloat = (RandomFloat)value;
                else if (value is RandomInt)
                    m_RandomInt = (RandomInt)value;
                else if (value is string)
                    m_String = (string)value;
                else if (value is GameObject)
                    m_Prefab = (GameObject)value;
                else if (value is AudioID)
                    _audioID = (AudioID)value;
                else if (value is ItemCategoryID)
                    _itemCategoryID = (ItemCategoryID)value;
                else if (value is AbilityID)
                    _abilityID = (AbilityID)value;
                else if (value is PlayerWeaponID)
                    _playerWeaponID = (PlayerWeaponID)value;
                else if (value is PrefabID)
                    _prefabID = (PrefabID)value;
                else if (value is EquipmentCategory)
                    _equipmentCategory = (EquipmentCategory)value;
                else if (value is EquipmentSet)
                    _equipmentSet = (EquipmentSet)value;
                else
                    throw new Exception();

                Changed.Send(this);
            }

            public object GetValue(Type type)
            {
                if (type == Type.Bool)
                    return m_Bool;
                else if (type == Type.Int)
                    return m_Int;
                else if (type == Type.Float)
                    return m_Float;
                else if (type == Type.FloatRange)
                    return m_FloatRange;
                else if (type == Type.IntRange)
                    return m_IntRange;
                else if (type == Type.RandomFloat)
                    return m_RandomFloat;
                else if (type == Type.RandomInt)
                    return m_RandomInt;
                else if (type == Type.String)
                    return m_String;
                else if (type == Type.Prefab)
                    return m_Prefab;
                else if (type == Type.AudioID)
                    return _audioID;
                else if (type == Type.ItemCategoryID)
                    return _itemCategoryID;
                else if (type == Type.AbilityID)
                    return _abilityID;
                else if (type == Type.PlayerWeaponID)
                    return _playerWeaponID;
                else if (type == Type.PrefabID)
                    return _prefabID;
                else if (type == Type.ConstructionCategoryID)
                    return _constructionCategoryID;
                else if (type == Type.EquipmentCategory)
                    return _equipmentCategory;
                else if (type == Type.EquimpentSet)
                    return _equipmentSet;

                return null;
            }

            public void SetValue(Type type, object value) => SetValue(value);

            public override string ToString() => GetValue(Type).ToString();
        }
    }
}
