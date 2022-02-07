using UnityEngine;
using System;

namespace Game.Models
{
    public class FirebaseModel : MonoBehaviour
    {
        public bool IsFirebaseReady { get; private set; }

        public event Action OnFirebaseReady;

        public void SetFirebaseReady()
        {
            print(" ------ Firebase available");

            IsFirebaseReady = true;
            OnFirebaseReady?.Invoke();
        }
    }
}
