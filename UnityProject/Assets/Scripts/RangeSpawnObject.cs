using System;
using UnityEngine;

namespace Game
{
    public class RangeSpawnObject : MonoBehaviour
    {
        public float SpawnTime { get; set; }

        public event Action<RangeSpawnObject> OnDeath;

        private void OnDisable()
        {
            OnDeath?.Invoke(this);
        }
    }
}
