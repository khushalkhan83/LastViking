using System;
using System.Collections.Generic;
using Game.Providers;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class EquipmentModel : MonoBehaviour
    {
        [Range(0f,1f)]
        [SerializeField] private float deathDurabilityRelativeDamage = 0.3f;
        [SerializeField] private EquipmentSetInfoProvider equipmentSetInfoProvider = default;

        public float DeathDurabilityRelativeDamage => deathDurabilityRelativeDamage;
        public EquipmentSetInfoProvider EquipmentSetInfoProvider => equipmentSetInfoProvider;
        public Dictionary<EquipmentSet, int> EquipmentSetItemCounts {get;set;} = new Dictionary<EquipmentSet, int>();

    }

    public enum EquipmentSet
    {
        None = 0,
        Sailor = 1,
        Doctor = 2,
    }

    [Serializable]
    public class EquipmentSetInfo
    {
        [SerializeField] private ItemProperty.Value bonusProperty = default;
        [SerializeField] private AbilityID abilityID = default;
        [SerializeField] private int fullSetCount = default;

        public ItemProperty.Value BonusProperty => bonusProperty;
        public AbilityID AbilityID => abilityID;
        public int FullSetCount => fullSetCount;
    }

}
