using Core.Views;
using System;
using TMPro;
using UnityEngine;

namespace Game.Views
{
    interface IRessourceMessageView
    {
        void PlayPulse();
        void PlayShow();

        bool IsTransition {get;}

        void SetCountText(string count);
        void SetIsVisibleCount(bool isVisible);
    }

    interface IHaveFloatingTextPosition
    {
        Vector3 PositionFloatingText {get;}
    }

    public class MessageAppendBlueprintView : ViewBase, IRessourceMessageView, IHaveFloatingTextPosition
    {
        private int TriggerIdPulse => Animator.StringToHash("Pulse");
        private int TriggerIdShow => Animator.StringToHash("Show");

#pragma warning disable 0649

        [SerializeField] private TextMeshProUGUI _valueText;
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _valueObject;
        [SerializeField] public Transform _floatingTextSpawn;

#pragma warning restore 0649
        public Vector3 PositionFloatingText => _floatingTextSpawn.position;
        public bool IsTransition => _animator.IsInTransition(0);

        public void SetCountText(string count) => _valueText.text = count;
        public void SetIsVisibleCount(bool isVisible) => _valueObject.SetActive(isVisible);

        public void PlayPulse() => _animator.SetTrigger(TriggerIdPulse);
        public void PlayShow() => _animator.SetTrigger(TriggerIdShow);

        //Animation
        public event Action<MessageAppendBlueprintView> OnEndHideAnimation;
        public void EndHideAnimation() => OnEndHideAnimation?.Invoke(this);
    }
}
