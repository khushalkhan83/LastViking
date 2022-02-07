using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Models
{
    [Serializable]
    public class ItemConfig
    {
        public string name;
        public int countMin;
        public int countMax;
        public float dropPriority;

        public ItemConfig(ItemConfig config)
        {
            name = config.name;
            countMin = config.countMin;
            countMax = config.countMax;
            dropPriority = config.dropPriority;
        }
    }

    [Serializable]
    public class ItemConfigs {
        public List<ItemConfig> items;

        public ItemConfigs(ItemConfigs configs)
        {
            items = new List<ItemConfig>();
            foreach (var config in configs.items)
            {
                items.Add(new ItemConfig(config));
            }
        }
    }
}
