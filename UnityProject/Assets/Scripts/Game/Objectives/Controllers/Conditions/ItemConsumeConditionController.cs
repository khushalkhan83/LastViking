using Core;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;

namespace Game.Objectives.Conditions.Controllers
{
    public class ItemConsumeConditionController : BaseConditionController<ConsumeItemConditionData, CountConditionDataModel>
    {
        [Inject] public PlayerConsumeModel PlayerConsumeModel { get; private set; }

        protected override void Subscribe()
        {
            PlayerConsumeModel.OnStartConsumeProcess += OnConsumeHandler;
        }


        protected override void Unsubscribe()
        {
            PlayerConsumeModel.OnStartConsumeProcess -= OnConsumeHandler;
        }

        private void OnConsumeHandler() => EventProcessing
            (
                data => data.ItemName == PlayerConsumeModel.Item.Name
                , model => model.Progress(1)
                , (data, model) => data.Value <= model.Value
            );
    }
}
