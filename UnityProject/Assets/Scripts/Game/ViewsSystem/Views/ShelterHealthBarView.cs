using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class ShelterHealthBarView : ViewAnimateBase
    {
        public int TriggerIdUp => Animator.StringToHash("Up");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _healthImage;

#pragma warning restore 0649
        #endregion

        public void SetHealthAmount(float value) => _healthImage.fillAmount = value;

        public void PlayUp() => Animator.SetTrigger(TriggerIdUp);
    }
}
