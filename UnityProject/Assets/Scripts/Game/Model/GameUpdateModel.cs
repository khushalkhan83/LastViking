using System;
using UnityEngine;

namespace Game.Models
{
    public class GameUpdateModel : MonoBehaviour
    {
        public event Action OnUpdate;

        private void Update()
        {
            OnUpdate?.Invoke();
        }
    }
}
