using System;
using UnityEngine;

namespace Core.StateMachine
{
    [Serializable]
    public class Interrupt
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ConditionBase[] _conditions;
        [SerializeField] private State _state;
        [SerializeField] private ushort _priority;
        [SerializeField] private EffectBase[] _effects;

#pragma warning restore 0649
        #endregion

        public State State => _state;
        public ConditionBase[] Conditions => _conditions;
        public EffectBase[] Effects => _effects;
        public ushort Priority => _priority;

        public void SetState(State state) => _state = state;

        public void SetEffects(EffectBase[] effects) => _effects = effects;
    }
}
