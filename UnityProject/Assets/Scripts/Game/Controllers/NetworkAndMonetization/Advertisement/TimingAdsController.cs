using Core;
using Core.Controllers;
using Game.Models;
using Game.Purchases;
using Game.Views;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class TimingAdsController : ITimingAdsController, IController
    {
        [Inject] public TimingAdsModel TimingAdsModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public PurchaseComplitedModel PurchaseComplitedModel { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }

        private IWatchPurchase WatchPurchase3MinInteract => PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.Min3thVideoInter);

        private int _waitForCanShowAds = -1;

        void IController.Enable()
        {
            if(TimingAdsModel.HasShown) return;

            MainLogic();
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            ViewsSystem.OnEndHide.RemoveListener(ViewConfigID.InventoryPlayer, OnCloseInventory);
            ViewsSystem.OnEndHide.RemoveListener(ViewConfigID.Craft, OnCloseInventory);

            CoroutineModel.BreakeCoroutine(_waitForCanShowAds);
        }

        private void MainLogic()
        {
            _waitForCanShowAds = CoroutineModel.InitCoroutine(WaitForCanShowAds());
        }

        private IEnumerator WaitForCanShowAds()
        {
            var watchPurchase3MinInteract = WatchPurchase3MinInteract;
            yield return new WaitForSecondsRealtime(TimingAdsModel.WaitTime - TimingAdsModel.LoadAdsGapTime);
            watchPurchase3MinInteract.Prepere();

            yield return new WaitForSecondsRealtime(TimingAdsModel.LoadAdsGapTime);

            ViewsSystem.OnEndHide.AddListener(ViewConfigID.InventoryPlayer, OnCloseInventory);
            ViewsSystem.OnEndHide.AddListener(ViewConfigID.Craft, OnCloseInventory);
            TimingAdsModel.SetCanShow(true);
        }

        private void OnCloseInventory()
        {
            if (TimingAdsModel.IsCanShow && TutorialModel.IsComplete)
            {
                PurchasesModel.Purchase(PurchaseID.Min3thVideoInter, OnShowAds);
            }
        }

        private void OnShowAds(PurchaseResult purchaseResult)
        {
            TimingAdsModel.ShowAds();
            ViewsSystem.OnEndHide.RemoveListener(ViewConfigID.InventoryPlayer, OnCloseInventory);
            ViewsSystem.OnEndHide.RemoveListener(ViewConfigID.Craft, OnCloseInventory);
        }
    }
}
