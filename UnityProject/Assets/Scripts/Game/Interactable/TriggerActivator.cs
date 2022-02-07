using Game.AI;
using Game.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Interactables
{
    public class TriggerActivator : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ColliderTriggerModel _activateCollider;
        [SerializeField] private TargetID _activatingTargetId; // Replace 
        [SerializeField] private ObjectsActivator _activator;

#pragma warning restore 0649
        #endregion

        public ColliderTriggerModel ActivateCollider => _activateCollider;
        public TargetID ActivatingTargetId => _activatingTargetId;

        private void OnEnable()
        {
            ActivateCollider.OnEnteredTrigger += OnActivateTrigger;
        }

        private void OnDisable()
        {
            ActivateCollider.OnEnteredTrigger -= OnActivateTrigger;
        }

        private void OnActivateTrigger(Collider col)
        {
            var target = col.GetComponent<Target>();
            if (target && target.ID == ActivatingTargetId)
                _activator.Activate();
        }
    }
}
