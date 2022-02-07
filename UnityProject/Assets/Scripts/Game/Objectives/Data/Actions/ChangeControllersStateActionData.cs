using Game.Controllers;
using UnityEngine;

namespace Game.Objectives.Actions
{
    public class ChangeControllersStateActionData : ActionBaseData
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ControllersStateID _controllersStateID;

#pragma warning restore 0649
        #endregion

        public ControllersStateID ControllersStateID => _controllersStateID;

        public override ActionID ActionID { get; } = ActionID.ChangeControllersState;
    }
}
