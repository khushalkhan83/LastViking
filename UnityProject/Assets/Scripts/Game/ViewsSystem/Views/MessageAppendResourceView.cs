using Core.Views;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class MessageAppendResourceView : ViewBase
    {
        public int TriggerIdPulse => Animator.StringToHash("Pulse");
        public int TriggerIdPulseBonus => Animator.StringToHash("PulseBonus");
        public int TriggerIdShow => Animator.StringToHash("Show");
        public int TriggerIdShowBonus => Animator.StringToHash("ShowBonus");

#pragma warning disable 0649

        [VisibleObject]
        [SerializeField] private GameObject _valueObject;
        [SerializeField] private TextMeshProUGUI _valueText;
        [SerializeField] private Image _itemIconRenderer;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _floatingTextSpawn;

#pragma warning restore 0649

        public Vector3 PositionFloatingText => _floatingTextSpawn.position;
        public bool IsTransition => _animator.IsInTransition(0);

        public void SetIcon(Sprite sprite) => _itemIconRenderer.sprite = sprite;
        public void SetCountText(string count) => _valueText.text = count;
        public void SetIsVisibleCount(bool isVisible) => _valueObject.SetActive(isVisible);

        public void PlayPulse() => _animator.SetTrigger(TriggerIdPulse);
        public void PlayPulseBonus() => _animator.SetTrigger(TriggerIdPulseBonus);
        public void PlayShow() => _animator.SetTrigger(TriggerIdShow);
        public void PlayShowBonus() => _animator.SetTrigger(TriggerIdShowBonus);

        //Animation
        public event Action<MessageAppendResourceView> OnEndHideAnimation;
        public void EndHideAnimation() => OnEndHideAnimation?.Invoke(this);
    }
}
