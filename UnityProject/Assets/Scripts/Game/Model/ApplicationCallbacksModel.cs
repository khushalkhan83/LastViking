using System;
using UnityEditor;
using UnityEngine;

namespace Game.Models
{
    public class ApplicationCallbacksModel : MonoBehaviour
    {
        public event Action ApplicationQuit;
        public event Action<bool> ApplicationFocus;
        public event Action<bool> ApplicationPause;

        public bool IsApplicationQuitting {get;private set;}

        private void OnApplicationQuit() 
        {
            IsApplicationQuitting = true;
            ApplicationQuit?.Invoke();
        }

        private void OnApplicationFocus(bool hasFoculs) 
        {
            ApplicationFocus?.Invoke(hasFoculs);
        }

        private void OnApplicationPause(bool isPause) 
        {
            ApplicationPause?.Invoke(isPause);
        }
    }
}
