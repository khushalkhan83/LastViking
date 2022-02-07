using Core;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;
using static Game.Models.TutorialHouseModel;

namespace Game.Objectives.Conditions.Controllers
{
    public class BuildHouseWallsConditionController :  BaseConditionController<BuildHouseWallsConditionData, DoneConditionDataModel>
    {
        [Inject] public TutorialHouseModel TutorialHouseModel { get; private set; }

        protected override void Subscribe()
        {
            TutorialHouseModel.OnPartBuilded += OnPartBuilded;
        }

        protected override void Unsubscribe()
        {
            TutorialHouseModel.OnPartBuilded -= OnPartBuilded;
        }

        private void OnPartBuilded(TutorialHousePart part) => EventProcessing
            (
                data => part == TutorialHousePart.Walls
                , model => model.Progress(true)
                , (data, model) => part == TutorialHousePart.Walls
            );
    }
}
