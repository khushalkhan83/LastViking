using System;
using UnityEngine;

namespace Game.Models
{
    public class ColliderTriggerModel : MonoBehaviour
    {
        public event Action<Collider> OnEnteredTrigger;
        public event Action<Collider> OnExitedTrigger;
        public event Action<Collider> OnStayingTrigger;

        private void OnTriggerEnter(Collider collision) => OnEnteredTrigger?.Invoke(collision);
        private void OnTriggerExit(Collider collision) => OnExitedTrigger?.Invoke(collision);
        private void OnTriggerStay(Collider collision) => OnStayingTrigger?.Invoke(collision);
    }
}
