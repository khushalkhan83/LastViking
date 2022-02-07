using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace Game.VillageBuilding
{
    public class HouseViewAnimation : MonoBehaviour
    {
        [SerializeField] private Transform animatedTransform = default;
        [SerializeField] private Vector3 hidenPosition = default;
        [SerializeField] private Vector3 showedPosition = default;
        [SerializeField] private float showAniamtionDuration = 1f;
        [SerializeField] private float hideAniamtionDuration = 1f;

        private Tween tween;

        public void SetShowedPosition()
        {
            animatedTransform.localPosition = showedPosition;
        }

        public void SetHidenPosition()
        {
            animatedTransform.localPosition = hidenPosition;
        }

        public void PlayerShowAnimantion(Action callback = null)
        {
            if(tween != null)
                tween.Kill();

            SetHidenPosition();
            tween = animatedTransform.DOLocalMove(showedPosition, showAniamtionDuration).OnComplete(() => callback?.Invoke());
        }

        public void PlayerHideAnimation(Action callback = null)
        {
            if(tween != null)
                tween.Kill();

            SetShowedPosition();
            tween = animatedTransform.DOLocalMove(hidenPosition, showAniamtionDuration).OnComplete(() => callback?.Invoke());
        }
    }
}
