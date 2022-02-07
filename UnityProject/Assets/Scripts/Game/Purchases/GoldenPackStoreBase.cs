using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using UnityEngine;

namespace Game.Purchases.Purchasers
{
    public class GoldenPackStoreBase : PurchaserStoreBase, ICoinRewardInfo
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredInt _coinsAdd;

#pragma warning restore 0649
        #endregion

        public int CoinsAdd => _coinsAdd;
        public InventoryOperationsModel InventoryOperationsModel => ModelsSystem.Instance._inventoryOperationsModel;

        protected override void AddReward()
        {
            InventoryOperationsModel.AddItemToPlayer("coins",_coinsAdd);
        }
    }
}