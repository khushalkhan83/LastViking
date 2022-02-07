using Core.Providers;
using Game.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Provider_treasureLootGroup_default", menuName = "Providers/TreasureLootGroup", order = 1)]
    public class TreasureLootGroupProvider : ProviderScriptable<LootGroupID, ItemConfigs>
    {
        [EnumNamedArray(typeof(LootGroupID))]
        [SerializeField] private ItemConfigs[] _data;

        public override ItemConfigs this[LootGroupID key] => new ItemConfigs(_data[((int)(object)key - 1)]);
    }
}
