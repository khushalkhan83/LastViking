using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class CraftInteractView : ViewBase
    {
        public int AnimatorParametrIdRotateGear { get; } = Animator.StringToHash("RotateGear");
        public int AnimatorParametrIdDefault { get; } = Animator.StringToHash("Default");
        public int AnimatorParametrIdPulse { get; } = Animator.StringToHash("Pulse");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Button _button;
        [SerializeField] private GameObject _craftGear;
        [SerializeField] private Text _count_text;
        [SerializeField] private Animator _animator;

#pragma warning restore 0649
        #endregion

        public void SetIsInteractable(bool isInteractable) => _button.interactable = isInteractable;
        public void SetTextCount(string text) => _count_text.text = text;
        public void SetIsVisibleGear(bool isVisible) => _craftGear.SetActive(isVisible);
        public void PlayRotateGear() => _animator.SetTrigger(AnimatorParametrIdRotateGear);
        public void PlayDefault() => _animator.SetTrigger(AnimatorParametrIdDefault);
        public void PlayPulse() => _animator.SetTrigger(AnimatorParametrIdPulse);

        public GameObject Button => _button.gameObject;

        //UI
        public event Action OnClick;
        public void ActionClick() => OnClick?.Invoke();
    }
}
