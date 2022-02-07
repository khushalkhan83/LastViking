using System;
using UnityEngine;

namespace Game.Models
{
    public class OutLineCoconutModel : MonoBehaviour
    {
        public event Action OnSelect;
        public event Action OnDeselect;

        public bool IsSelect { get; private set; }

        public void Select()
        {
            if (!IsSelect)
            {
                IsSelect = true;
                OnSelect?.Invoke();
            }
        }

        public void Deselect()
        {
            if (IsSelect)
            {
                IsSelect = false;
                OnDeselect?.Invoke();
            }
        }
    }
}
