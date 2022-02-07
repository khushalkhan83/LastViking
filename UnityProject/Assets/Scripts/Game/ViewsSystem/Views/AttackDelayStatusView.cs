using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class AttackDelayStatusView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image fillImage;
        [SerializeField] private Text timeText;

#pragma warning restore 0649
        #endregion

        public void SetLeftTimeAmount(float part) => fillImage.fillAmount = part;
        public void SetLeftTimeText(string text) => timeText.text = text;
    }
}
