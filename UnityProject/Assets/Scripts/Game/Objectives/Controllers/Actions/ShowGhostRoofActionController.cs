using Core;
using Game.Models;
using UnityEngine;
using static Game.Models.TutorialHouseModel;

namespace Game.Objectives.Actions.Controllers
{
    public class ShowGhostRoofActionController : BaseActionController<ShowGhostRoofActionData>
    {
        [Inject] public TutorialHouseModel TutorialHouseModel { get; private set; }

        protected override void Action(ShowGhostRoofActionData actionData)
        {
            TutorialHouseModel.SetNeedBuildPart(TutorialHousePart.Roof , actionData.Show);
        }
    }
}
