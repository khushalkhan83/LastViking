using Core;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;

namespace Game.Objectives.Conditions.Controllers
{
    public class CraftItemConditionController : BaseConditionController<CraftItemConditionData, CountConditionDataModel>
    {
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public CraftModel CraftModel { get; private set; }

        protected override void Subscribe()
        {
            CraftModel.OnCraftedItem += OnCraftedItemHandler;
        }

        protected override void Unsubscribe()
        {
            CraftModel.OnCraftedItem -= OnCraftedItemHandler;
        }

        private void OnCraftedItemHandler(int itemId) => EventProcessing
            (
                data => ItemsDB.ItemDatabase.GetItemById(itemId).Name == data.ItemName
                , model => model.Progress(ItemsDB.GetItem(itemId).Recipe.CraftCount)
                , (data, model) => data.Value <= model.Value
            );
    }
}
