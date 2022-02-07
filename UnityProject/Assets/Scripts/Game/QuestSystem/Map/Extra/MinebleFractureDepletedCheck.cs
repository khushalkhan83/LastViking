using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.QuestSystem.Map.Extra
{
    public class MinebleFractureDepletedCheck : MonoBehaviour
    {
        [SerializeField] private MinebleFractureObject minebleFractureObject;
        [SerializeField] private UnityEvent OnDepleted;

        private void Start() 
        {
            if(minebleFractureObject.ResourceValue == 0)
            {
                OnDepleted?.Invoke();
            }
            else
            {
                minebleFractureObject.Depleted += Depleted;
            }
        }

        private void OnDisable() 
        {
                minebleFractureObject.Depleted += Depleted;
        }

        private void Depleted(RaycastHit hit, Ray ray)
        {
            OnDepleted?.Invoke();
        }
    
    }
}
