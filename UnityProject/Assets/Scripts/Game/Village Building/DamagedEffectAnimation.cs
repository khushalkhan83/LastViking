using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using NaughtyAttributes;

namespace Game.VillageBuilding
{
    public class DamagedEffectAnimation : MonoBehaviour
    {
        [SerializeField] private Transform animatedTransform = default;
        [SerializeField] private float showAniamtionDuration = 1f;
        [SerializeField] private  float shakeStrength = 1;
        [SerializeField] private  int vibrato = 10;
        [SerializeField] private  float randomness = 90;
        [SerializeField] private  bool fadeOut = true;

        private Tween tween;

        [Button]
        public void PlayAnimation()
        {
            if(tween != null)
                tween.Kill();

            tween = animatedTransform.DOShakeScale(showAniamtionDuration,shakeStrength,vibrato,randomness,fadeOut);
        }
    }
}