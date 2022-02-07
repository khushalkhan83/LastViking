using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.ObjectPooling
{
    public class ObjectResetter : MonoBehaviour
    {
        [SerializeField] private UnityEvent onReset;
        private IResettable[] resettables;
        private void Awake() 
        {
            resettables = GetComponentsInChildren<IResettable>(true);    
        }

        private void OnEnable() 
        {
            if(resettables != null)
            {
                foreach(var resettable in resettables)
                {
                    resettable.ResetObject();
                }
            }
            onReset?.Invoke();
        }
    }
}
