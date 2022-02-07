using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class AttackButtonViewController : ViewControllerBase<AttackButtonView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public AttackButtonViewModel AttackButtonViewModel { get; private set; }

        private void OnInputStopAttackHandler()
        {
            GameUpdateModel.OnUpdate -= AttackContinuouslyProcess;
            View.SetIcon(View.IconDefault);
        }

        private void OnInputAttackHandler()
        {
            GameUpdateModel.OnUpdate += AttackContinuouslyProcess;
            PlayerEventHandler.AttackOnce.Try();
            View.SetIcon(View.IconActive);
        }

        protected override void Show()
        {
            PlayerDeathModel.OnPreRevival += OnInputStopAttackHandler;
            View.OnPointerDown_ += OnPointerDownHandler;
            View.OnPointerUp_ += OnPointerUpHandler;
            AttackButtonViewModel.OnPulseAnimationChanged += OnPulseAnimationChanged;
            View.SetIcon(View.IconDefault);
            if(AttackButtonViewModel.PulseAnimation)
                OnPulseAnimationChanged();
        }

        protected override void Hide()
        {
            View.OnPointerDown_ -= OnPointerDownHandler;
            View.OnPointerUp_ -= OnPointerUpHandler;
            AttackButtonViewModel.OnPulseAnimationChanged -= OnPulseAnimationChanged;
            if (PlayerDeathModel)
            {
                PlayerDeathModel.OnPreRevival -= OnInputStopAttackHandler;
            }
            if (GameUpdateModel)
            {
                GameUpdateModel.OnUpdate -= AttackContinuouslyProcess;
            }
        }

        private void OnPulseAnimationChanged()
        {
            if(AttackButtonViewModel.PulseAnimation)
                View.PlayPulse();
            else
                View.PlayDefault();
        }

        private void OnPointerDownHandler() => OnInputAttackHandler();

        private void OnPointerUpHandler() => OnInputStopAttackHandler();

        private void AttackContinuouslyProcess() => PlayerEventHandler.AttackContinuously.Try();
    }
}
