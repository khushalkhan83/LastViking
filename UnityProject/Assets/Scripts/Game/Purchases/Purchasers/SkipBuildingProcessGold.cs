using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using UnityEngine;

namespace Game.Purchases.Purchasers
{
    public class SkipBuildingProcessGold : PurchaserBase, IPurchaseCoinInfo
    {
        [SerializeField] private ObscuredInt _coinCost = default;

        public int CoinCost => _coinCost;
        public CoinsModel CoinsModel => ModelsSystem.Instance._coinsModel;

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
