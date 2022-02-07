using Core.StateMachine;
using UnityEngine;

namespace Game.StateMachine.Effects
{

    public class DestroyObjectEffect : EffectBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private GameObject _object;
        [SerializeField] private float _time;

#pragma warning restore 0649
        #endregion

        public GameObject Object => _object;
        public float Time => _time;


        public override void Apply()
        {
            Destroy(Object, Time);
        }
    }
}
