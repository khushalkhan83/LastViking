using System.Collections;
using System.Collections.Generic;
using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class StartupView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Slider _loadingSlider;
        [SerializeField] private Text _sliderText;
        [SerializeField] private Image _backgroundImage;

#pragma warning restore 0649
        #endregion

        public void SetSliderValue(float sliderValue) => _loadingSlider.value = sliderValue;
        public void SetAlpha(float alpha) => _canvasGroup.alpha = alpha;
        public void SetSliderText(string text) => _sliderText.text = text;
        public void SetImageAmount(float amount) => _backgroundImage.fillAmount = amount;
    }
}
