using Core.Views;
using Game.Models;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views
{
    public class AttackButtonView : ViewAnimateBase
        , IPointerDownHandler
        , IPointerUpHandler
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _image;

        [SerializeField] private Sprite _default;
        [SerializeField] private Sprite _active;

#pragma warning restore 0649
        #endregion
        private int animatorParametrIdDefault = Animator.StringToHash("Default");
        private int animatorParametrIdPulse = Animator.StringToHash("Pulse");

        public Sprite IconDefault => _default;
        public Sprite IconActive => _active;


        public void SetIcon(Sprite sprite) => _image.sprite = sprite;

        //UI
        public event Action OnPointerDown_;
        public void OnPointerDown(PointerEventData eventData) => OnPointerDown_?.Invoke();

        //UI
        public event Action OnPointerUp_;
        public void OnPointerUp(PointerEventData eventData) => OnPointerUp_?.Invoke();

        public void PlayDefault() => Animator.SetTrigger(animatorParametrIdDefault);
        public void PlayPulse() => Animator.SetTrigger(animatorParametrIdPulse);

    }
}
