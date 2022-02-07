using Core;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;
using Game.Views;
using static Game.Models.TutorialHouseModel;

namespace Game.Objectives.Conditions.Controllers
{
    public class BuildHouseFoundationConditionController : BaseConditionController<BuildHouseFoundationConditionData, DoneConditionDataModel>
    {
        [Inject] public TutorialHouseModel TutorialHouseModel { get; private set; }

        protected override void Subscribe()
        {
            TutorialHouseModel.OnPartBuilded += OnHouseBuilded;
        }

        protected override void Unsubscribe()
        {
            TutorialHouseModel.OnPartBuilded -= OnHouseBuilded;
        }

        private void OnHouseBuilded(TutorialHousePart part) => EventProcessing
            (
                data => part == TutorialHousePart.Foundation
                , model => model.Progress(true)
                , (data, model) => part == TutorialHousePart.Foundation
            );
    }
}
