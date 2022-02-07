using Core.Views;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views
{
    public class FishingBitingView : ViewBase
        , IPointerDownHandler
    {

#pragma warning disable 0649

        [SerializeField] private GameObject _biteMessage;
        [SerializeField] private Animator _animator;

#pragma warning restore 0649

        public void SetBiting(bool isBiting)
        {
            if(isBiting)
            {
                _animator.SetTrigger("Biting");
            }
            else
            {
                _animator.SetTrigger("Idle");
            }

        }

        public event Action OnPointerDown_;
        public void OnPointerDown(PointerEventData eventData) => OnPointerDown_?.Invoke();
    }
}
