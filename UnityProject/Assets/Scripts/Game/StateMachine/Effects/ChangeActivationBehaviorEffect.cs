using Core.StateMachine;
using UnityEngine;

namespace Game.StateMachine.Effects
{
    public class ChangeActivationBehaviorEffect : EffectBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Behaviour[] _behaviours;

#pragma warning restore 0649
        #endregion

        public Behaviour[] Behaviours => _behaviours;

        public override void Apply()
        {
            foreach (var obj in Behaviours)
            {
                obj.enabled = !obj.enabled;
            }
        }
    }
}
