using Core.Views;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views
{
    public class PickUpTimeDelayTimerHealthCursorView : CursorViewExtended
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _amountImage;

        [SerializeField] private Image _amountImageHealth;
        [SerializeField] private Text _text;

#pragma warning restore 0649
        #endregion


        public void SetFillAmountHealth(float value) => _amountImageHealth.fillAmount = value;
        public void SetText(string value) => _text.text = value;

        public void SetFillAmount(float value) => _amountImage.fillAmount = 1 - value;
    }
}