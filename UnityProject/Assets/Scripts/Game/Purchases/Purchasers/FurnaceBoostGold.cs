using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using Game.Models.RemoteSettings.Firebase;
using UnityEngine;

namespace Game.Purchases.Purchasers
{
    public class FurnaceBoostGold : PurchaserBase, IPurchaseCoinInfo
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private CoinsModel _coinsModel;
        [SerializeField] private FurnaceGroupModel _furnacesModel;
        [SerializeField] private FirebaseRemoteSettingsModel _firebaseRemoteSettingsModel;

#pragma warning restore 0649
        #endregion

        private int RemoteCostBySlot => FirebaseRemoteSettingsModel.GetParam<FurnaceRemoteSettings>(FirebaseRemoteSettingID.Furnace).BoostGoldPerSlot;
        private int? MeltableCount => FurnacesModel.ActiveFurnace?.MeltableItemsCount;

        public int CoinCost => RemoteCostBySlot * Mathf.Max(1, MeltableCount ?? 1);
        public CoinsModel CoinsModel => _coinsModel;
        public FurnaceGroupModel FurnacesModel => _furnacesModel;
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
