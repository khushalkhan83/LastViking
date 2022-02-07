using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using UnityEngine;

namespace Game.Purchases.Purchasers
{
    public class CoinRewardInfoBase : PurchaserBase, ICoinRewardInfo
    {
        private const string k_CoinsItemName = "coins";

        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredInt _coinsAdd;

#pragma warning restore 0649
        #endregion

        public int CoinsAdd => _coinsAdd;
        public InventoryOperationsModel InventoryOperationsModel => ModelsSystem.Instance._inventoryOperationsModel;

        public override void Purchase(PurchaseCallback callback)
        {
            if (IsCanPurchase)
            {
                InventoryOperationsModel.AddItemToPlayer(k_CoinsItemName, _coinsAdd);
                callback?.Invoke(PurchaseResult.Successfully);
            }
            else
            {
                callback?.Invoke(PurchaseResult.Failure);
            }
        }

        public override bool IsCanPurchase => true;
    }
}
