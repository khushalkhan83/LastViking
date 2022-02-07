using Core.Views;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views
{
    public class PlaceButtonView : ViewBase
        , IPointerDownHandler
        , IPointerUpHandler
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _image;
        [SerializeField] private Animator _animator;

#pragma warning restore 0649
        #endregion
        public int AnimatorParametrIdDefault { get; } = Animator.StringToHash("Default");
        public int AnimatorParametrIdPulse { get; } = Animator.StringToHash("Pulse");

        public void SetIcon(Sprite sprite) => _image.sprite = sprite;

        public event Action OnPointerDown_;
        public void OnPointerDown(PointerEventData eventData) => OnPointerDown_?.Invoke();

        public event Action OnPointerUp_;
        public void OnPointerUp(PointerEventData eventData) => OnPointerUp_?.Invoke();
        public void PlayDefault() => _animator.SetTrigger(AnimatorParametrIdDefault);
        public void PlayPulse() => _animator.SetTrigger(AnimatorParametrIdPulse);
    }
    
}
