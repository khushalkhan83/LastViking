using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Puzzles
{
    public abstract class TriggeredMechanismBase : MonoBehaviour
    {
        [SerializeField] protected TriggerSetting[] triggerSettings;

        public abstract bool Activated
        {
            get;
            protected set;
        }
        public TriggerSetting[] TriggerSettings => triggerSettings;

        protected virtual void OnEnable() 
        {
            foreach(var triggerSetting in triggerSettings)
            {
                triggerSetting.trigger.OnIsActiveChanged += UpdateMechanismState;
            }
            UpdateMechanismState();
        }

        protected virtual void OnDisable() 
        {
            foreach(var triggerSetting in triggerSettings)
            {
                triggerSetting.trigger.OnIsActiveChanged -= UpdateMechanismState;
            }
        }

        protected virtual void UpdateMechanismState()
        {
            Activated = CheckIsActive();
            UpdateMechanismView();
        }

        protected virtual bool CheckIsActive()
        {
            foreach(var triggerSetting in triggerSettings)
            {
                if(triggerSetting.trigger.IsActive != triggerSetting.neededActiveState)
                {
                    return false;
                }
            }
            return true;
        }

        protected abstract void UpdateMechanismView();

        [Serializable]
        public class TriggerSetting
        {
            public TriggerBase trigger;
            public bool neededActiveState;
        }
    }
}