using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Purchases;
using Game.Views;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class RateMeViewController : ViewControllerBase<RateMeView>
    {
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public RateMeViewModel RateMeViewModel { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }

        protected override void Show()
        {
            AudioSystem.PlayOnce(AudioID.WindowOpen);

            View.OnClose += OnCloseHandler;
            View.OnRatePressed += OnRatePressedHandler;

            LocalizationModel.OnChangeLanguage += SetLocalization;
            SetLocalization();
        }

        protected override void Hide()
        {
            View.OnClose -= OnCloseHandler;
            View.OnRatePressed -= OnRatePressedHandler;

            LocalizationModel.OnChangeLanguage -= SetLocalization;
        }

        public void OnRatePressedHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            RateMeViewModel.AlwaysHide();
            StartCoroutine(CollectCoin());

            Application.OpenURL(RateMeViewModel.URL);
        }

        private IEnumerator CollectCoin()
        {
            yield return new WaitForSecondsRealtime(RateMeViewModel.DelayBeforeGetCoins);
            PurchasesModel.Purchase(PurchaseID.RateGameGiftGold, OnPurchaseHandler);
            Close();
        }

        private void OnPurchaseHandler(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                AudioSystem.PlayOnce(AudioID.PickUpCoin);
            }
        }

        public void OnCloseHandler() => Close();

        private void Close()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            ViewsSystem.Hide(View);
        }

        private void SetLocalization()
        {
            View.SetTextTitle(LocalizationModel.GetString(LocalizationKeyID.Rate_PopUp_Description));
            View.SetTextRateButton(LocalizationModel.GetString(LocalizationKeyID.Rate_PopUp_RateBtn));
        }
    }
}

