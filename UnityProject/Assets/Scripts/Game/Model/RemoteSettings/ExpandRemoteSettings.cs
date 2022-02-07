using System.Collections.Generic;
using UnityEngine;

namespace Game.Models.RemoteSettings
{
    public class ExpandRemoteSettings : MonoBehaviour
    {
        private IDictionary<ExpandGroup, RemoteSettingID> Map { get; } = new Dictionary<ExpandGroup, RemoteSettingID>()
        {
            { ExpandGroup.loot_container_chest_basic, RemoteSettingID.BasicChest },
            { ExpandGroup.loot_container_chest_food, RemoteSettingID.FoodChest },
            { ExpandGroup.loot_container_militarybox, RemoteSettingID.MilitaryChest },
        };

        public RemoteSettingID Get(ExpandGroup expandGroup)
        {
            if (Map.ContainsKey(expandGroup))
            {
                return Map[expandGroup];
            }

            return RemoteSettingID.None;
        }
    }
}
