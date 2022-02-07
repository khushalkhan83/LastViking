using Core;
using Core.Controllers;
using Game.Models;
using Game.Models.RemoteSettings;
using Game.Models.RemoteSettings.Firebase;
using Game.Purchases;
using Game.Views;
using UnityEngine;
using ButtonAddonID = Game.Models.PlayerHealthBarViewModel.ButtonAddonID;
using HealthAddonRemoteSettings = Game.Models.RemoteSettings.HealthAddonRemoteSettings;

namespace Game.Controllers
{
    public class HealthBarViewController : ViewControllerBase<HealthBarView>
    {
        [Inject] public PlayerBleedingDamagerModel PlayerBleedingDamagerModel { get; private set; }
        [Inject] public PlayerHealthBarViewModel PlayerHealthBarViewModel { get; private set; }
        [Inject] public RemoteSettingsModel RemoteSettingsModel { get; private set; }
        [Inject] public AdvertisementsModel AdvertisementsModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public FirebaseRemoteSettingsModel FirebaseRemoteSettingsModel { get; private set; }

        protected bool GetIsShowButtonAddonWatch(IWatchPurchase watchPurchase) => !HealthAddonRemoteSettings.IsAddonGold && (watchPurchase.IsCanPurchase || NetworkModel.IsHasConnection);

        protected bool IsShowButtonAddonWatch => GetIsShowButtonAddonWatch(WatchPurchaseHealthAddon);

        protected IWatchPurchase WatchPurchaseHealthAddon => PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.HealthAddonWatch);

        protected HealthAddonRemoteSettings HealthAddonRemoteSettings => RemoteSettingsModel.Get<HealthAddonRemoteSettings>(RemoteSettingID.HealthAddon);

        protected PurchasesView PurchasesView { get; private set; }

        protected override void Show()
        {
            PlayerHealthModel.OnChangeHealth += OnChangeHealthHandler;
            PlayerHealthModel.OnChangeAddonLevel += OnChangeAddonLevel;
            PlayerHealthModel.OnChangeEquipmentBonus += OnChangeEquipmentBonus;
            PlayerHealthBarViewModel.OnChangeAddonButtonType += OnAddonButtonTypeChangeHandler;

            View.OnAddAddon += OnAddAddonHandler;

            NetworkModel.OnInternetConnectionStateChange += HandleInternetConnection;
            NetworkModel.UpdateInternetConnectionStatus();

            FirebaseRemoteSettingsModel.OnFetchedRemote += OnFetchedRemote;

            View.SetTextBuyCoinsCount(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.HealthAddonGold).CoinCost.ToString());

            UpdateHealthLevel();
        }
        protected override void Hide()
        {
            PlayerHealthModel.OnChangeHealth -= OnChangeHealthHandler;
            PlayerHealthModel.OnChangeAddonLevel -= OnChangeAddonLevel;
            PlayerHealthModel.OnChangeEquipmentBonus -= OnChangeEquipmentBonus;
            PlayerHealthBarViewModel.OnChangeAddonButtonType -= OnAddonButtonTypeChangeHandler;

            NetworkModel.OnInternetConnectionStateChange -= HandleInternetConnection;
            FirebaseRemoteSettingsModel.OnFetchedRemote -= OnFetchedRemote;

            View.OnAddAddon -= OnAddAddonHandler;
        }

        private void OnFetchedRemote() => View.SetTextBuyCoinsCount(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.HealthAddonGold).CoinCost.ToString());

        private void HandleInternetConnection() => UpdateTypeButtonAddon();

        private void UpdateTypeButtonAddon() => PlayerHealthBarViewModel.SetButtonAddonType(GetButtonAddonID());

        private void OnChangeAddonLevel() => UpdateHealthLevel();
        private void OnChangeEquipmentBonus() => UpdateHealthLevel();
        private void OnChangeHealthHandler() => UpdateHealthLevel();

        private void UpdateHealthLevel()
        {
            var isVisibleAddonHealth = PlayerHealthModel.HealthAddon > 0;

            View.SetValueFillAmount(PlayerHealthModel.Health / PlayerHealthModel.HealthMax);

            View.SetIsVisibleValueAddon(isVisibleAddonHealth);
            if (isVisibleAddonHealth)
            {
                View.SetValueAddonFillAmount(PlayerHealthModel.HealthAddon / PlayerHealthModel.HealthBonus);
            }

            UpdateTypeButtonAddon();

            View.SetValue(((int)PlayerHealthModel.HealthCurrent).ToString());
        }

        float __pivotValue;
        private void Update()
        {
            if (View.RectTransformHealthDelta)
            {
                View.SetIsVisibleDeltaHealth(true);
                var rect = View.RectTransformHealthDelta.rect;

                var xMin = GetPositionX(PlayerHealthModel.HealthCurrent);
                var xMax = GetPositionX(__pivotValue);
                var x = xMin;
                var width = Mathf.Abs(xMax - xMin);

                if (xMin > xMax)
                {
                    x = xMax;
                    View.SetDeltaColor(View.ColorDeltaHealthAdd);
                }
                else
                {
                    View.SetDeltaColor(View.ColorDeltaHealthRemove);
                }

                View.RectTransformHealthDelta.localPosition = new Vector3(x, View.RectTransformHealthDelta.localPosition.y, View.RectTransformHealthDelta.localPosition.z);
                View.RectTransformHealthDelta.sizeDelta = new Vector3(width, View.RectTransformHealthDelta.sizeDelta.y);

                __pivotValue = Mathf.Lerp(__pivotValue, PlayerHealthModel.HealthCurrent, Time.deltaTime * View.DeltaSpeedMul);
            }
        }

        private ButtonAddonID GetButtonAddonID()
        {
            if (PlayerHealthModel.HealthCurrent < PlayerHealthBarViewModel.HealthToShowButtonAddon)
            {
                if (IsShowButtonAddonWatch)
                {
                    return ButtonAddonID.WatchVideo;
                }

                return ButtonAddonID.Buy;
            }

            return ButtonAddonID.None;
        }

        private void OnAddonButtonTypeChangeHandler() => SetButtonAddonType(PlayerHealthBarViewModel.ButtonAddonIDCurrent);

        private void SetButtonAddonType(ButtonAddonID buttonAddonID)
        {
            HideGoldButton();
            HideWatchButton();

            switch (buttonAddonID)
            {
                case ButtonAddonID.WatchVideo:
                    ShowWatchButton();
                    break;
                case ButtonAddonID.Buy:
                    ShowGoldButton();
                    break;
            }
        }

        private void ShowGoldButton() => View.SetIsVisibleBuyAddonButton(true);

        private void ShowWatchButton()
        {
            var watchPurchaseHealthAddon = WatchPurchaseHealthAddon;
            if (watchPurchaseHealthAddon.IsCanPurchase)
            {
                ShowWatchButtonView();
            }
            else
            {
                watchPurchaseHealthAddon.OnReady += OnReadyWatchPurchaseHandler;
                watchPurchaseHealthAddon.Prepere();
            }
        }

        private void OnReadyWatchPurchaseHandler(bool isready)
        {
            WatchPurchaseHealthAddon.OnReady -= OnReadyWatchPurchaseHandler;
            UpdateTypeButtonAddon();
        }

        private void ShowWatchButtonView() => View.SetIsVisibleWatchVideoButton(true);

        private void HideGoldButton() => View.SetIsVisibleBuyAddonButton(false);

        private void HideWatchButton() => View.SetIsVisibleWatchVideoButton(false);

        private float GetPositionX(float health)
        {
            var rectTransformHealth = View.RectTransformHealth;
            var localPosition = rectTransformHealth.localPosition;
            var rect = rectTransformHealth.rect;
            var healthMax = PlayerHealthModel.HealthMax;

            if (health <= healthMax)
            {
                return localPosition.x + rect.xMin + rect.width * (health / healthMax);
            }

            var healthPercent = (health - healthMax) / PlayerHealthModel.HealthBonus;

            return localPosition.x + rect.xMin + rect.width + View.RectTransformHealthAddon.rect.height * healthPercent;
        }

        //TODO: similar syntax in DeathPlayerViewController. 
        //move to one controller
        // add same logic for water and food

        private void OnAddAddonHandler()
        {
            if (IsShowButtonAddonWatch)
            {
                PurchasesModel.Purchase(PurchaseID.HealthAddonWatch, OnPurchaseHealthAddonWatch);
            }
            else
            {
                PurchasesModel.Purchase(PurchaseID.HealthAddonGold, OnPurchaseHealthAddonGold);
            }

            void OnPurchaseHealthAddonGold(PurchaseResult purchaseResult)
            {
                if (purchaseResult == PurchaseResult.Successfully)
                {
                    PositiveResult();
                }
                else
                {
                    PurchasesView = ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
                    PurchasesView.OnClose += OnClosePurchaseHandler;
                }
            }

            void OnClosePurchaseHandler()
            {
                PurchasesView.OnClose -= OnClosePurchaseHandler;
                PurchasesModel.Purchase(PurchaseID.ResurrectPlayerGold, OnPurchaseSecondTry);
            }

            void PositiveResult()
            {
                PlayerBleedingDamagerModel.SetBleeding(false);
                PlayerHealthModel.AdjustHealth(PlayerHealthModel.HealthMax - PlayerHealthModel.HealthCurrent);
                UpdateTypeButtonAddon();
            }

            void OnPurchaseSecondTry(PurchaseResult result)
            {
                if (result == PurchaseResult.Successfully)
                {
                    PositiveResult();
                }
                else
                {
                    // nothing
                }
            }
        }

        private void OnPurchaseHealthAddonWatch(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                PlayerBleedingDamagerModel.SetBleeding(false);
                PlayerHealthModel.AdjustHealth(PlayerHealthModel.HealthMax - PlayerHealthModel.HealthCurrent);
                UpdateTypeButtonAddon();
            }
        }
    }
}
