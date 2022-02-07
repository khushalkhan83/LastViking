using System;
using NaughtyAttributes;
using UnityEngine;

namespace Game.Models
{
    public class ActivitiesLogViewModel : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private Sprite nextUpdateIcon;
        #pragma warning restore 0649
        #endregion

        [Button] void Show() => SetShow(true);
        [Button] void Hide() => SetShow(false);


        public bool IsShow { get; private set; }
        public event Action OnShowChanged;

        public void SetShow(bool value)
        {
            IsShow = value;
            OnShowChanged?.Invoke();
        }

        public void SwitchShow() => SetShow(!IsShow);

        public Sprite NextUpdateIcon => nextUpdateIcon;
    }
}
