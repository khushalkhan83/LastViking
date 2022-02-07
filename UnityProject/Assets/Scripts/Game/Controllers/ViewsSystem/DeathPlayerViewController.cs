using Core;
using Core.Controllers;
using Core.Views;
using Game.Audio;
using Game.Models;
using Game.Purchases;
using Game.Views;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class DeathPlayerViewController : ViewControllerBase<DeathPlayerView>
    {
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public LoseViewModel LoseViewModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public PlayerDeathHandler PlayerDeathHandler { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerFoodModel PlayerFoodModel { get; private set; }
        [Inject] public PlayerWaterModel PlayerWaterModel { get; private set; }
        [Inject] public PlayerStaminaModel PlayerStaminaModel { get; private set; }
        [Inject] public PlayerProfileModel PlayerProfileModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public PlayerRunModel PlayerRunModel { get; private set; }
        [Inject] public TouchpadModel TouchpadModel { get; private set; }
        [Inject] public JoystickModel JoystickModel { get; private set; }
        [Inject] public PlayerBleedingDamagerModel PlayerBleedingDamagerModel { get; private set; }
        [Inject] public PlayerPoisonDamagerModel PlayerPoisonDamagerModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public InventoryOperationsModel InventoryOperationsModel { get; private set; }
        [Inject] public BonusItemsModel BonusItemsModel { get; private set; }

        protected PurchasesView PurchasesView { get; private set; }

        protected IWatchPurchase WatchPurchaseResurectPlayer => PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.ResurrectPlayerWatch);

        protected override void Show()
        {
            View.OnShow += OnShow;

            GameUpdateModel.OnUpdate += OnUpdate;
            LocalizationModel.OnChangeLanguage += SetLocalization;
            NetworkModel.OnInternetConnectionStateChange += OnInternetConnectionStateChangeHandler;
            NetworkModel.UpdateInternetConnectionStatus();

            SetLocalization();
            UpdateResurectButton();

            View.SetResurrectGoldCostText(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.ResurrectPlayerGold).CoinCost);
            bool hasBonus = true;

            View.ShowGoldBonusObj(hasBonus);
            if(hasBonus)
            {
                var item = BonusItemsModel.GetBonusItem();
                View.SetBonusImage(ItemsDB.GetItem(item).Icon);
            }

            View.SetPlayerScore(PlayerProfileModel.PlayerScore);
            View.SetResurrectTimeProgress(1);
        }

        protected override void Hide()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;

            View.OnShow -= OnShow;
            View.OnWatch -= OnResurrectWatch;
            View.OnGold -= OnResurrectGold;
            NetworkModel.OnInternetConnectionStateChange -= OnInternetConnectionStateChangeHandler;

            LocalizationModel.OnChangeLanguage -= SetLocalization;

            StopAllCoroutines();
        }

        private void OnInternetConnectionStateChangeHandler() => UpdateResurectButton();

        private void UpdateResurectButton()
        {
            View.OnGold -= OnResurrectGold;
            View.OnWatch -= OnResurrectWatch;

            var isOverCount = LoseViewModel.ResurrectWatchCount >= LoseViewModel.WatchCountToResurrectGold;
            var watchPurchaseResurectPlayer = WatchPurchaseResurectPlayer;
            var isGoldResurrect = !watchPurchaseResurectPlayer.IsCanPurchase || !NetworkModel.IsHasConnection || isOverCount;

            View.SelectResurrectButton(isGoldResurrect);

            if (isGoldResurrect)
            {
                View.OnGold += OnResurrectGold;
            }
            else
            {
                watchPurchaseResurectPlayer.Prepere();
                View.OnWatch += OnResurrectWatch;
            }
        }

        private void OnShow(IView view)
        {
            _tickProgress = LoseViewModel.ResurrectTimeWait;
            _isResurrectProgressActive = true;
        }

        private float _tickProgress = 0;
        private bool _isResurrectProgressActive;

        private void OnUpdate()
        {
            if (_isResurrectProgressActive)
            {
                _tickProgress -= Time.unscaledDeltaTime;
                View.SetResurrectTimeProgress(_tickProgress / LoseViewModel.ResurrectTimeWait);

                if (_tickProgress <= 0)
                {
                    _isResurrectProgressActive = false;
                    OnEndResurrectTime();
                }
            }
        }

        private void OnEndResurrectTime()
        {
            LoseViewModel.EndResurrectTime();
            View.OnHide += OnHideEndTime;
            ViewsSystem.Hide(View);
        }

        private void OnHideEndTime(IView view)
        {
            View.OnHide -= OnHideEndTime;
        }

        //TODO: similar syntax in HealthBarViewController. 
        //move to one controller
        // add same logic for water and food

        private void OnResurrectGold()
        {
            AudioSystem.PlayOnce(AudioID.WindowOpen);
            _isResurrectProgressActive = false;

            var resurrectGold = PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.ResurrectPlayerGold);
            PurchasesModel.Purchase(PurchaseID.ResurrectPlayerGold, OnResurrectGoldPurchased);

            void OnResurrectGoldPurchased(PurchaseResult purchaseResult)
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
                
                void OnClosePurchaseHandler()
                {
                    PurchasesView.OnClose -= OnClosePurchaseHandler;
                    PurchasesModel.Purchase(PurchaseID.ResurrectPlayerGold, OnResurectPurchaseSecondTry);
                }

                void OnResurectPurchaseSecondTry(PurchaseResult result)
                {
                    if (result == PurchaseResult.Successfully)
                    {
                        PositiveResult();
                    }
                    else
                    {
                        _isResurrectProgressActive = true;
                    }
                }
            }

            void PositiveResult()
            {
                var item = BonusItemsModel.GetBonusItem();
                InventoryOperationsModel.AddItemToPlayer(item);
                BonusItemsModel.ReceiveBonus();

                LoseViewModel.RessurectAction();
                LoseViewModel.ResetShouldCreateTomb();
                Resurrect();
                PlayerBleedingDamagerModel.SetBleeding(false);
                PlayerPoisonDamagerModel.AddAntidote();
            }
        }

        private void OnResurrectWatch()
        {
            AudioSystem.PlayOnce(AudioID.WindowOpen);
            _isResurrectProgressActive = false;
            PurchasesModel.Purchase(PurchaseID.ResurrectPlayerWatch, OnResurrectWatchPurchased);

            void OnResurrectWatchPurchased(PurchaseResult purchaseResult)
            {
                if (purchaseResult == PurchaseResult.Successfully)
                {
                    if (LoseViewModel.ResurrectWatchCount < LoseViewModel.WatchCountToResurrectGold)
                    {
                        LoseViewModel.UpdateResurrectWatchCount();
                    }
                    LoseViewModel.RessurectAction();
                    LoseViewModel.ResetShouldCreateTomb();
                    Resurrect();
                    PlayerBleedingDamagerModel.SetBleeding(false);
                    PlayerPoisonDamagerModel.AddAntidote();
                }
                else
                {
                    _isResurrectProgressActive = true;
                }
            }
        }

        private void Resurrect()
        {
            JoystickModel.SetDefaultAxes();
            TouchpadModel.SetDefaultAxes();
            PlayerRunModel.RunStop();
            PlayerRunModel.RunTogglePassive();
            PlayerEventHandler.Jump.ForceStop();
            PlayerEventHandler.Aim.ForceStop();

            View.OnHide += OnHideRestart;
            ViewsSystem.Hide(View);
        }

        private void OnHideRestart(IView view)
        {
            View.OnHide -= OnHideRestart;
            StopAllCoroutines();
            RestartGame();
        }

        private void RestartGame()
        {
            Refill();

            PlayerEventHandler.OnPreSaveHandler();
            PlayerDeathHandler.Respawn(PlayerDeathModel.ImunitetDuration);

            PlayerDeathModel.PrelimRevival();
            PlayerDeathModel.BeginImunitet();
        }

        private void Refill()
        {
            PlayerHealthModel.RefillHealth();
            PlayerFoodModel.RefillFood();
            PlayerWaterModel.RefillWater();
            PlayerStaminaModel.RefillStamina();
        }

        private void SetLocalization()
        {
            View.SetTextTitle(LocalizationModel.GetString(LocalizationKeyID.LoseMenu_Title));
            View.SetTextScoreTitle(LocalizationModel.GetString(LocalizationKeyID.AccountPopup_Score));
            View.SetTextDescriptionResurectBack(LocalizationModel.GetString(LocalizationKeyID.LoseMenu_ResurrectTimer));
            View.SetTextDescriptionResurectFront(LocalizationModel.GetString(LocalizationKeyID.LoseMenu_ResurrectTimer));
            View.SetTextWatchButtonText(LocalizationModel.GetString(LocalizationKeyID.GameStore_WatchBtn));
            View.SetTextGoldButtonText(LocalizationModel.GetString(LocalizationKeyID.LoseMenu_GoldBtn));
        }
    }
}
