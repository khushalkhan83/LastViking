using Game.AI;
using Game.Models;
using UnityEngine;

namespace Game.StateMachine.Parametrs
{
    public class TargetProvider : TargetBase
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private TargetID _targetID = TargetID.Player;
#pragma warning restore 0649
        #endregion

        private TargetID TargetID => _targetID;

        public override Target Target
        {
            get
            {
                switch (TargetID)
                {
                    case TargetID.Player:
                        return ModelsSystem.Instance._playerAsTarget;
                    case TargetID.Main:
                        return ModelsSystem.Instance.shelterTarget;
                    default:
                        "Cant provide target with this id".Error();
                        return null;
                }
            }
        }

        public void SetTarget(Target target)
        {
            Debug.LogError("Not supported now");
        }
    }
}
