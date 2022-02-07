using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using Game.Models.RemoteSettings.Firebase;
using UnityEngine;

namespace Game.Purchases.Purchasers
{
    public class TeleportGold : PurchaserBase, IPurchaseCoinInfo
    {
          #region Data
#pragma warning disable 0649
        [SerializeField] private ObscuredInt _coinsCost;

#pragma warning restore 0649
        #endregion

        public int CoinCost => _coinsCost;
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