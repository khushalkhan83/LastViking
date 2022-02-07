using System.Collections;
using System.Collections.Generic;
using Core.Providers;
using Game.Models;
using UnityEngine;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Providers_EquipmentSetInfo", menuName = "Providers/EquipmentSetInf", order = 0)]
    public class EquipmentSetInfoProvider : ProviderScriptable<EquipmentSet, EquipmentSetInfo>
    {
        [EnumNamedArray(typeof(EquipmentSet))] 
        [SerializeField] private EquipmentSetInfo[] _data;

        public override EquipmentSetInfo this[EquipmentSet key] => _data[((int)(object)key - 1)];
    }
}