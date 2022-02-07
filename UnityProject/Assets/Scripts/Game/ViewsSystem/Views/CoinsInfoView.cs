using System;
using Core.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class CoinsInfoView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private TextMeshProUGUI _coinsValue;
        [SerializeField] private TextMeshProUGUI _citizensValue;
        [SerializeField] private AnimationCurve _pulseAnimation;
        [SerializeField] private float _pulseAnimationDuration;
        [SerializeField] private RawImage _glowImage;
        [SerializeField] private GameObject _iconGlowGroup;

#pragma warning restore 0649
        #endregion

        private int mDisplayedCoinsValue;
        private float mPulseAnimationTime;
        private Vector3 mIconGlowGroupOriginalScale;

        public event Action OnAddCoins;

        public void SetTextCitizensValue(string text) => _citizensValue.text = text;

        public int DisplayedCoinsValue
        {
            get => mDisplayedCoinsValue;
            set
            {
                var _indicateIncrement = value > mDisplayedCoinsValue;

                mDisplayedCoinsValue = value;

                _coinsValue.text = value.ToString();

                if (_indicateIncrement)
                {
                    AnimateIncrement();
                }
            }
        }

        private AnimationCurve PulseAnimation => _pulseAnimation;
        private RawImage GlowImage => _glowImage;
        private GameObject IconGlowGroup => _iconGlowGroup;

        public void ActionAddCoins() => OnAddCoins?.Invoke();

        private void Start()
        {
            mIconGlowGroupOriginalScale = IconGlowGroup.transform.localScale;
        }

        private void Update()
        {
            // Pulse Animation.
            if (mPulseAnimationTime > 0)
            {
                mPulseAnimationTime -= Time.smoothDeltaTime;

                float animationProgression = (_pulseAnimationDuration - mPulseAnimationTime) / _pulseAnimationDuration;
                float animation = PulseAnimation.Evaluate(animationProgression);

                // Animate Glow image alpha.
                var glowColor = GlowImage.color;
                glowColor.a = animation;
                GlowImage.color = glowColor;

                // Animate Group scale.
                IconGlowGroup.transform.localScale = mIconGlowGroupOriginalScale + (mIconGlowGroupOriginalScale * animation * 0.2f);
            }
        }

        private void AnimateIncrement()
        {
            mPulseAnimationTime = _pulseAnimationDuration;
        }
    }
}