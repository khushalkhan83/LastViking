using Core.StateMachine;
using UnityEngine;

namespace Game.StateMachine.Effects
{
    public class SetActivationBehaviorEffect : EffectBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private bool _isActive;
        [SerializeField] private Behaviour[] _behaviours;

#pragma warning restore 0649
        #endregion

        public Behaviour[] Behaviours => _behaviours;
        public bool IsActive => _isActive;

        public override void Apply()
        {
            foreach (var obj in Behaviours)
            {
                obj.enabled = IsActive;
            }
        }
    }
}
