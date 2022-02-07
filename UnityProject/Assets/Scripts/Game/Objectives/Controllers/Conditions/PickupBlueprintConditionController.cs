using Core;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;
using Game.Purchases;

namespace Game.Objectives.Conditions.Controllers
{
    public class PickupBlueprintConditionController : BaseConditionController<PickupBlueprintConditionData, CountConditionDataModel>
    {
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public BluePrintsModel BluePrintsModel { get; private set; }

        protected override void Subscribe()
        {
            BluePrintsModel.OnAdd += OnAddBlueprints;
        }

        protected override void Unsubscribe()
        {
            BluePrintsModel.OnAdd -= OnAddBlueprints;
        }

        private void OnAddBlueprints(int count) => EventProcessing
            (
                data => true
                , model => model.Progress(count)
                , (data, model) => data.Value <= model.Value
            );
    }
}
