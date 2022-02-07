using Core;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;

namespace Game.Objectives.Conditions.Controllers
{
    public class UpgradeShelterConditionController : BaseConditionController<UpgradeShelterConditionData, DoneConditionDataModel>
    {
        [Inject] public SheltersModel SheltersModel { get; private set; }

        protected override void Subscribe()
        {
            SheltersModel.OnUpgrade += OnUpgradeHandler;
        }

        protected override void Unsubscribe()
        {
            SheltersModel.OnUpgrade -= OnUpgradeHandler;
        }

        private void OnUpgradeHandler(ShelterModel shelterModel) => EventProcessing
            (
                data => data.ShelterModelID == shelterModel.ShelterID
                , model => model.Progress(true)
                , (data, model) => true
            );
    }
}
