using Core.Views;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views
{
    public class OpenDoorHealthCursorView : CursorViewExtended
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _amountImage;
        [SerializeField] private Text _text;

#pragma warning restore 0649
        #endregion

        public void SetHealthFillAmount(float value) => _amountImage.fillAmount = value;
        public void SetHealthText(string value) => _text.text = value;
    }
}
