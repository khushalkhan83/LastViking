using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Game.Views
{
    public class BuildingTokenView : ViewBase
    {
        [SerializeField] private Image icon = default;
        [SerializeField] private Vector2 startIconPosition = default;
        [SerializeField] private Vector2 upIconPosition = default;
        [SerializeField] private float animationDuration = 1.5f;
        [SerializeField] private Ease animationEase = default;
        [SerializeField] private Color unlockedColor = default;
        [SerializeField] private Color lockedColor = default;

        private Tween tween;

        public void SetIcon(Sprite iconSprite) => icon.sprite = iconSprite;
        public void LookAt(Vector3 position, Vector3 up) => transform.LookAt(position, up);
        public void SetLocalPosition(Vector3 position) => transform.localPosition = position;
        public void SetScale(Vector3 scale) => transform.localScale = scale;

        public void PlayBouncingAnimation()
        {
            StopBouncingAnimation();
            icon.rectTransform.anchoredPosition = startIconPosition;
            tween = icon.rectTransform.DOAnchorPosY(upIconPosition.y, animationDuration).SetEase(animationEase).SetLoops(-1, LoopType.Yoyo);
        }

        public void StopBouncingAnimation()
        {
            if(tween != null)
            {
                tween.Kill();
                tween = null;
            }
        }

        public void SetIsUnlockedColor(bool isUnlocked)
        {
            icon.color = isUnlocked ? unlockedColor : lockedColor;
        }

    }
}
