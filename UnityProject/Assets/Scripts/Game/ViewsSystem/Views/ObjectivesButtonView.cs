using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class ObjectivesButtonView : ViewBase
    {
        public int AnimatorParametrIdDefault { get; } = Animator.StringToHash("Default");
        public int AnimatorParametrIdPulse { get; } = Animator.StringToHash("Pulse");

        #region Data
#pragma warning disable 0649

        [SerializeField] private GameObject _attention;
        [SerializeField] private Button _button;
        [SerializeField] private Animator _animator;

#pragma warning restore 0649
        #endregion

        public void SetIsInteractable(bool isInteractable) => _button.interactable = isInteractable;
        public void SetAttentionIconVisible(bool isVisible) => _attention.SetActive(isVisible);
        public void PlayDefault() => _animator.SetTrigger(AnimatorParametrIdDefault);
        public void PlayPulse() => _animator.SetTrigger(AnimatorParametrIdPulse);

        public event Action OnClick;
        public void ActionClick() => OnClick?.Invoke();
    }
}
