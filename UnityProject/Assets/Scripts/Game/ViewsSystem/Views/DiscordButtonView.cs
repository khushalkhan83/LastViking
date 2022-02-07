using Core.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Views
{
    public class DiscordButtonView : ViewAnimateBase, IPointerClickHandler
    {
        public int TriggerIdIdle => Animator.StringToHash("Idle");
        public int TriggerIdHideImmediatly => Animator.StringToHash("HideImmediatly");

        public event Action OnClick;
        public void OnPointerClick(PointerEventData eventData) => OnClick?.Invoke();

        public void ShowIdle() => Animator.SetTrigger(TriggerIdIdle);
        public void HideImmediatly() => Animator.SetTrigger(TriggerIdHideImmediatly);
    }
}