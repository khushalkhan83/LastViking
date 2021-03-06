using Core;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;
using System.Linq;

namespace Game.Objectives.Conditions.Controllers
{
    public class BuildItemAnyConditionController : BaseConditionController<BuildItemAnyConditionData, CountConditionDataModel>
    {
        [Inject] public PlacementModel PlacementModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }

        protected override void Subscribe()
        {
            PlacementModel.OnPlaceItem += OnPlaceItemHandler;
        }

        protected override void Unsubscribe()
        {
            PlacementModel.OnPlaceItem -= OnPlaceItemHandler;
        }

        private void OnPlaceItemHandler(int itemId) => EventProcessing
            (
                data => data.ItemNames.Contains(ItemsDB.ItemDatabase.GetItemById(itemId).Name)
                , model => model.Progress(1)
                , (data, model) => data.Value <= model.Value
            );
    }
}
