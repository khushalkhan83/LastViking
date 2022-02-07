using Core;
using Game.Models;
using UnityEngine;
using static Game.Models.TutorialHouseModel;

namespace Game.Objectives.Actions.Controllers
{
    public class ShowGhostDoorActionController : BaseActionController<ShowGhostDoorActionData>
    {
        [Inject] public TutorialHouseModel TutorialHouseModel { get; private set; }

        protected override void Action(ShowGhostDoorActionData actionData)
        {
            TutorialHouseModel.SetNeedBuildPart(TutorialHousePart.Door , actionData.Show);
        }
    }
}
