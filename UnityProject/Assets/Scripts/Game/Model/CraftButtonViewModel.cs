using System;
using UnityEngine;

namespace Game.Models
{
    public class CraftButtonViewModel : MonoBehaviour
    {
        public event Func<GameObject> OnGetButton;
        public GameObject GetButton()
        {
            return OnGetButton?.Invoke();
        }
    }
}
