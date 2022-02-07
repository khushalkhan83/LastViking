using Core;
using Game.Models;
using UnityEngine;

namespace Game.Objectives.Actions.Controllers
{
    public class ApplyModificatorActionController : BaseActionController<ApplyModificatorActionData>
    {
        [Inject] public ControllersModel ControllersModel { get; private set; }

        protected override void Action(ApplyModificatorActionData actionData)
        {
            Debug.Log("<color=orange>Activate " + actionData + "</color>");
            ControllersModel.ApplyModificator(actionData.Modificator);
        }
    }
}
