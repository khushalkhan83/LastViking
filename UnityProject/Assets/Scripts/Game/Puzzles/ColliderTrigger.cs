using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UnityEngine;

namespace Game.Puzzles
{
    public abstract class ColliderTrigger : TriggerBase
    {
        [SerializeField] private ColliderTriggerModel colliderTriggerModel = default;

        private int collisionsCount = 0;


        protected override void OnEnable()
        {
            base.OnEnable();
            colliderTriggerModel.OnEnteredTrigger += OnEnteredTrigger;
            colliderTriggerModel.OnExitedTrigger += OnExitedTrigger;
            collisionsCount = 0;
        }

        protected virtual void OnDisable() 
        {
            colliderTriggerModel.OnEnteredTrigger += OnEnteredTrigger;
            colliderTriggerModel.OnExitedTrigger += OnExitedTrigger;
        }

        private void OnEnteredTrigger(Collider other)
        {
            collisionsCount++;
            UseTrigger();
        }

        private void OnExitedTrigger(Collider other)
        {
            collisionsCount--;
            UseTrigger();
        }

        public override void UseTrigger()
        {
            if(useOnce && isUsed)
                return;

            isActive = collisionsCount > 0;
            isUsed = true;
            UpdateTriggerView();
            OnIsActiveChanged?.Invoke();
            OnUse?.Invoke();
        }
    }
}
