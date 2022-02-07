using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using Game.Models.RemoteSettings.Firebase;
using UnityEngine;

namespace Game.Purchases.Purchasers
{
    public class CraftBoostGold : PurchaserBase, IPurchaseCoinInfo
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private CoinsModel _coinsModel;
        [SerializeField] private CraftModel _craftModel;
        [SerializeField] private FirebaseRemoteSettingsModel _firebaseRemoteSettingsModel;

#pragma warning restore 0649
        #endregion

        private int RemoteCostBySlot => FirebaseRemoteSettingsModel.GetParam<CraftRemoteSettings>(FirebaseRemoteSettingID.Craft).BoostGoldPerSlot;

        public int CoinCost => RemoteCostBySlot * CraftModel.Craft.Count;
        public CoinsModel CoinsModel => _coinsModel;
        public CraftModel CraftModel => _craftModel;
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
