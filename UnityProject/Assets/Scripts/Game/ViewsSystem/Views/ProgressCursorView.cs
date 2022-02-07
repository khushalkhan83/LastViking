using Core.Views;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views
{
    public class ProgressCursorView : CursorViewExtended
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _amountImage;
        [SerializeField] private Text _timerText;
        [SerializeField] private GameObject _timerTextObject;
        [SerializeField] private GameObject _useObject;
        [SerializeField] private GameObject _cursor;

#pragma warning restore 0649
        #endregion


        public void SetFillAmount(float value) => _amountImage.fillAmount = value;
        public void SetTimerText(string text) => _timerText.text = text;

        public void SetTimerVisible(bool isVisible) => _timerTextObject.SetActive(isVisible);
        public void SetUseObjectVisible(bool isVisible) => _useObject.SetActive(isVisible);
        public void SetCursorVisible(bool isVisible) => _cursor.SetActive(isVisible);
    }
}
