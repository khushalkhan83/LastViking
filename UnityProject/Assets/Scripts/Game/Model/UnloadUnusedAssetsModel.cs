using System;
using UnityEngine;

namespace Game.Models
{
    public class UnloadUnusedAssetsModel : MonoBehaviour
    {
        public event Action OnUnusedSceneAssetsUnloaded;

        public void SetUnusedSceneAssetsUnloaded()
        {
            OnUnusedSceneAssetsUnloaded?.Invoke();
        }        
    }
}
