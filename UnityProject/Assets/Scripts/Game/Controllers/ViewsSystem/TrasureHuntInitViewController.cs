using System;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class TrasureHuntInitViewController : ViewControllerBase<AimButtonView>
    {
        [Inject] public TreasureHuntModel huntModel { get; private set; }
        [Inject] public RealTimeModel realTimeModel { get; private set; }

        protected override void Hide()
        {
            huntModel.OnActiveChanged -= UpdateInit;
            Deinit();
        }

        protected override void Show()
        {
            UpdateInit();
            huntModel.OnActiveChanged += UpdateInit;
        }

        private void UpdateInit()
        {
            if (huntModel.IsActive)
            {
                Init();
            }
            else
            {
                Deinit();
            }
        }

        private void Init()
        {
            SetAlpha(1);
            View.OnPointerDown_ += OnUseBottle;
        }

        private void Deinit()
        {
            SetAlpha(0.5f);
            View.OnPointerDown_ -= OnUseBottle;
        }

        private void SetAlpha(float a)
        {
            var col = View.Image.color;
            col.a = a;
            View.Image.color = col;
        }

        void OnUseBottle()
        {
            huntModel.StartDigMode();
        }
    }
}