using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class DestroyableCursorView : CursorViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _amountImage;

#pragma warning restore 0649
        #endregion

        public void SetFillAmount(float value) => _amountImage.fillAmount = value;
    }
}