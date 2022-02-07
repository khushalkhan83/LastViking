using Core;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;

namespace Game.Objectives.Conditions.Controllers
{
    public class RepairAnyItemConditionController : BaseConditionController<RepairAnyItemConditionData, CountConditionDataModel>
    {
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public RepairingItemsModel RepairingItemsModel { get; private set; }

        protected override void Subscribe()
        {
            RepairingItemsModel.OnRepairedItem += OnRepairedItem;
        }

        protected override void Unsubscribe()
        {
            RepairingItemsModel.OnRepairedItem -= OnRepairedItem;
        }

        private void OnRepairedItem(UltimateSurvival.SavableItem item) => EventProcessing
            (
                data => true
                , model => model.Progress(1)
                , (data, model) => data.Value <= model.Value
            );
    }
}
