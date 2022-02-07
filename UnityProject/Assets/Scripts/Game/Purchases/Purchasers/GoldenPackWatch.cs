using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using UnityEngine;

namespace Game.Purchases.Purchasers
{
    public class GoldenPackWatch : WatchPurchaseBase, ICoinRewardInfo
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredInt _coinsAdd;
        [SerializeField] private CoinsModel _coinsModel;
        [SerializeField] private WatchPurchaseBase _watchPurchaseBase;

#pragma warning restore 0649
        #endregion

        public int CoinsAdd => _coinsAdd;
        public CoinsModel CoinsModel => _coinsModel;
        public WatchPurchaseBase WatchPurchase => _watchPurchaseBase;

        private ActionsLogModel ActionsLogModel => ModelsSystem.Instance._actionsLogModel;

        public override event Action<bool> OnReady;

        public override bool IsCanPurchase => WatchPurchase.IsCanPurchase;

        protected void Callback(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                CoinsModel.Adjust(CoinsAdd);

                ActionsLogModel.SendMessage(new MessageAppendCoinData(CoinsAdd));
            }
        }

        public override void Cancel() => WatchPurchase.Cancel();
        public override void Prepere() => WatchPurchase.Prepere();
        public override void Purchase(PurchaseCallback callback) => WatchPurchase.Purchase(callback + Callback);
    }
}
