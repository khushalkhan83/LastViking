using System;
using Game.Models;
using UnityEngine;

namespace Game.Purchases.Purchasers
{
    public class PurchaseRewardedVideo : WatchPurchaseBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private AdvertisementsModel _advertisementsModel;

#pragma warning restore 0649
        #endregion

        public AdvertisementsModel AdvertisementsModel => _advertisementsModel;

        private PurchaseCallback __purchaseCallback;

        public override event Action<bool> OnReady;

        public override void Purchase(PurchaseCallback callback)
        {
            __purchaseCallback = callback;

            if (IsCanPurchase)
            {
                Watch();
            }
            else
            {
                OnReady += OnReadyHandler;
            }
        }

        public override void Cancel() => OnReadyHandler(false);

        public override void Prepere() => AdvertisementsModel.LoadRewardedVideo();

        public override bool IsCanPurchase => AdvertisementsModel.IsReadyRewardedVideo;

        //[UnityEngine]
        private void OnEnable()
        {
            AdvertisementsModel.OnChangeIsLoadedRewardedVideo += OnChangeIsLoadedHandler;
        }

        //[UnityEngine]
        private void OnDisable()
        {
            AdvertisementsModel.OnChangeIsLoadedRewardedVideo -= OnChangeIsLoadedHandler;
            AdvertisementsModel.OnResultRewardedVideo -= OnResultHandler;
            OnReady -= OnReadyHandler;
        }

        private void OnChangeIsLoadedHandler() => OnReady?.Invoke(IsCanPurchase);

        private void OnReadyHandler(bool isReady)
        {
            OnReady -= OnReadyHandler;

            if (isReady)
            {
                Watch();
            }
            else
            {
                OnResultHandler(RewardedVideoResult.Failed);
            }
        }

        private void Watch()
        {
            AdvertisementsModel.OnResultRewardedVideo += OnResultHandler;
            AdvertisementsModel.ShowRewardedVideo();
        }

        private void OnResultHandler(RewardedVideoResult rewardedVideoResult)
        {
            AdvertisementsModel.OnResultRewardedVideo -= OnResultHandler;

            if (rewardedVideoResult == RewardedVideoResult.Ended)
            {
                Callback(PurchaseResult.Successfully);
            }
            else
            {
                Callback(PurchaseResult.Failure);
            }
        }

        private void Callback(PurchaseResult purchaseResult) => __purchaseCallback?.Invoke(purchaseResult);
    }
}
