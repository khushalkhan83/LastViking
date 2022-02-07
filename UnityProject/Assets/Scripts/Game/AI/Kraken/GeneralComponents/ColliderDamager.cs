using System;
using Game.StateMachine.Events;
using UnityEngine;
using Extensions;

namespace Game.AI.Behaviours.Kraken
{
    public class ColliderDamager : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private LayerMask _activatorLayer;
        [SerializeField] private Hit _hit;

#pragma warning restore 0649
        #endregion

        private Collider _collider;
        private bool _active;

        public event Action<Target, Collider> OnTargetDamaged;

        #region MonoBehaviour
        private void Start()
        {
            if (_collider == null)
                _collider = GetComponent<Collider>();
        }

        private void OnEnable()
        {
            _hit.OnTargetDamaged += OnDamagedHandler;
        }

        private void OnDisable()
        {
            _hit.OnTargetDamaged -= OnDamagedHandler;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_active) return;

            if (!other.gameObject.InsideLayerMask(_activatorLayer)) return;

            _hit.DoDamage();
            _active = false;
        }

        #endregion

        public void SetActive(bool active) => _active = active;

        private void OnDamagedHandler(Target target) => OnTargetDamaged?.Invoke(target, _collider);

    }
}

