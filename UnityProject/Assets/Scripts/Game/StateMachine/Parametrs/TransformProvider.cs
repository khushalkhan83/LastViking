using Core.States.Parametrs;
using UnityEngine;

namespace Game.StateMachine.Parametrs
{
    public class TransformProvider : ParametrBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Transform _target;

#pragma warning restore 0649
        #endregion

        public Transform Target => _target;

        public void SetTarget(Transform target) => _target = target;
    }
}
