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
    public class WarmBarViewController : ViewControllerBase<WarmBarView>
    {
        [Inject] public RemoteSettingsModel RemoteSettingsModel { get; private set; }
        [Inject] public AdvertisementsModel AdvertisementsModel { get; private set; }
        [Inject] public PlayerWarmModel PlayerWarmModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        protected bool GetIsShowButtonAddonWatch(IWatchPurchase watchPurchase) => !WaterAddonRemoteSettings.IsAddonGold && (watchPurchase.IsCanPurchase || NetworkModel.IsHasConnection);

        protected bool IsShowButtonAddonWatch => false;

        protected IWatchPurchase WatchPurchaseWaterAddon => PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.WaterAddonWatch);

        protected WaterAddonRemoteSettings WaterAddonRemoteSettings => RemoteSettingsModel.Get<WaterAddonRemoteSettings>(RemoteSettingID.WaterAddon);

        protected override void Show()
        {
            PlayerWarmModel.OnChangeWarm += OnChangeWarmHandler;
            View.OnAddAddon += OnAddAddonHandler;
            View.SetTextBuyCoinsCount(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.WaterAddonGold).CoinCost.ToString());

            UpdateWarmLevel();
        }

        protected override void Hide()
        {
            PlayerWarmModel.OnChangeWarm -= OnChangeWarmHandler;
            View.OnAddAddon -= OnAddAddonHandler;
        }

        private void OnFetchedRemote() => View.SetTextBuyCoinsCount(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.WaterAddonGold).CoinCost.ToString());

        private void OnChangeWarmHandler() => UpdateWarmLevel();

        private void UpdateWarmLevel()
        {
            UpdateAddonButton();
            UpdateViewColor();
            View.SetValue(((int)PlayerWarmModel.Warm).ToString());
        }

        private void UpdateViewColor()
        {
            bool isHunger = PlayerWarmModel.Warm < PlayerWarmModel.WarmToFreeze;
            View.SetValueTextColor( isHunger ? View.LowWarmColor : View.NormalWarmColor);
            View.SetIconColor( isHunger ? View.LowWarmColor : View.NormalWarmColor);
        }

        private void UpdateAddonButton()
        {
            SetButtonAddonType(GetButtonAddonID());
        }

        private ButtonAddonID GetButtonAddonID()
        {
            if (PlayerWarmModel.Warm < PlayerWarmModel.WarmToFreeze)
            {
                if (IsShowButtonAddonWatch)
                {
                    return ButtonAddonID.WatchVideo;
                }

                return ButtonAddonID.Buy;
            }

            return ButtonAddonID.None;
        }

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
        }

        private void ShowWatchButtonView() => View.SetIsVisibleWatchVideoButton(true);

        private void HideGoldButton() => View.SetIsVisibleBuyAddonButton(false);

        private void HideWatchButton() => View.SetIsVisibleWatchVideoButton(false);

        private void OnAddAddonHandler()
        {
            if (IsShowButtonAddonWatch)
            {
                PurchasesModel.Purchase(PurchaseID.WaterAddonWatch, OnPurchaseAddonWatch);
            }
            else
            {
                PurchasesModel.Purchase(PurchaseID.WaterAddonGold, OnPurchaseAddonGold);
            }
        }

        private void OnPurchaseAddonWatch(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                PlayerWarmModel.AdjustWarm(PlayerWarmModel.MaxWarm - PlayerWarmModel.Warm);
            }
        }

        private void OnPurchaseAddonGold(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                PlayerWarmModel.AdjustWarm(PlayerWarmModel.MaxWarm - PlayerWarmModel.Warm);
            }
            else
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
        }

    }
}
