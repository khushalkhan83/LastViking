using System;
using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Purchases;
using Game.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Controllers
{
    public class PurchasesViewController : ViewControllerBase<PurchasesView>
    {
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public CoinsModel CoinsModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public PurchasesViewModel PurchasesViewModel { get; private set; }
        [Inject] public StarterPackModel StarterPackModel { get; private set; }
        [Inject] public PurchasersStoreModel PurchasersStoreModel { get; private set; }
        [Inject] public EventSystemModel EventSystemModel { get; private set; }
        [Inject] public InputModel InputModel { get; private set; }

        private StarterPackView starterPackView;

        private DisablableButtonView selectedButton;

        private bool IsShowPurchaseWatch => GetIsShowPurchaseWatch(WatchPurchaseWatch);
        private bool GetIsShowPurchaseWatch(IWatchPurchase purchase) => PurchasesViewModel.IsHasLimitWatch && (purchase.IsCanPurchase || NetworkModel.IsHasConnection);

        private bool TryGetIsShowPurchaseWatch(out IWatchPurchase watchPurchase)
        {
            watchPurchase = WatchPurchaseWatch;
            return GetIsShowPurchaseWatch(watchPurchase);
        }

        private IWatchPurchase WatchPurchaseWatch => PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.GoldenPackWatch);

        protected override void Show()
        {
            AudioSystem.PlayOnce(AudioID.WindowOpen);

            EventSystemModel.OnSelectionChanged += OnSelectionChanged;
            // EventSystemModel.SetSelectedGameObject(View.GoldenPackTier01Button.transform.parent.GetComponent<Selectable>().gameObject);
            EventSystemModel.SetSelectedGameObject(null);

            InputModel.OnInput.AddListener(PlayerActions.Interact,OnTryPurchaseWithButton);
            InputModel.OnInput.AddListener(PlayerActions.InteractAlternative,OnCloseHandler);

            NetworkModel.OnInternetConnectionStateChange += HandleInternetConnection;
            PurchasersStoreModel.Initialized += HandlePurchasersStoreModelInitialized;
            NetworkModel.UpdateInternetConnectionStatus();

            LocalizationModel.OnChangeLanguage += SetLocalization;

            CoinsModel.OnChange += OnChangeCoinsHandler;

            PurchasesViewModel.OnChangeCountWatch += OnChangeCountWatchHandler;

            View.OnClose += OnCloseHandler;

            View.GoldenPackTier01Button.SetText(PurchasesModel.GetInfo<IPurchaseStoreInfo>(PurchaseID.GoldenPackTier01).PriceString);
            View.GoldenPackTier02Button.SetText(PurchasesModel.GetInfo<IPurchaseStoreInfo>(PurchaseID.GoldenPackTier02).PriceString);
            View.GoldenPackTier03Button.SetText(PurchasesModel.GetInfo<IPurchaseStoreInfo>(PurchaseID.GoldenPackTier03).PriceString);
            View.GoldenPackTier04Button.SetText(PurchasesModel.GetInfo<IPurchaseStoreInfo>(PurchaseID.GoldenPackTier04).PriceString);
            View.GoldenPackTier05Button.SetText(PurchasesModel.GetInfo<IPurchaseStoreInfo>(PurchaseID.GoldenPackTier05).PriceString);
            View.GoldenPackTier06Button.SetText(PurchasesModel.GetInfo<IPurchaseStoreInfo>(PurchaseID.GoldenPackTier06).PriceString);

            ShowPurchaseButtons();
            ShowPurchaseWatchButton();
            ShowStarterPackView();

            CoinsUpdate();

            SetLocalization();
        }

        private void ShowStarterPackView()
        {
            // View.StarterPackHolder.gameObject.SetActive(false);

            // if (!ShowCondition()) return;

            // View.StarterPackHolder.gameObject.SetActive(true);

            // starterPackView = ViewsSystem.Show<StarterPackView>(ViewConfigID.StarterPackInsidePurchases, View.StarterPackHolder);

            // SubscribeStarterPack();

            // bool ShowCondition() => StarterPackModel.IsPackAvailable;
        }

        private void HideStarterPackView()
        {
            // View.StarterPackHolder.gameObject.SetActive(false);

            // if (starterPackView == null) return;
            // UnsubscribeStarterPack();
            // ViewsSystem.Hide(starterPackView);
            // starterPackView = null;
        }

        private void SubscribeStarterPack()
        {
            // StarterPackModel.OnOfferEnded += HideStarterPackView;
            // if (NetworkModel.IsHasConnection)
            // {
            //     SubscribeStarterPackButtons();
            // }
        }

        private void UnsubscribeStarterPack()
        {
            // StarterPackModel.OnOfferEnded -= HideStarterPackView;
            // if (starterPackView == null) return;
            // UnsubscribeStarterPackButtons();
        }

        private void SubscribeStarterPackButtons()
        {
            // starterPackView.OnBuyDominant += HideMainView;
            // starterPackView.OnBuySurvival += HideMainView;
        }

        private void UnsubscribeStarterPackButtons()
        {
            // starterPackView.OnBuyDominant -= HideMainView;
            // starterPackView.OnBuySurvival -= HideMainView;
        }

        private void UpdateStarerPackButtons()
        {
            // if (starterPackView == null) return;
            // UnsubscribeStarterPackButtons();
            // if (NetworkModel.IsHasConnection && StarterPackModel.IsPackAvailable)
            // {
            //     SubscribeStarterPackButtons();
            // }
        }

        private void HideMainView() => View.ActionClose();

        protected override void Hide()
        {
            EventSystemModel.OnSelectionChanged -= OnSelectionChanged;

            InputModel.OnInput.RemoveListener(PlayerActions.Interact,OnTryPurchaseWithButton);
            InputModel.OnInput.RemoveListener(PlayerActions.InteractAlternative,OnCloseHandler);

            NetworkModel.OnInternetConnectionStateChange -= HandleInternetConnection;
            PurchasersStoreModel.Initialized -= HandlePurchasersStoreModelInitialized;

            LocalizationModel.OnChangeLanguage -= SetLocalization;

            CoinsModel.OnChange -= OnChangeCoinsHandler;
            PurchasesViewModel.OnChangeCountWatch -= OnChangeCountWatchHandler;

            View.OnClose -= OnCloseHandler;

            HidePurchaseGoldButtons();
            HidePurchaseWatchButton();
            HideStarterPackView();
        }

        private void OnTryPurchaseWithButton()
        {
            if(selectedButton == View.GoldenPackTier01Button)
            {
                OnPurchaseGoldenPackTier01Handler();
            }
            if(selectedButton == View.GoldenPackTier02Button)
            {
                OnPurchaseGoldenPackTier02Handler();
            }
            if(selectedButton == View.GoldenPackTier03Button)
            {
                OnPurchaseGoldenPackTier03Handler();
            }
            if(selectedButton == View.GoldenPackTier04Button)
            {
                OnPurchaseGoldenPackTier04Handler();
            }
            if(selectedButton == View.GoldenPackTier05Button)
            {
                OnPurchaseGoldenPackTier05Handler();
            }
            if(selectedButton == View.GoldenPackTier06Button)
            {
                OnPurchaseGoldenPackTier06Handler();
            }
            if(selectedButton == View.PurchaseWatchButton)
            {
                OnPurchaseWatchHandler();
            }
        }

        
        private void OnSelectionChanged(GameObject newSelection, GameObject oldSelection)
        {
            selectedButton = newSelection?.GetComponentInChildren<DisablableButtonView>();
        }

        private void OnChangeCountWatchHandler() => UpdatePurchaseWatchButton();

        private void OnPurchaseGoldenPackTier06Handler() => PurchaseHandler(PurchaseID.GoldenPackTier06,OnResultPurchaseHandler);
        private void OnPurchaseGoldenPackTier05Handler() => PurchaseHandler(PurchaseID.GoldenPackTier05,OnResultPurchaseHandler);
        private void OnPurchaseGoldenPackTier04Handler() => PurchaseHandler(PurchaseID.GoldenPackTier04,OnResultPurchaseHandler);
        private void OnPurchaseGoldenPackTier03Handler() => PurchaseHandler(PurchaseID.GoldenPackTier03,OnResultPurchaseHandler);
        private void OnPurchaseGoldenPackTier02Handler() => PurchaseHandler(PurchaseID.GoldenPackTier02,OnResultPurchaseHandler);
        private void OnPurchaseGoldenPackTier01Handler() => PurchaseHandler(PurchaseID.GoldenPackTier01,OnResultPurchaseHandler);
     

        private void OnPurchaseWatchHandler() => PurchaseHandler(PurchaseID.GoldenPackWatch,OnResultPurchaseWatchHandler);

        private void PurchaseHandler(PurchaseID purchaseID, PurchaseCallback callback)
        {
            AudioSystem.PlayOnce(AudioID.Button);
            UnsubscribeAll();
            PurchasesModel.Purchase(purchaseID, callback);
        }

        private void OnResultPurchaseWatchHandler(PurchaseResult purchaseResult)
        {
            PurchasesViewModel.Watching();
            UpdateAllPurchaseButtons();
        }

        private void OnResultPurchaseHandler(PurchaseResult purchaseResult) => UpdateAllPurchaseButtons();

        private void OnChangeCoinsHandler() => CoinsUpdate();

        private void OnCloseHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            HideStarterPackView();
            ViewsSystem.Hide(View);
        }

        private void CoinsUpdate() => View.SetTextCoinsValue(CoinsModel.Coins.ToString());

        private void SetLocalization()
        {
            View.SetCostMobile(LocalizationModel.GetString(LocalizationKeyID.GameStore_WatchBtn));
        }

        private void HandleInternetConnection()
        {
            UpdateAllPurchaseButtons();
            UpdateStarerPackButtons();
        }

        private void HandlePurchasersStoreModelInitialized()
        {
            View.GoldenPackTier01Button.SetText(PurchasesModel.GetInfo<IPurchaseStoreInfo>(PurchaseID.GoldenPackTier01).PriceString);
            View.GoldenPackTier02Button.SetText(PurchasesModel.GetInfo<IPurchaseStoreInfo>(PurchaseID.GoldenPackTier02).PriceString);
            View.GoldenPackTier03Button.SetText(PurchasesModel.GetInfo<IPurchaseStoreInfo>(PurchaseID.GoldenPackTier03).PriceString);
            View.GoldenPackTier04Button.SetText(PurchasesModel.GetInfo<IPurchaseStoreInfo>(PurchaseID.GoldenPackTier04).PriceString);
            View.GoldenPackTier05Button.SetText(PurchasesModel.GetInfo<IPurchaseStoreInfo>(PurchaseID.GoldenPackTier05).PriceString);
            View.GoldenPackTier06Button.SetText(PurchasesModel.GetInfo<IPurchaseStoreInfo>(PurchaseID.GoldenPackTier06).PriceString);
            UpdatePurchasesButton();
            UpdateStarerPackButtons();
        }

        private void UpdateAllPurchaseButtons()
        {
            UpdatePurchasesButton();
            UpdatePurchaseWatchButton();
        }

        private void UpdatePurchasesButton()
        {
            HidePurchaseGoldButtons();
            ShowPurchaseButtons();
        }
        
        private void ShowPurchaseButtons()
        {
            bool connectedAndReady = NetworkModel.IsHasConnection && PurchasersStoreModel.IsReady;
            if (connectedAndReady)
            {
                SubscribeGoldPurchases();
            }

            View.PurchaseWatchButton.SetIsVisible(connectedAndReady);
            View.GoldenPackTier01Button.SetIsVisible(connectedAndReady);
            View.GoldenPackTier02Button.SetIsVisible(connectedAndReady);
            View.GoldenPackTier03Button.SetIsVisible(connectedAndReady);
            View.GoldenPackTier04Button.SetIsVisible(connectedAndReady);
            View.GoldenPackTier05Button.SetIsVisible(connectedAndReady);
            View.GoldenPackTier06Button.SetIsVisible(connectedAndReady);
        }

        private void HidePurchaseGoldButtons() => UnsubscribeGoldPurchases();

        private void UnsubscribeAll()
        {
            UnsubscribePurchaseWatchButton();
            UnsubscribeGoldPurchases();
            UnsubscribeStarterPack(); // add it here ?
        }

        private void UnsubscribeGoldPurchases()
        {
            View.OnPurchaseGoldenPackTier01 -= OnPurchaseGoldenPackTier01Handler;
            View.OnPurchaseGoldenPackTier02 -= OnPurchaseGoldenPackTier02Handler;
            View.OnPurchaseGoldenPackTier03 -= OnPurchaseGoldenPackTier03Handler;
            View.OnPurchaseGoldenPackTier04 -= OnPurchaseGoldenPackTier04Handler;
            View.OnPurchaseGoldenPackTier05 -= OnPurchaseGoldenPackTier05Handler;
            View.OnPurchaseGoldenPackTier06 -= OnPurchaseGoldenPackTier06Handler;
        }

        private void SubscribeGoldPurchases()
        {
            View.OnPurchaseGoldenPackTier01 += OnPurchaseGoldenPackTier01Handler;
            View.OnPurchaseGoldenPackTier02 += OnPurchaseGoldenPackTier02Handler;
            View.OnPurchaseGoldenPackTier03 += OnPurchaseGoldenPackTier03Handler;
            View.OnPurchaseGoldenPackTier04 += OnPurchaseGoldenPackTier04Handler;
            View.OnPurchaseGoldenPackTier05 += OnPurchaseGoldenPackTier05Handler;
            View.OnPurchaseGoldenPackTier06 += OnPurchaseGoldenPackTier06Handler;
        }

        private void UpdatePurchaseWatchButton()
        {
            HidePurchaseWatchButton();
            ShowPurchaseWatchButton();
        }

        private void ShowPurchaseWatchButton()
        {
            if (TryGetIsShowPurchaseWatch(out var watchPurchaseWatch))
            {
                watchPurchaseWatch.Prepere();
                View.PurchaseWatchButton.SetIsVisible(true);
                View.SetVisiblePurchaseWatch(true);
                SubscribePurchaseWatchButton();
            }
        }

        private void HidePurchaseWatchButton()
        {
            UnsubscribePurchaseWatchButton();
            View.SetVisiblePurchaseWatch(false);
            View.PurchaseWatchButton.SetIsVisible(false);
        }

        private void SubscribePurchaseWatchButton() => View.OnPurchaseWatch += OnPurchaseWatchHandler;

        private void UnsubscribePurchaseWatchButton() => View.OnPurchaseWatch -= OnPurchaseWatchHandler;
    }
}
