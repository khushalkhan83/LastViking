using Core;
using Game.Models;
using UnityEngine;

namespace Game.Objectives.Actions.Controllers
{
    public class ChangeControllersStateActionDataController : BaseActionController<ChangeControllersStateActionData>
    {
        [Inject] public ControllersModel ControllersModel { get; private set; }

        protected override void Action(ChangeControllersStateActionData actionData)
        {
            Debug.Log("<color=red>Activate " + actionData + "</color>");
            ControllersModel.ApplyState(actionData.ControllersStateID);
        }
    }
}
