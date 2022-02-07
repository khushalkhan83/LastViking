using System;
using Game.Models;
using UnityEngine;

namespace Game.Purchases.Purchasers
{
    public class PurchaseVideo : WatchPurchaseBase
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
                BeginWaitIsReady();
            }
        }

        public override void Cancel() => OnReadyHandler(false);

        public override void Prepere() => AdvertisementsModel.LoadVideo();

        public override bool IsCanPurchase => AdvertisementsModel.IsReadyVideo;

        private void BeginWaitIsReady() => OnReady += OnReadyHandler;
        private void EndWaitIsReady() => OnReady -= OnReadyHandler;
        private void BeginWaitResult() => AdvertisementsModel.OnResultVideo += OnResultHandler;
        private void EndWaitResult() => AdvertisementsModel.OnResultVideo -= OnResultHandler;

        //[UnityEngine]
        private void OnEnable()
        {
            AdvertisementsModel.OnChangeIsLoadedVideo += OnChangeIsLoadedVideoHandler;
        }

        //[UnityEngine]
        private void OnDisable()
        {
            AdvertisementsModel.OnChangeIsLoadedVideo -= OnChangeIsLoadedVideoHandler;
            EndWaitResult();
            EndWaitIsReady();
        }

        private void OnChangeIsLoadedVideoHandler() => OnReady?.Invoke(IsCanPurchase);

        private void OnReadyHandler(bool isReady)
        {
            EndWaitIsReady();

            if (isReady)
            {
                Watch();
            }
            else
            {
                OnResultHandler(VideoResult.Failed);
            }
        }

        private void Watch()
        {
            BeginWaitResult();
            AdvertisementsModel.ShowVideo();
        }

        private void OnResultHandler(VideoResult videoResult)
        {
            EndWaitResult();

            if (videoResult == VideoResult.Ended)
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
