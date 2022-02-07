using System;
using UnityEngine;

namespace Game.Models
{
    public class TutorialSimpleDarkViewModel : MonoBehaviour
    {
        public bool Show {get; private set;}
        public event Action OnShowChanged;
        public void SetShow(bool value)
        {
            Show = value;
            OnShowChanged?.Invoke();
        }

        public event Action OnPlayAnimation;

        public void PlayAnimation()
        {
            OnPlayAnimation?.Invoke();
        }
    }
}