using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using UnityEngine;

namespace Game.Purchases.Purchasers
{
    public class PurchaserGoldBase : PurchaserBase, IPurchaseCoinInfo
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredInt _coinCost;
        [SerializeField] private CoinsModel _coinsModel;

#pragma warning restore 0649
        #endregion

        public int CoinCost => _coinCost;
        public CoinsModel CoinsModel => _coinsModel;

        public override void Purchase(PurchaseCallback callback)
        {
            if (IsCanPurchase)
            {
                CoinsModel.Adjust(-CoinCost);
                callback(PurchaseResult.Successfully);
            }
            else
            {
                callback(PurchaseResult.Failure);
            }
        }

        public override bool IsCanPurchase => CoinsModel.Coins >= CoinCost;

    }
}
