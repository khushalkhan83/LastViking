using Core.StateMachine;
using UnityEngine;

namespace Game.StateMachine.Effects
{
    public class SetColliderEffect : EffectBase
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private bool _setActive;
        [SerializeField] private Collider[] _colliders;

#pragma warning restore 0649
        #endregion

        public Collider[] Colliders => _colliders;

        public override void Apply()
        {
            foreach (var obj in Colliders)
            {
                obj.enabled = _setActive;
            }
        }
    }
}
