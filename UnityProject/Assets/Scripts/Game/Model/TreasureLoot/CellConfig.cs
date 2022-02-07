using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.Purchases;
using static Core.Storage.DataBase;

namespace Game.Models
{
    [Serializable]
    public class CellConfig
    {
        public SpawnType spwanType;
        public bool isSpecial;
        public List<LootGroupPriority> lootGroupPriorities;
        public List<ItemConfig> items;
        public bool canBeEvent;
        public EventLootID eventLootID;
    }

    [Serializable]
    public class TreasureChestConfig {
        public PurchaseID X2Watch;
        public PurchaseID X2Gold;
        public PurchaseID Respin;
        public List<CellConfig> cells;
    }
}

