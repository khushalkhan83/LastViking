using Core;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;

namespace Game.Objectives.Conditions.Controllers
{
    public class RaiseFoodConditionController : BaseConditionController<RaiseFoodConditionData, DoneConditionDataModel>
    {
        [Inject] public PlayerConsumeModel PlayerConsumeModel { get; private set; }
        [Inject] public PlayerFoodModel PlayerFoodModel { get; private set; }

        protected override void Subscribe()
        {
            PlayerFoodModel.OnAddFood += OnAddFood;
        }

        protected override void Unsubscribe()
        {
            PlayerFoodModel.OnAddFood -= OnAddFood;
        }

        private RaiseFoodConditionData _data = default;
        private void OnAddFood() => EventProcessing
            (
                data => { _data = data; return true; }
                , model => model.Progress(PlayerFoodModel.FoodCurrent >= _data?.TargetFood)
                , (data, model) => PlayerFoodModel.FoodCurrent >= data.TargetFood
            );
    }
}
