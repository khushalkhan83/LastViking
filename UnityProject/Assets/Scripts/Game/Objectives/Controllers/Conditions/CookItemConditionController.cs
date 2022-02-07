using Core;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;

namespace Game.Objectives.Conditions.Controllers
{
    public class CookItemConditionController : BaseConditionController<CookItemConditionData, CountConditionDataModel>
    {
        [Inject] public CampFiresModel CampFiresModel { get; private set; }

        protected override void Subscribe()
        {
            CampFiresModel.OnCookItem += OnCookItemHandler;
        }

        protected override void Unsubscribe()
        {
            CampFiresModel.OnCookItem -= OnCookItemHandler;
        }

        private void OnCookItemHandler(string itemName, int count) => EventProcessing
            (
                data => data.ItemName == itemName
                , model => model.Progress(count)
                , (data, model) => data.Value <= model.Value
            );
    }
}
