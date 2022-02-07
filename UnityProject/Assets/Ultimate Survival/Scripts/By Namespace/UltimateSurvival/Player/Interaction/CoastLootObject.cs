using Game.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
    public class CoastLootObject : TreasureLootObject
    {
        public CoastLootChestModel CoastLootChestModel => ModelsSystem.Instance._coastLootChestModel;

        public override float RespawnSec => CoastLootChestModel.DurationRespawnSec;
        public override List<SavableItem> LootItems
        {
            get { return CoastLootChestModel.LootItems; }
            set { CoastLootChestModel.LootItems = value; }
        }
        public override bool DoubleUsed
        {
            get { return CoastLootChestModel.DoubleUsed; }
            set { CoastLootChestModel.DoubleUsed = value; }
        }
        public override bool Spined
        {
            get { return CoastLootChestModel.Spined; }
            set { CoastLootChestModel.Spined = value; }
        }
        public override long TimeSpawnTicks
        {
            get { return CoastLootChestModel.TimeSpawnTicks; }
            set { CoastLootChestModel.TimeSpawnTicks = value; }
        }

        public override void AddLootItem(SavableItem item)
        {
            CoastLootChestModel.AddLootItem(item);
        }

        public override void RemoveLootItem(SavableItem item)
        {
            CoastLootChestModel.RemoveLootItem(item);
        }

        public override void ReplaceLootItem(int index, SavableItem newItem)
        {
            CoastLootChestModel.ReplaceLootItem(index, newItem);
        }

        public override void ClearLootItems()
        {
            CoastLootChestModel.ClearLootItems();
        }

        protected override void InitializeData()
        {
        }
    }
}
