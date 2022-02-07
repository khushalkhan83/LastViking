using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Core.StateMachine.Operations.Effects
{
    public class AgregatorEffects : EffectBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private EffectBase[] _effects;

#pragma warning restore 0649
        #endregion

        public IEnumerable<EffectBase> Effects => _effects;

        public override void Apply() => Effects.Apply();
    }
}
