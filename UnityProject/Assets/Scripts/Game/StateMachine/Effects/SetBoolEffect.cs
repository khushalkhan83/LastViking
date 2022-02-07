using Core.StateMachine;
using Core.States.Parametrs;
using UnityEngine;

namespace Game.StateMachine.Effects
{
    public class SetBoolEffect : EffectBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private BoolParametr _boolParametr;
        [SerializeField] private bool _value;

#pragma warning restore 0649
        #endregion

        public BoolParametr Parametr => _boolParametr;
        public bool Value => _value;

        public override void Apply()
        {
            Parametr.SetValue(Value);
        }
    }
}
