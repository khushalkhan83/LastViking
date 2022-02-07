using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Models.RemoteSettings;
using Game.Purchases;
using Game.Purchases.Purchasers;
using Game.Views;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UltimateSurvival;
using UnityEngine;
using Game.Providers;

namespace Game.Controllers
{
    public class TreasureHuntLootPopupViewController : TreasureLootPopupViewController
    {
        [Inject] public TreasureHuntLootChestModel TreasureHuntLootChestModel { get; set; }
        [Inject] public TreasureHuntModel TreasureHuntModel { get; set; }
        [Inject(true)] public TreasureHuntPlacesProvider TreasureHuntPlaces { get; private set; }

        public override float RespawnSec => 0;
        public override bool OpenLootBoxAfterDrop => false;
        public override TreasureID TreasureID => TreasureHuntLootChestModel.TreasureID;

        protected override List<SavableItem> LootItems
        {
            get { return TreasureHuntLootChestModel.LootItems; }
            set { TreasureHuntLootChestModel.LootItems = value; }
        }
        protected override bool DoubleUsed
        {
            get { return false; }
            set { }
        }
        protected override bool Spined
        {
            get { return false; }
            set { }
        }
        protected override long TimeSpawnTicks
        {
            get { return 0; }
            set { }
        }

        protected override void AddLootItem(SavableItem item) => TreasureHuntLootChestModel.AddLootItem(item);

        protected override void RemoveLootItem(SavableItem item) => TreasureHuntLootChestModel.RemoveLootItem(item);

        protected override void ReplaceLootItem(int index, SavableItem newItem) => TreasureHuntLootChestModel.ReplaceLootItem(index, newItem);

        protected override void ClearLootItems() => TreasureHuntLootChestModel.ClearLootItems();

        protected override void InitializeLootItmes() {
            GenerateItems();
        }

        protected override void OnChestOpen()
        {
            int emptyDiggetCount = 0;
            for (int i = 0; i < TreasureHuntPlaces.Count; i++)
            {
                if (TreasureHuntModel.IsHoleUsed(i))
                {
                    emptyDiggetCount++; 
                }
            }
            emptyDiggetCount--;

            TreasureHuntModel.OnChestOpen(emptyDiggetCount);
        }
        protected override void GenerateItems() 
        {
            ClearLootItems();
            var items = TreasureLootModel.GetLootItems(TreasureID);
            foreach (var item in items)
            {
                AddLootItem(item);
            }
        }
    }
}