using Core.Providers;
using Game.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Provider_treasureConfig_default", menuName = "Providers/TreasureConfig", order = 1)]
    public class TreasureConfigProvider : ProviderScriptable<TreasureID, TreasureChestConfig>
    {
        [EnumNamedArray(typeof(TreasureID))]
        [SerializeField] private TreasureChestConfig[] _data;

        public override TreasureChestConfig this[TreasureID key] => _data[((int)(object)key - 1)];
    }
}
