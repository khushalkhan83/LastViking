using Core.Views;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Game.Views
{
    public class TreasureLootTimerCursorView : CursorViewExtended
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _amountImage;
        [SerializeField] private Text _timerText;

#pragma warning restore 0649
        #endregion

        public void SetFillAmount(float value) => _amountImage.fillAmount = 1 - value;
        public void SetTimerText(string time) => _timerText.text = time;
    }
}
