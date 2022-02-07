using Core;
using Game.Models;
using UnityEngine;

namespace Game.Objectives.Actions.Controllers
{
    public class ShowGhostDockActionDataController : BaseActionController<ShowGhostDockActionData>
    {
        [Inject] public ConstructionDockModel ConstructionDockModel { get; private set; }

        protected override void Action(ShowGhostDockActionData actionData)
        {
            ConstructionDockModel.NeedBuildDock = actionData.Show;
        }
    }
}