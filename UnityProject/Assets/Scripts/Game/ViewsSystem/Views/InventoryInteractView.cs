using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class InventoryInteractView : ViewBase
    {
        public int AnimatorParametrIdPulse { get; } = Animator.StringToHash("Pulse");
        public int AnimatorParametrIdDefault { get; } = Animator.StringToHash("Default");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Button _button;
        [SerializeField] private Animator _animator;

#pragma warning restore 0649
        #endregion

        public void SetIsInteractable(bool value) => _button.interactable = value;
        public void PlayPulse() => _animator.SetTrigger(AnimatorParametrIdPulse);
        public void PlayDefault() => _animator.SetTrigger(AnimatorParametrIdDefault);

        public GameObject Button => _button.gameObject;

        //UI
        public event Action OnClick;
        public void OpenInventoryView() => OnClick?.Invoke();
    }
}
