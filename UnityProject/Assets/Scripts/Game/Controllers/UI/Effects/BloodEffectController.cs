using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class BloodEffectController : IBloodEffectController, IController
    {
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public BloodEffectModel BloodEffectModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        public BloodEffectView BloodEffectView { get; private set; }

        void IController.Enable()
        {
            if (!PlayerHealthModel.IsDead)
            {
                ShowBloodView();
            }
            else
            {
                PlayerDeathModel.OnRevival += OnRevivalhandler;
            }
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            PlayerHealthModel.OnDeath -= OnDeathHandler;
            PlayerDeathModel.OnRevival -= OnRevivalhandler;
            PlayerHealthModel.OnChangeHealth -= OnChangeHealthHandler;

            HideBloodView();
        }

        private void ShowBloodView()
        {
            BloodEffectView = ViewsSystem.Show<BloodEffectView>(ViewConfigID.Blood);
            UpdateAlpha();

            PlayerHealthModel.OnChangeHealth += OnChangeHealthHandler;
            PlayerHealthModel.OnDeath += OnDeathHandler;
        }

        private void OnRevivalhandler()
        {
            PlayerDeathModel.OnRevival -= OnRevivalhandler;

            ShowBloodView();
        }

        private void OnDeathHandler()
        {
            PlayerHealthModel.OnDeath -= OnDeathHandler;
            PlayerHealthModel.OnChangeHealth -= OnChangeHealthHandler;

            PlayerDeathModel.OnRevival += OnRevivalhandler;

            HideBloodView();
        }

        private void HideBloodView()
        {
            if (BloodEffectView != null)
            {
                ViewsSystem.Hide(BloodEffectView);
            }
        }

        private void OnChangeHealthHandler() => UpdateAlpha();
        private void UpdateAlpha() => BloodEffectView.SetAlpha(1 - Mathf.Clamp(PlayerHealthModel.HealthCurrent, 0, BloodEffectModel.HealthMin) / BloodEffectModel.HealthMin);
    }
}
