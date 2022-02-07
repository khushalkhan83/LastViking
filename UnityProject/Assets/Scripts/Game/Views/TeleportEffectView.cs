using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Game.Views
{
    public class TeleportEffectView : ViewBase
    {
        [SerializeField] private Image backgroundImage = default;
        [SerializeField] private float fadeTime = 0.2f;
        [SerializeField] private float waitTime = 1f;

        public void PlayFadeAimation(Action callback)
        {
            backgroundImage.color = new Color(0, 0, 0, 0);
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(backgroundImage.DOFade(1f, fadeTime));
            mySequence.AppendInterval(waitTime);
            mySequence.Append(backgroundImage.DOFade(0f, fadeTime));
            mySequence.OnComplete(() => callback?.Invoke());
        }

    }
}
