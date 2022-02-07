using Core;
using Game.Models;
using UnityEngine;
using static Game.Models.TutorialHouseModel;

namespace Game.Objectives.Actions.Controllers
{
    public class ShowGhostFoundationActionController :  BaseActionController<ShowGhostFoundationActionData>
    {
        [Inject] public TutorialHouseModel TutorialHouseModel { get; private set; }

        protected override void Action(ShowGhostFoundationActionData actionData)
        {
            TutorialHouseModel.SetNeedBuildPart(TutorialHousePart.Foundation , actionData.Show);
        }
    }
}
