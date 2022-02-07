using Core.StateMachine;
using UnityEngine;

namespace Game.StateMachine.Effects
{
    public class TransitionToStateEffect : EffectBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private StateProcessor _stateProcessor;
        [SerializeField] private State _state;

#pragma warning restore 0649
        #endregion

        public StateProcessor StateProcessor => _stateProcessor;
        public State State => _state;

        public override void Apply()
        {
            StateProcessor.TransitionTo(State);
        }
    }
}
