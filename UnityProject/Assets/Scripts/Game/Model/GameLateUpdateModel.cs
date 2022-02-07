using System;
using UnityEngine;

namespace Game.Models
{
    public class GameLateUpdateModel : MonoBehaviour
    {
        public event Action OnLaterUpdate;

        private void LateUpdate()
        {
            OnLaterUpdate?.Invoke();
        }
    }
}
