using CodeStage.AntiCheat.ObscuredTypes;
using Core.Views;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views
{
    public class MessageCraftResourceView : ViewBase
    {
        public int TriggerIdPulse => Animator.StringToHash("Pulse");
        public int TriggerIdShow => Animator.StringToHash("Show");

#pragma warning disable 0649

        [VisibleObject]
        [SerializeField] private GameObject _valueObject;
        [SerializeField] private TextMeshProUGUI _valueText;
        [SerializeField] private Image _itemIconRenderer;
        [SerializeField] public Transform _floatingTextSpawn;
        [SerializeField] private Animator _animator;

#pragma warning restore 0649

        public Vector3 PositionFloatingText => _floatingTextSpawn.position;
        public bool IsTransition => _animator.IsInTransition(0);

        public void SetIcon(Sprite sprite) => _itemIconRenderer.sprite = sprite;
        public void SetCountText(string count) => _valueText.text = count;
        public void SetIsVisibleCount(bool isVisible) => _valueObject.SetActive(isVisible);


        public void PlayPulse() => _animator.SetTrigger(TriggerIdPulse);
        public void PlayShow() => _animator.SetTrigger(TriggerIdShow);

        //Animation
        public event Action<MessageCraftResourceView> OnEndHideAnimation;
        public void EndHideAnimation() => OnEndHideAnimation?.Invoke(this);

    }
}
