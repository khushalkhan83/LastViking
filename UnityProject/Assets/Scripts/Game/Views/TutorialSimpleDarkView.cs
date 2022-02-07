using Core.Views;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class TutorialSimpleDarkView : ViewBase
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private float _animationDuration = 0.2f;

#pragma warning restore 0649
        #endregion

        private float defaultAlpha;

        private void Awake()
        {
            defaultAlpha = _backgroundImage.color.a;
        }

        [Button]
        public void PlayFadeAnimation()
        {
            _backgroundImage.DOFade(0, 0);
            _backgroundImage.DOFade(defaultAlpha, _animationDuration);
        }
    }
}
