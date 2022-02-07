using Core;
using Core.Controllers;
using Game.Models;
using Game.Models.RemoteSettings;
using Game.Models.RemoteSettings.Firebase;
using Game.Purchases;
using Game.Views;
using UnityEngine;
using ButtonAddonID = Game.Models.PlayerWaterBarViewModel.ButtonAddonID;
using WaterAddonRemoteSettings = Game.Models.RemoteSettings.WaterAddonRemoteSettings;

namespace Game.Controllers
{
    public class WaterBarViewController : ViewControllerBase<WaterBarView>
    {
        [Inject] public PlayerWaterBarViewModel PlayerWaterBarViewModel { get; private set; }
        [Inject] public RemoteSettingsModel RemoteSettingsModel { get; private set; }
        [Inject] public AdvertisementsModel AdvertisementsModel { get; private set; }
        [Inject] public PlayerWaterModel PlayerWaterModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public FirebaseRemoteSettingsModel FirebaseRemoteSettingsModel { get; private set; }

        protected bool GetIsShowButtonAddonWatch(IWatchPurchase watchPurchase) => !WaterAddonRemoteSettings.IsAddonGold && (watchPurchase.IsCanPurchase || NetworkModel.IsHasConnection);

        protected bool IsShowButtonAddonWatch => GetIsShowButtonAddonWatch(WatchPurchaseWaterAddon);

        protected IWatchPurchase WatchPurchaseWaterAddon => PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.WaterAddonWatch);

        protected WaterAddonRemoteSettings WaterAddonRemoteSettings => RemoteSettingsModel.Get<WaterAddonRemoteSettings>(RemoteSettingID.WaterAddon);

        protected override void Show()
        {
            PlayerWaterModel.OnChangeWater += OnChangeWaterHandler;
            PlayerWaterModel.OnChangeAddonLevel += OnChangeAddonLevel;
            PlayerWaterBarViewModel.OnChangeAddonButtonType += OnAddonButtonTypeChangeHandler;

            View.OnAddAddon += OnAddAddonHandler;

            NetworkModel.OnInternetConnectionStateChange += HandleInternetConnection;
            NetworkModel.UpdateInternetConnectionStatus();

            FirebaseRemoteSettingsModel.OnFetchedRemote += OnFetchedRemote;

            View.SetTextBuyCoinsCount(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.WaterAddonGold).CoinCost.ToString());

            UpdateWaterLevel();
        }

        protected override void Hide()
        {
            PlayerWaterModel.OnChangeWater -= OnChangeWaterHandler;
            PlayerWaterModel.OnChangeAddonLevel -= OnChangeAddonLevel;
            PlayerWaterBarViewModel.OnChangeAddonButtonType -= OnAddonButtonTypeChangeHandler;

            NetworkModel.OnInternetConnectionStateChange -= HandleInternetConnection;
            FirebaseRemoteSettingsModel.OnFetchedRemote -= OnFetchedRemote;

            View.OnAddAddon -= OnAddAddonHandler;
        }

        private void OnFetchedRemote() => View.SetTextBuyCoinsCount(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.WaterAddonGold).CoinCost.ToString());

        private void HandleInternetConnection() => UpdateTypeButtonAddon();

        private void UpdateTypeButtonAddon() => PlayerWaterBarViewModel.SetButtonAddonType(GetButtonAddonID());

        private void OnChangeAddonLevel() => UpdateWaterLevel();

        private void OnChangeWaterHandler() => UpdateWaterLevel();

        private void UpdateWaterLevel()
        {
            UpdateTypeButtonAddon();
            UpdateViewColor();
            View.SetValue(((int)PlayerWaterModel.WaterCurrent).ToString());
        }

        private void UpdateViewColor()
        {
            bool isHunger = PlayerWaterModel.WaterCurrent < PlayerWaterModel.WaterToThirst;
            View.SetValueTextColor( isHunger ? View.LowWaterColor : View.NormalWaterColor);
            View.SetIconColor( isHunger ? View.LowWaterColor : View.NormalWaterColor);
        }

        private ButtonAddonID GetButtonAddonID()
        {
            if (PlayerWaterModel.WaterCurrent < PlayerWaterBarViewModel.WaterToShowButtonAddon)
            {
                if (IsShowButtonAddonWatch)
                {
                    return ButtonAddonID.WatchVideo;
                }

                return ButtonAddonID.Buy;
            }

            return ButtonAddonID.None;
        }

        private void OnAddonButtonTypeChangeHandler() => SetButtonAddonType(PlayerWaterBarViewModel.ButtonAddonIDCurrent);

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
            var watchPurchaseWaterAddon = WatchPurchaseWaterAddon;
            if (watchPurchaseWaterAddon.IsCanPurchase)
            {
                ShowWatchButtonView();
            }
            else
            {
                watchPurchaseWaterAddon.OnReady += OnReadyWatchPurchaseHandler;
                watchPurchaseWaterAddon.Prepere();
            }
        }

        private void OnReadyWatchPurchaseHandler(bool isready)
        {
            WatchPurchaseWaterAddon.OnReady -= OnReadyWatchPurchaseHandler;
            UpdateTypeButtonAddon();
        }

        private void ShowWatchButtonView() => View.SetIsVisibleWatchVideoButton(true);

        private void HideGoldButton() => View.SetIsVisibleBuyAddonButton(false);

        private void HideWatchButton() => View.SetIsVisibleWatchVideoButton(false);

        private void OnAddAddonHandler()
        {
            if (IsShowButtonAddonWatch)
            {
                PurchasesModel.Purchase(PurchaseID.WaterAddonWatch, OnPurchaseWaterAddonWatch);
            }
            else
            {
                PurchasesModel.Purchase(PurchaseID.WaterAddonGold, OnPurchaseWaterAddonGold);
            }
        }

        private void OnPurchaseWaterAddonWatch(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                PlayerWaterModel.AdjustWater(PlayerWaterModel.WaterMax - PlayerWaterModel.WaterCurrent);
                UpdateTypeButtonAddon();
            }
        }

        private void OnPurchaseWaterAddonGold(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                PlayerWaterModel.AdjustWater(PlayerWaterModel.WaterMax - PlayerWaterModel.WaterCurrent);
                UpdateTypeButtonAddon();
            }
            else
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
        }
    }
}
