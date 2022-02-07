using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class MessageAttentionResourceView : ViewBase
    {
        public int TriggerIdPulse => Animator.StringToHash("Pulse");
        public int TriggerIdShow => Animator.StringToHash("Show");

#pragma warning disable 0649

        [SerializeField] private Image _itemIconRenderer;
        [SerializeField] private Animator _animator;

#pragma warning restore 0649

        public void SetIcon(Sprite sprite) => _itemIconRenderer.sprite = sprite;

        public void PlayPulse() => _animator.SetTrigger(TriggerIdPulse);
        public void PlayShow() => _animator.SetTrigger(TriggerIdShow);

        //Animation
        public event Action<MessageAttentionResourceView> OnEndHideAnimation;
        public void EndHideAnimation() => OnEndHideAnimation?.Invoke(this);
    }
}
