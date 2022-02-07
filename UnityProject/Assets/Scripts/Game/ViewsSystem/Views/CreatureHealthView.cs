using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class CreatureHealthView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _amountImage;
        [SerializeField] RectTransform _healthbarTransform;

#pragma warning restore 0649
        #endregion

        public void SetFillAmount(float value) => _amountImage.fillAmount = value;

        public void SetAnchoredPosition(Vector3 position) => _healthbarTransform.anchoredPosition = position;
        public void SetScale(float scale) => _healthbarTransform.localScale = Vector3.one * scale;
    }
}
