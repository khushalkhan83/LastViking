using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class FurnaceHealthCursorView : CursorViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _fireIcon;

        [SerializeField] private Sprite _fireOn;
        [SerializeField] private Sprite _fireOff;

        [SerializeField] private Image _amountImage;
        [SerializeField] private Text _text;

#pragma warning restore 0649
        #endregion

        public Sprite FireOn => _fireOn;
        public Sprite FireOff => _fireOff;

        public void SetFireIcon(Sprite value) => _fireIcon.sprite = value;

        public void SetFillAmount(float value) => _amountImage.fillAmount = value;

        public void SetText(string value) => _text.text = value;

        //UI
        public event Action OnClickOpen;
        public void ClickOpen() => OnClickOpen?.Invoke();

        //UI
        public event Action OnClickFire;
        public void ClickFire() => OnClickFire?.Invoke();
    }
}
