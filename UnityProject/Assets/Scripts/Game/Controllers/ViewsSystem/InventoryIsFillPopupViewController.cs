using Core;
using Core.Controllers;
using Game.Models;
using Game.Models.RemoteSettings;
using Game.Purchases;
using Game.Views;

namespace Game.Controllers
{
    public class InventoryIsFillPopupViewController : ViewControllerBase<InventoryIsFillPopupView>
    {
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public RemoteSettingsModel RemoteSettingsModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public InventoryViewModel InventoryViewModel { get; private set; }

        private InventoryRemoteSettings RemoteSettingsInventory => RemoteSettingsModel.Get<InventoryRemoteSettings>(RemoteSettingID.Inventory);
        private IWatchPurchase WatchPurchasePlayerInventoryExpand => PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.InventoryExpandWatch);
        private bool IsShowExpaundButtonWatch => !RemoteSettingsInventory.IsBuySlotsGold && (WatchPurchasePlayerInventoryExpand.IsCanPurchase || NetworkModel.IsHasConnection);

        private bool GetCanExpandInventory() => !InventoryViewModel.IsMaxExpandLevel;

        protected override void Show()
        {
            NetworkModel.OnInternetConnectionStateChange += OnInternetConnectionStateChangeHanler;
            NetworkModel.UpdateInternetConnectionStatus();

            LocalizationModel.OnChangeLanguage += OnChangeLanguageHandler;
            UpdateButton();

            SetLocalization();
        }

        private void OnInternetConnectionStateChangeHanler() => UpdateButton();

        protected override void Hide()
        {
            LocalizationModel.OnChangeLanguage -= OnChangeLanguageHandler;
            NetworkModel.OnInternetConnectionStateChange -= OnInternetConnectionStateChangeHanler;
        }

        private void UpdateButton()
        {
            bool canExpand = GetCanExpandInventory();
            var isWatchButton = IsShowExpaundButtonWatch;

            View.SetIsVisibleExpandGoldButton(canExpand && !isWatchButton);
            View.SetIsVisibleExpandWatchButton(canExpand && isWatchButton);

            if (!isWatchButton)
            {
                View.SetCountCoin(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.PlayerInventoryExpandGold).CoinCost.ToString());
            }
        }

        private void OnChangeLanguageHandler() => SetLocalization();

        private void SetLocalization()
        {
            View.SetTextMessageText(LocalizationModel.GetString(LocalizationKeyID.NotEnoughSpacePopUp_Text));
            View.SetTextBackButton(LocalizationModel.GetString(LocalizationKeyID.NotEnoughSpacePopUp_BackBtn));
            View.SetTextExpandGoldButton(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_ExpandOnceBtn));
            View.SetTextExpandWatchButton(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_ExpandOnceBtn));
        }
    }
}
