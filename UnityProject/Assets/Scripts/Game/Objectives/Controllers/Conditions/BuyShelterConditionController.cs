using Core;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;

namespace Game.Objectives.Conditions.Controllers
{
    public class BuyShelterConditionController : BaseConditionController<BuyShelterConditionData, DoneConditionDataModel>
    {
        [Inject] public SheltersModel SheltersModel { get; private set; }

        protected override void Subscribe()
        {
            SheltersModel.OnBuy += OnBuyHandler;
        }

        protected override void Unsubscribe()
        {
            SheltersModel.OnBuy -= OnBuyHandler;
        }

        private void OnBuyHandler(ShelterModel shelterModel) => EventProcessing
            (
                data => data.ShelterModelID == shelterModel.ShelterID
                , model => model.Progress(true)
                , (data, model) => true
            );
    }
}
