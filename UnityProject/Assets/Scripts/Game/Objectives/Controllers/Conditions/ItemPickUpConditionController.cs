using Core;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;

namespace Game.Objectives.Conditions.Controllers
{
    public class ItemPickUpConditionController : BaseConditionController<PickupItemConditionData, CountConditionDataModel>
    {
        [Inject] public PickUpsModel PickUpsModel { get; private set; }

        protected override void Subscribe()
        {
            PickUpsModel.OnPickUp += OnPickUpHandler;
        }

        protected override void Unsubscribe()
        {
            PickUpsModel.OnPickUp -= OnPickUpHandler;
        }

        private void OnPickUpHandler(string itemName, int count) => EventProcessing
            (
                data => data.ItemName == itemName
                , model => model.Progress(count)
                , (data, model) => data.Value <= model.Value
            );
    }
}
