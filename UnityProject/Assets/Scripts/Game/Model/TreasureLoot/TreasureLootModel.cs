using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UltimateSurvival;
using Game.Providers;

namespace Game.Models
{
    public class TreasureLootModel : MonoBehaviour
    {
        public event Func<TreasureID, List<SavableItem>> OnGetLootItems;
        public event Func<TreasureID, int, List<string>, SavableItem> OnGetCellRespinLootItem;
        public event Func<TreasureID, int, SavableItem, List<SavableItem>> OnGetSpinItemsList;
        public event Func<TreasureID, int, bool> OnIsCellSpecial;

        #region Data
#pragma warning disable 0649

        [SerializeField] TreasureLootGroupProvider _treasureLootGroupProvider;
        [SerializeField] TreasureConfigProvider _treasureConfigProvider;
        [SerializeField] EventLootProvider _eventLootProvider;
        [SerializeField] int _spinItemsCount = 6;
#pragma warning restore 0649
        #endregion

        // move provider out to Models (and use them in controllers)
        public TreasureLootGroupProvider TreasureLootGroupProvider => _treasureLootGroupProvider;
        public TreasureConfigProvider TreasureConfigProvider => _treasureConfigProvider;
        public EventLootProvider EventLootProvider => _eventLootProvider;
        public int SpinItemsCount => _spinItemsCount;

        public List<SavableItem> GetLootItems(TreasureID treasureID) {
            if (OnGetLootItems != null)
            {
                return OnGetLootItems(treasureID);
            }
            else 
            {
                return null;
            }
        }

        public SavableItem GetCellRespinLootItem(TreasureID treasureID, int cellId, List<string> exceptItemNames) {
            if (OnGetLootItems != null)
            {
                return OnGetCellRespinLootItem(treasureID, cellId, exceptItemNames);
            }
            else
            {
                return null;
            }
        }

        public List<SavableItem> GetSpinItemsList(TreasureID treasureID, int cellID, SavableItem targetItem) {
            if (OnGetSpinItemsList != null)
            {
                return OnGetSpinItemsList(treasureID, cellID, targetItem);
            }
            else {
                return null;
            }
        }

        public bool IsCellSpecial(TreasureID treasureID, int cellID) {
            if (OnIsCellSpecial != null)
            {
                return OnIsCellSpecial(treasureID, cellID);
            }
            else {
                return false;
            }
        }
    }
}
