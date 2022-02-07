using Core;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;
using UltimateSurvival;

namespace Game.Objectives.Conditions.Controllers
{
    public class ResourceCollectConditionController : BaseConditionController<ResourceCollectConditionData, CountConditionDataModel>
    {
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public ItemsOwnershipModel ItemsOwnershipModel { get; private set; }

        protected override void Subscribe()
        {
            ItemsOwnershipModel.OnItemOwnedByPlayer += OnItemOwnedByPlayer;
        }

        protected override void Unsubscribe()
        {
            ItemsOwnershipModel.OnItemOwnedByPlayer -= OnItemOwnedByPlayer;
        }

        private void OnItemOwnedByPlayer(int itemId, int count) => EventProcessing
            (
                data => ItemsDB.ItemDatabase.GetItemByName(data.ResourceName).Id == itemId
                , model => model.Progress(count)
                , (data, model) => data.Value <= model.Value
            );
    }
}
