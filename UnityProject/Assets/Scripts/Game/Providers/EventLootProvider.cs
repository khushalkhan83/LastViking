using Core.Providers;
using Game.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Provider_eventLoot_default", menuName = "Providers/EventLoot", order = 1)]
    public class EventLootProvider : ProviderScriptable<EventLootID, ItemConfigs>
    {
        [EnumNamedArray(typeof(EventLootID))]
        [SerializeField] private ItemConfigs[] _data;

        public override ItemConfigs this[EventLootID key] => _data[((int)(object)key - 1)];
    }
}
