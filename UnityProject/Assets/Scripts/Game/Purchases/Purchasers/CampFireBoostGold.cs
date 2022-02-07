using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using Game.Models.RemoteSettings.Firebase;
using UnityEngine;

namespace Game.Purchases.Purchasers
{
    public class CampFireBoostGold : PurchaserBase, IPurchaseCoinInfo
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private CoinsModel _coinsModel;
        [SerializeField] private CampFiresModel _campFiresModel;
        [SerializeField] private FirebaseRemoteSettingsModel _firebaseRemoteSettingsModel;

#pragma warning restore 0649
        #endregion

        private int RemoteCostBySlot => FirebaseRemoteSettingsModel.GetParam<CampfireRemoteSettings>(FirebaseRemoteSettingID.Campfire).BoostGoldPerSlot;
        private int? CookableCount => CampFiresModel.ActiveCampFire?.CookableBoostableItemsCount;

        public int CoinCost => RemoteCostBySlot * Mathf.Max(1, CookableCount ?? 1);
        public CoinsModel CoinsModel => _coinsModel;
        public CampFiresModel CampFiresModel => _campFiresModel;
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
