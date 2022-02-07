using Core;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;

namespace Game.Objectives.Conditions.Controllers
{
    public class RaiseWaterConditionController : BaseConditionController<RaiseWaterConditionData, DoneConditionDataModel>
    {
        [Inject] public PlayerConsumeModel PlayerConsumeModel { get; private set; }
        [Inject] public PlayerWaterModel PlayerWaterModel { get; private set; }

        protected override void Subscribe()
        {
            PlayerWaterModel.OnAddWater += OnAddWater;
        }

        protected override void Unsubscribe()
        {
            PlayerWaterModel.OnAddWater -= OnAddWater;
        }

        private RaiseWaterConditionData _data = default;
        private void OnAddWater() => EventProcessing
            (
                data => { _data = data; return true; }
                , model => model.Progress(PlayerWaterModel.WaterCurrent >= _data?.TargetWater)
                , (data, model) => PlayerWaterModel.WaterCurrent >= data.TargetWater
            );
    }
}
