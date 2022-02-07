using Core.StateMachine;
using System.Collections;
using UnityEngine;

namespace Game.StateMachine.Effects
{
    public class DestroyRootAfterDelayEffect : EffectBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private float _duration;
        [SerializeField] private GameObject _root;

#pragma warning restore 0649
        #endregion

        public float Duration => _duration;
        public GameObject Root => _root;

        public override void Apply()
        {
            StartCoroutine(DestroyRoot(Duration));
        }

        private IEnumerator DestroyRoot(float duration)
        {
            yield return new WaitForSeconds(duration);
            Destroy(Root);
        }
    }
}
