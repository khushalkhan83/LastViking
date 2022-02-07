using Core;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;

namespace Game.Objectives.Conditions.Controllers
{
    public class RaiseHealthConditionController : BaseConditionController<RaiseHealthConditionData, DoneConditionDataModel>
    {
        [Inject] public PlayerConsumeModel PlayerConsumeModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }

        protected override void Subscribe()
        {
            PlayerHealthModel.OnAddHealth += OnAddHealth;
        }

        protected override void Unsubscribe()
        {
            PlayerHealthModel.OnAddHealth -= OnAddHealth;
        }

        private RaiseHealthConditionData _data = default;
        private void OnAddHealth() => EventProcessing
            (
                data => { _data = data; return true; }
                , model => model.Progress(PlayerHealthModel.HealthCurrent >= _data?.TargetHealth)
                , (data, model) => PlayerHealthModel.HealthCurrent >= data.TargetHealth
            );
    }
}
