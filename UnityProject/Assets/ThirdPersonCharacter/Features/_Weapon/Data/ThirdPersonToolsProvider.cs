using Core.Providers;
using Game.Models;
using NaughtyAttributes;
using UnityEngine;
using UltimateSurvival;
using System.Collections.Generic;
using System.Linq;

namespace Game.ThirdPerson.Weapon.Data
{   
    [CreateAssetMenu(fileName = "SO_Providers_thirdPersonTools_new", menuName = "Providers/ThirdPersonTools", order = 0)]
    public class ThirdPersonToolsProvider : ProviderScriptable<PlayerWeaponID, ToolData>
    {
        [EnumNamedArray(typeof(PlayerWeaponID))]

        [SerializeField] private ToolData[] _data;

        public override ToolData this[PlayerWeaponID key] => _data[((int)(object)key - 1)];

        // [SerializeField] private ToolData defaultData;
        // [Button] void SetDefaultValues()
        // {
        //     for (int i = 0; i < _data.Length; i++)
        //     {
        //         _data[i].attackSpeed = defaultData.AttackSpeed;
        //         _data[i].damage = defaultData.damage;
        //         _data[i].extractionSettings = defaultData.extractionSettings;
        //     }
        // }
        // [Button] void SetDefaultPrefabs()
        // {
        //     for (int i = 0; i < _data.Length; i++)
        //     {
        //         _data[i].prefab = defaultData.prefab;
        //     }
        // }
    }

    [System.Serializable]
    public class ToolData
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private float attackSpeed = 1;
        [SerializeField] private float cooldown = 0.5f;
        [SerializeField] private float hitZoneRadius = 1;
        [SerializeField] private float hitZoneLength = 3.5f;
        [SerializeField] private float damage;
        [SerializeField] private ExtractionSetting[] extractionSettings;
        [SerializeField] private bool rightHand = true;
        [SerializeField] private bool isRanged = false;

        public GameObject Prefab => prefab;
        public float AttackSpeed => attackSpeed;
        public float Cooldown => cooldown;
        public float HitZoneRadius => hitZoneRadius;
        public float HitZoneLength => hitZoneLength;
        public float Damage => damage;
        public List<ExtractionSetting> ExtractionSettings => extractionSettings.ToList();
        public bool RightHand => rightHand;
        public bool IsRanged => isRanged;
    }
}