using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class RestorableCursorView : CursorViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _amountImage;
        [SerializeField] private Text _text;

#pragma warning restore 0649
        #endregion

        public void SetFillAmount(float value) => _amountImage.fillAmount = value;

        public void SetText(string value) => _text.text = value;
    }
}
