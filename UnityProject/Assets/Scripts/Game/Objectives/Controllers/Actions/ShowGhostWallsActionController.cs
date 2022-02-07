using Core;
using Game.Models;
using UnityEngine;
using static Game.Models.TutorialHouseModel;

namespace Game.Objectives.Actions.Controllers
{
    public class ShowGhostWallsActionController : BaseActionController<ShowGhostWallsActionData>
    {
        [Inject] public TutorialHouseModel TutorialHouseModel { get; private set; }

        protected override void Action(ShowGhostWallsActionData actionData)
        {
            TutorialHouseModel.SetNeedBuildPart(TutorialHousePart.Walls , actionData.Show);
        }
    }
}
