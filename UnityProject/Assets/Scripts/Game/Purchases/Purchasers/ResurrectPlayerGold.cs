using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using Game.Models.RemoteSettings.Firebase;
using UnityEngine;

namespace Game.Purchases.Purchasers
{
    public class ResurrectPlayerGold : PurchaserBase, IPurchaseCoinInfo
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private CoinsModel _coinsModel;
        [SerializeField] private FirebaseRemoteSettingsModel _firebaseRemoteSettingsModel;

#pragma warning restore 0649
        #endregion

        public int CoinCost => FirebaseRemoteSettingsModel.GetParam<ResurrectRemoteSettings>(FirebaseRemoteSettingID.Resurrect).ResurrectGold;
        public CoinsModel CoinsModel => _coinsModel;
        public FirebaseRemoteSettingsModel FirebaseRemoteSettingsModel => _firebaseRemoteSettingsModel;

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
