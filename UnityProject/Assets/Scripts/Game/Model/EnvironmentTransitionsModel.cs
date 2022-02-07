using System;
using UnityEngine;

namespace Game.Models
{
    public class EnvironmentTransitionsModel : MonoBehaviour
    {
        public event Func<bool> OnGetCanGoToOtherLocation;
        public bool CanGoToOtherLocation => OnGetCanGoToOtherLocation.Invoke();

    }
}
