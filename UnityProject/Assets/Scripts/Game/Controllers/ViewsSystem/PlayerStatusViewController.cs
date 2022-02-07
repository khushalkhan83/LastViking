using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class PlayerStatusViewController : ViewControllerBase<PlayerStatusView>
    {
        [Inject] public PlayerHealthModel PlayerHealthModel { get; protected set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; protected set; }
        [Inject] public PlayerWaterModel PlayerWaterModel { get; protected set; }
        [Inject] public PlayerFoodModel PlayerFoodModel { get; protected set; }
        [Inject] public PlayerWarmModel PlayerWarmModel { get; protected set; }
        [Inject] public ViewsSystem ViewsSystem { get; protected set; }
        [Inject] public PlayerPoisonDamagerModel PlayerPoisonDamagerModel { get; protected set; }
        [Inject] public PlayerBleedingDamagerModel PlayerBleedingDamagerModel { get; protected set; }
        [Inject] public AttackDelayStatusViewModel AttackDelayStatusViewModel { get; protected set; }
        [Inject] public PlayerStatusViewModel PlayerStatusViewModel { get; protected set; }

        public ColdStatusView ColdView;
        public ThirstStatusView ThirstView;
        public HungerStatusView HungerView;
        public BleedingView BleedingView;
        public PoisoningView PoisoningView;
        public AttackDelayStatusView AttackDelayStatusView;

        protected override void Show()
        {
            PlayerWarmModel.OnStartColding += OnPlayerStartColding;
            PlayerWarmModel.OnStopColding += OnPlayerStopColding;

            PlayerWaterModel.OnStartThirsting += OnPlayerStartThirsting;
            PlayerWaterModel.OnStopThirsting += OnPlayerStopThirsting;

            PlayerFoodModel.OnStartHungering += OnPlayerStartHungering;
            PlayerFoodModel.OnStopHungering += OnPlayerStopHungering;

            PlayerPoisonDamagerModel.OnAddPosion += OnAddPoisonHandler;
            PlayerPoisonDamagerModel.OnAddAntidote += OnAddAntidoteHandler;
            PlayerPoisonDamagerModel.OnEndPoison += OnEndPoisonHandler;

            PlayerBleedingDamagerModel.OnChangeHitFrom += OnChangeHitFromHandler;

            AttackDelayStatusViewModel.OnStartWaitAttack += OnStartWaitAttack;
            AttackDelayStatusViewModel.OnEndWaitAttack += OnEndWaitAttack;

            PlayerStatusViewModel.OnEnableEnviroTime += OnEnableEnviroTime;
            PlayerStatusViewModel.OnDisableEnviroTime += OnDisableEnviroTime;

            if (PlayerWarmModel.IsColding)
                OnPlayerStartColding();
            if (PlayerWaterModel.IsThirsting)
                OnPlayerStartThirsting();
            if (PlayerFoodModel.IsHungering)
                OnPlayerStartHungering();
            if (PlayerBleedingDamagerModel.IsBleeding)
                ShowView(ref BleedingView, ViewConfigID.Bleeding);
            if (PlayerPoisonDamagerModel.IsHasPoison)
                OnAddPoisonHandler();
        }

        protected override void Hide()
        {
            PlayerWaterModel.OnStartThirsting -= OnPlayerStartThirsting;
            PlayerWaterModel.OnStopThirsting -= OnPlayerStopThirsting;

            PlayerWarmModel.OnStartColding -= OnPlayerStartColding;
            PlayerWarmModel.OnStopColding -= OnPlayerStopColding;

            PlayerFoodModel.OnStartHungering -= OnPlayerStartHungering;
            PlayerFoodModel.OnStopHungering -= OnPlayerStopHungering;

            PlayerPoisonDamagerModel.OnAddPosion -= OnAddPoisonHandler;
            PlayerPoisonDamagerModel.OnAddAntidote -= OnAddAntidoteHandler;
            PlayerPoisonDamagerModel.OnEndPoison -= OnEndPoisonHandler;

            PlayerBleedingDamagerModel.OnChangeHitFrom -= OnChangeHitFromHandler;

            AttackDelayStatusViewModel.OnStartWaitAttack -= OnStartWaitAttack;
            AttackDelayStatusViewModel.OnEndWaitAttack -= OnEndWaitAttack;

            PlayerStatusViewModel.OnEnableEnviroTime -= OnEnableEnviroTime;
            PlayerStatusViewModel.OnDisableEnviroTime -= OnDisableEnviroTime;

            OnPlayerStopThirsting();
            OnPlayerStopColding();
            OnPlayerStopHungering();
            OnAddAntidoteHandler();
            HideView(ref BleedingView);
            OnEndWaitAttack();
        }

        private void OnAddPoisonHandler() => ShowView(ref PoisoningView, ViewConfigID.Poisoning);
        private void OnAddAntidoteHandler() => HideView(ref PoisoningView);
        private void OnEndPoisonHandler() => HideView(ref PoisoningView);

        private void OnChangeHitFromHandler()
        {
            if (PlayerBleedingDamagerModel.IsBleeding)
            {
                ShowView(ref BleedingView, ViewConfigID.Bleeding);
            }
            else
            {
                HideView(ref BleedingView);
            }
        }

        private void OnPlayerStartThirsting() => ShowView(ref ThirstView, ViewConfigID.ThirstStatus);
        private void OnPlayerStopThirsting() => HideView(ref ThirstView);

        private void OnPlayerStartColding() => ShowView(ref ColdView, ViewConfigID.ColdStatus);
        private void OnPlayerStopColding() => HideView(ref ColdView);

        private void OnPlayerStartHungering() => ShowView(ref HungerView, ViewConfigID.HungerStatus);
        private void OnPlayerStopHungering() => HideView(ref HungerView);

        private void OnStartWaitAttack()
        {
            if (!PlayerStatusViewModel.IsShowingAttackDelayView)
            {
                PlayerStatusViewModel.IsShowingAttackDelayView = true;
                ShowView(ref AttackDelayStatusView, ViewConfigID.AttackDelayStatus);
            }
        }

        private void OnEndWaitAttack()
        {
            if (PlayerStatusViewModel.IsShowingAttackDelayView)
            {
                PlayerStatusViewModel.IsShowingAttackDelayView = false;
                HideView(ref AttackDelayStatusView);
            }
        }

        private void OnEnableEnviroTime()
        {
            if (PlayerStatusViewModel.IsShowingAttackDelayView)
            {
                ShowView(ref AttackDelayStatusView, ViewConfigID.AttackDelayStatus);
            }
        }
        private void OnDisableEnviroTime() => HideView(ref AttackDelayStatusView);

        private void ShowView<T>(ref T view, ViewConfigID viewConfigID) where T : ViewBase
        {
            if (!view)
                view = ViewsSystem.Show<T>(viewConfigID, View.ViewsContainer);

            OnAfterShowView();
        }

        private void HideView<T>(ref T view) where T : ViewBase
        {
            if (view != null)
            {
                ViewsSystem.Hide(view);
                view = null;
            }
        }

        private void OnAfterShowView()
        {
            if (AttackDelayStatusView)
            {
                AttackDelayStatusView.transform.SetAsFirstSibling();
            }
        }
    }
}
