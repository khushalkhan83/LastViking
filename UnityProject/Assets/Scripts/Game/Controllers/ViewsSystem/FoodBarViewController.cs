using Core;
using Core.Controllers;
using Game.Models;
using Game.Models.RemoteSettings;
using Game.Models.RemoteSettings.Firebase;
using Game.Purchases;
using Game.Views;
using UnityEngine;
using ButtonAddonID = Game.Models.PlayerFoodBarViewModel.ButtonAddonID;
using FoodAddonRemoteSettings = Game.Models.RemoteSettings.FoodAddonRemoteSettings;

namespace Game.Controllers
{
    public class FoodBarViewController : ViewControllerBase<FoodBarView>
    {
        [Inject] public PlayerFoodBarViewModel PlayerFoodBarViewModel { get; private set; }
        [Inject] public RemoteSettingsModel RemoteSettingsModel { get; private set; }
        [Inject] public AdvertisementsModel AdvertisementsModel { get; private set; }
        [Inject] public PlayerFoodModel PlayerFoodModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public FirebaseRemoteSettingsModel FirebaseRemoteSettingsModel { get; private set; }
        [Inject] public PlayerHungerModel PlayerHungerModel { get; private set; }

        protected bool GetIsShowButtonAddonWatch(IWatchPurchase watchPurchase) => !FoodAddonRemoteSettings.IsAddonGold && (watchPurchase.IsCanPurchase || NetworkModel.IsHasConnection);

        protected bool IsShowButtonAddonWatch => GetIsShowButtonAddonWatch(WatchPurchaseFoodAddon);

        protected IWatchPurchase WatchPurchaseFoodAddon => PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.FoodAddonWatch);

        protected FoodAddonRemoteSettings FoodAddonRemoteSettings => RemoteSettingsModel.Get<FoodAddonRemoteSettings>(RemoteSettingID.FoodAddon);

        protected override void Show()
        {
            PlayerFoodModel.OnChangeFood += OnChangeFoodHandler;
            PlayerFoodModel.OnChangeAddonLevel += OnChangeAddonLevel;
            PlayerFoodBarViewModel.OnChangeAddonButtonType += OnAddonButtonTypeChangeHandler;

            View.OnAddAddon += OnAddAddonHandler;

            NetworkModel.OnInternetConnectionStateChange += HandleInternetConnection;
            NetworkModel.UpdateInternetConnectionStatus();

            FirebaseRemoteSettingsModel.OnFetchedRemote += OnFetchedRemote;

            View.SetTextBuyCoinsCount(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.FoodAddonGold).CoinCost.ToString());

            UpdateFoodLevel();
        }

        protected override void Hide()
        {
            PlayerFoodModel.OnChangeFood -= OnChangeFoodHandler;
            PlayerFoodModel.OnChangeAddonLevel -= OnChangeAddonLevel;
            PlayerFoodBarViewModel.OnChangeAddonButtonType -= OnAddonButtonTypeChangeHandler;

            NetworkModel.OnInternetConnectionStateChange -= HandleInternetConnection;
            FirebaseRemoteSettingsModel.OnFetchedRemote -= OnFetchedRemote;

            View.OnAddAddon -= OnAddAddonHandler;
        }

        private void OnFetchedRemote() => View.SetTextBuyCoinsCount(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.FoodAddonGold).CoinCost.ToString());

        private void HandleInternetConnection() => UpdateTypeButtonAddon();

        private void UpdateTypeButtonAddon() => PlayerFoodBarViewModel.SetButtonAddonType(GetButtonAddonID());

        private void OnChangeAddonLevel() => UpdateFoodLevel();

        private void OnChangeFoodHandler() => UpdateFoodLevel();

        private void UpdateFoodLevel()
        {
            UpdateTypeButtonAddon();
            UpdateViewColor();
            View.SetValue(((int)PlayerFoodModel.FoodCurrent).ToString());
        }

        private void UpdateViewColor()
        {
            bool isHunger = PlayerFoodModel.FoodCurrent < PlayerHungerModel.FoodToHunger;
            View.SetValueTextColor( isHunger ? View.LowFoodColor : View.NormalFoodColor);
            View.SetIconColor( isHunger ? View.LowFoodColor : View.NormalFoodColor);
        }

        private ButtonAddonID GetButtonAddonID()
        {
            if (PlayerFoodModel.FoodCurrent < PlayerFoodBarViewModel.FoodToShowButtonAddon)
            {
                if (IsShowButtonAddonWatch)
                {
                    return ButtonAddonID.WatchVideo;
                }

                return ButtonAddonID.Buy;
            }

            return ButtonAddonID.None;
        }

        private void OnAddonButtonTypeChangeHandler() => SetButtonAddonType(PlayerFoodBarViewModel.ButtonAddonIDCurrent);

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
            var watchPurchaseFoodAddon = WatchPurchaseFoodAddon;
            if (watchPurchaseFoodAddon.IsCanPurchase)
            {
                ShowWatchButtonView();
            }
            else
            {
                watchPurchaseFoodAddon.OnReady += OnReadyWatchPurchaseHandler;
                watchPurchaseFoodAddon.Prepere();
            }
        }

        private void OnReadyWatchPurchaseHandler(bool isready)
        {
            WatchPurchaseFoodAddon.OnReady -= OnReadyWatchPurchaseHandler;
            UpdateTypeButtonAddon();
        }

        private void ShowWatchButtonView() => View.SetIsVisibleWatchVideoButton(true);

        private void HideGoldButton() => View.SetIsVisibleBuyAddonButton(false);

        private void HideWatchButton() => View.SetIsVisibleWatchVideoButton(false);

        private void OnAddAddonHandler()
        {
            if (IsShowButtonAddonWatch)
            {
                PurchasesModel.Purchase(PurchaseID.FoodAddonWatch, OnPurchaseFoodAddonWatch);
            }
            else
            {
                PurchasesModel.Purchase(PurchaseID.FoodAddonGold, OnPurchaseFoodAddonGold);
            }
        }

        private void OnPurchaseFoodAddonWatch(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                PlayerFoodModel.AdjustFood(PlayerFoodModel.FoodMax - PlayerFoodModel.FoodCurrent);
                UpdateTypeButtonAddon();
            }
        }

        private void OnPurchaseFoodAddonGold(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                PlayerFoodModel.AdjustFood(PlayerFoodModel.FoodMax - PlayerFoodModel.FoodCurrent);
                UpdateTypeButtonAddon();
            }
            else
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
        }
    }
}
