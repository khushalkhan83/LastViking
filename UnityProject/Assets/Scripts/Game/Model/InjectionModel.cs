using System;
using UnityEngine;

namespace Game.Models
{
    public class InjectionModel : MonoBehaviour
    {
        public event Action OnSceneDependenciesInjected;
        public bool SceneDependenciesInjected {get;private set;}
        public void SetSceneDependenciesInjected()
        {
            SceneDependenciesInjected = true;
            OnSceneDependenciesInjected?.Invoke();
        }
    }
}
