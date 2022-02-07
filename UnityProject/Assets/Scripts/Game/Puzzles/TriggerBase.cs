using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Puzzles
{
    public abstract class TriggerBase : MonoBehaviour
    {
        [SerializeField] protected bool isActive = false;
        [SerializeField] protected bool useOnce = false;

        protected bool isUsed = false;

        public UnityEvent OnUse;
        public Action OnIsActiveChanged;

        public bool IsActive => isActive;

        protected virtual void OnEnable() 
        {
            UpdateTriggerView();
        }
        
        public virtual void UseTrigger()
        {
            if(useOnce && isUsed)
                return;

            isActive = !isActive;
            isUsed = true;
            UpdateTriggerView();
            OnIsActiveChanged?.Invoke();
            OnUse?.Invoke();
        }

        protected abstract void UpdateTriggerView();
    }
}
