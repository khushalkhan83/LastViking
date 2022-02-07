using Core;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;

namespace Game.Objectives.Conditions.Controllers
{
    public class RepairItemConditionController : BaseConditionController<RepairItemConditionData, CountConditionDataModel>
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
                data => ItemsDB.ItemDatabase.GetItemByName(data.ItemName).Id == item.Id
                , model => model.Progress(1)
                , (data, model) => data.Value <= model.Value
            );
    }
}
