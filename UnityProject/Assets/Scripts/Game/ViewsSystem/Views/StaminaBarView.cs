using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class StaminaBarView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _amountImage;

#pragma warning restore 0649
        #endregion

        public void SetAmount(float value) => _amountImage.fillAmount = value;
    }
}
