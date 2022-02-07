using System;
using UnityEngine;

namespace Game.Models
{
    public class TeleporHomeModel : MonoBehaviour
    {
        [SerializeField] private float showDelay = 5f;

        public event Action OnShowPopup;

        public  float ShowDelay => showDelay;

        public void ShowPopup()
        {
            OnShowPopup?.Invoke();
        }

    }
}
