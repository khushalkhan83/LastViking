using Core;
using Core.Controllers;
using Game.Models;
using Game.Purchases;
using Game.Views;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{

    // [NEED TO DISABLE]

    public class AndroidTimingAdsController : MonoBehaviour, IAndroidTimingAdsController, IController
    {
        [Inject] public AndroidTimingAdsModel AndroidTimingAdsModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public PurchaseComplitedModel PurchaseComplitedModel { get; private set; }

        private IWatchPurchase WatchPurchase3MinInteract => PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.FirstInteractionVideoInter);

        void IController.Enable()
        {

        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {

        }

        private IEnumerator WaitForCanShowAds()
        {
            yield return new WaitForSecondsRealtime(AndroidTimingAdsModel.WaitTime - AndroidTimingAdsModel.LoadAdsGapTime);
            WatchPurchase3MinInteract.Prepere();

            yield return new WaitForSecondsRealtime(AndroidTimingAdsModel.LoadAdsGapTime);
            ViewsSystem.OnEndHide.AddListener(ViewConfigID.InventoryPlayer, OnCloseInventory);
            ViewsSystem.OnEndHide.AddListener(ViewConfigID.Craft, OnCloseInventory);
            AndroidTimingAdsModel.CanShow = true;
        }

        private void OnCloseInventory()
        {
            if (AndroidTimingAdsModel.CanShow)
            {
                PurchasesModel.Purchase(PurchaseID.FirstInteractionVideoInter, OnShowAds);
            }
        }

        private void OnShowAds(PurchaseResult purchaseResult)
        {
            AndroidTimingAdsModel.ShowAds();
            ViewsSystem.OnEndHide.RemoveListener(ViewConfigID.InventoryPlayer, OnCloseInventory);
            ViewsSystem.OnEndHide.RemoveListener(ViewConfigID.Craft, OnCloseInventory);
        }
    }
}
