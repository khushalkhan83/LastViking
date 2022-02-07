using Game.Controllers.Controllers.States.Modificators;
using UnityEngine;

namespace Game.Objectives.Actions
{
    [CreateAssetMenu(fileName = "ApplyModificatorAction", menuName = "Actions/ApplyModificatorActionData", order = 0)]
    public class ApplyModificatorActionData: ActionBaseData
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ModificatorBase _modificator;

#pragma warning restore 0649
        #endregion

        public ModificatorBase Modificator => _modificator;

        public override ActionID ActionID { get; } = ActionID.ApplyModificator;
    }
}
