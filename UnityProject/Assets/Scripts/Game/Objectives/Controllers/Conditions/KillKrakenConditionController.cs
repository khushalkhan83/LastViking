using System;
using Core;
using Game.AI;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;

namespace Game.Objectives.Conditions.Controllers
{
    public class KillKrakenConditionController : BaseConditionController<KillKrakenConditionData, CountConditionDataModel>
    {
        [Inject] public AnimalsModel AnimalsModel { get; private set; }
        [Inject] public FirstKrakenModel FirstKrakenModel { get; private set; }

        private int KrakenProgress => (int)(FirstKrakenModel.Health / FirstKrakenModel.MaxHealth);

        protected override void Subscribe()
        {
            UpdateKrakenHealthBar();
            FirstKrakenModel.OnKrakenHealthChanged += OnKrakenHealthChangedHandler;
        }


        protected override void Unsubscribe()
        {
            FirstKrakenModel.OnKrakenHealthChanged -= OnKrakenHealthChangedHandler;
        }
        private void OnKrakenHealthChangedHandler(float obj) => UpdateKrakenHealthBar();

        private void UpdateKrakenHealthBar() => EventProcessing
            (
                data => data.ConditionID == ConditionID.KillKraken
                , model => model.SetCount((int)FirstKrakenModel.Health)
                , (data, model) => FirstKrakenModel.Health <= 0
            );
    }
}
