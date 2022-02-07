using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class RunButtonViewController : ViewControllerBase<RunButtonView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public PlayerStaminaModel PlayerStaminaModel { get; private set; }
        [Inject] public PlayerRunModel PlayerRunModel { get; private set; }
        [Inject] public WeaponCoolDownModel WeaponCoolDownModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }

        protected override void Show()
        {
            View.OnPointerDown_ += OnPointerDownHandler;
            PlayerRunModel.OnChangeIsRunToggle += OnChangeRunHandler;

            UpdateIcon();
        }

        protected override void Hide()
        {
            View.OnPointerDown_ -= OnPointerDownHandler;
            PlayerRunModel.OnChangeIsRunToggle -= OnChangeRunHandler;
        }

        private void OnChangeRunHandler()
        {
            UpdateIcon();

            if (PlayerRunModel.IsRunToggle)
            {
                GameUpdateModel.OnUpdate += OnUpdateHandler;
            }
            else
            {
                GameUpdateModel.OnUpdate -= OnUpdateHandler;
            }
        }

        private void OnUpdateHandler()
        {
            if (WeaponCoolDownModel.CoolDownTime > 0)
            {
                WeaponCoolDownModel.CoolDownTime = WeaponCoolDownModel.CoolDownTimeDefault;
            }           
        }

        private void OnPointerDownHandler()
        {
            if (PlayerRunModel.IsRunToggle)
            {
                PlayerRunModel.RunStop();
                PlayerRunModel.RunTogglePassive();
            }
            else
            {
                PlayerRunModel.RunStart();
                PlayerRunModel.RunToggleActive();
            }
        }

        private void UpdateIcon()
        {
            if (PlayerRunModel.IsRunToggle)
            {
                View.SetIcon(View.IconActive);
            }
            else
            {
                View.SetIcon(View.IconDefault);
            }
        }
    }
}
