using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace Game.Views
{
    public class ZoneMessageView : ViewBase
    {
        [SerializeField] private TextMeshProUGUI messageText = default;
        [SerializeField] private RectTransform messageTransform = default;
        [SerializeField] private CanvasGroup canvasGroup = default;
        [SerializeField] private float showAnimationDuration;
        [SerializeField] private float hideAnimationDuration;

        public float ShowAnimationDuration => showAnimationDuration;
        public float HideAnimationDuration => hideAnimationDuration;

        public void SetMessateText(string text) => messageText.text = text;

        public void PlayShowAnimatoin(Action onEndCallback = null)
        {
            DOTween.Kill(messageTransform);
            DOTween.Kill(canvasGroup);
            canvasGroup.alpha = 1;
            messageTransform.localScale = Vector3.zero;
            messageTransform.DOScale(1, showAnimationDuration).SetEase(Ease.InQuad).OnComplete(() => onEndCallback?.Invoke());
        }

        public void PlayHideAnimation(Action onEndCallback = null)
        {
            canvasGroup.DOFade(0, hideAnimationDuration).OnComplete(() => onEndCallback?.Invoke());
        }
    }
}
