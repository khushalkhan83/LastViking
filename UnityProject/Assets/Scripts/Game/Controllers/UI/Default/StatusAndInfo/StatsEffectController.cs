using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class StatsEffectController : IStatsEffectController, IController
    {
        [Inject] public StatsEffectModel StatsEffectModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        protected AddStatsEffectView AddStatsEffectView { get; private set; }

        void IController.Enable() 
        {
            StatsEffectModel.OnShowStatsEffect += AddStatsEffect;
            StatsEffectModel.OnHideStatsEffect += HideStatsEffect;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            StatsEffectModel.OnShowStatsEffect -= AddStatsEffect;
            StatsEffectModel.OnHideStatsEffect -= HideStatsEffect;
            HideStatsEffect();
        }

        private void AddStatsEffect(SavableItem item, Vector3 position)
        {
            var statInfos = new List<AddStatsEffectView.StatInfo>();

            if (item.TryGetProperty("Health Change", out var property))
            {
                statInfos.Add(new AddStatsEffectView.StatInfo() { StatID = AddStatsEffectView.StatID.Health, Count = property.Int.Current });
            }

            if (item.TryGetProperty("Hunger Change", out property))
            {
                statInfos.Add(new AddStatsEffectView.StatInfo() { StatID = AddStatsEffectView.StatID.Food, Count = property.Int.Current });
            }

            if (item.TryGetProperty("Thirst Change", out property))
            {
                statInfos.Add(new AddStatsEffectView.StatInfo() { StatID = AddStatsEffectView.StatID.Water, Count = property.Int.Current });
            }

            HideStatsEffect();
            ShowAddEffect(position, statInfos);
        }

        private void ShowAddEffect(Vector3 position, List<AddStatsEffectView.StatInfo> statInfos)
        {
            AddStatsEffectView = ViewsSystem.Show<AddStatsEffectView>(ViewConfigID.AddStatsEffect, StatsEffectModel.ViewContainer);
            AddStatsEffectView.OnEndAll += OnEndAllAddEffectsHandler;
            AddStatsEffectView.SetPosition(position);
            AddStatsEffectView.StartEffect(statInfos.ToArray());
        }

        private void HideStatsEffect()
        {
            if (AddStatsEffectView != null)
            {
                AddStatsEffectView.EndEffect();
                AddStatsEffectView.OnEndAll -= OnEndAllAddEffectsHandler;
                ViewsSystem.Hide(AddStatsEffectView);
                AddStatsEffectView = null;
            }
        }

        private void OnEndAllAddEffectsHandler() => HideStatsEffect();
    }
}
