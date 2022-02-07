using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class ObjectiveAttackProcessView : ViewBase
    {
        public int AnimatorParametrIdMoveFirst { get; } = Animator.StringToHash("MoveFirst");
        public int AnimatorParametrIdMoveSecond { get; } = Animator.StringToHash("MoveSecond");
        public int AnimatorParametrIdStay { get; } = Animator.StringToHash("Stay");
        public int AnimatorParametrIdShow { get; } = Animator.StringToHash("Show");
        public int AnimatorParametrIdHide { get; } = Animator.StringToHash("Hide");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _shelterHP;
        [SerializeField] private Text _timeText;
        [SerializeField] private Text _zombiesAlive;
        [SerializeField] private Slider _slider;
        [SerializeField] private Image _amountImage;
        [SerializeField] private Animator _animator;

#pragma warning restore 0649
        #endregion

        public void SetShelterHPText(string text) => _shelterHP.text = text;
        public void SetTimeText(string time) => _timeText.text = time;
        public void SetZombiesAliveText(string text) => _zombiesAlive.text = text;
        public void SetFillAmount(float value) => _amountImage.fillAmount = value;
        public void SetSliderValue(float value) => _slider.value = value;

        public void PlayMoveFirst() => _animator.SetTrigger(AnimatorParametrIdMoveFirst);
        public void PlayMoveSecond() => _animator.SetTrigger(AnimatorParametrIdMoveSecond);
        public void PlayStay() => _animator.SetTrigger(AnimatorParametrIdStay);
        public void PlayShow() => _animator.SetTrigger(AnimatorParametrIdShow);
        public void PlayHide() => _animator.SetTrigger(AnimatorParametrIdHide);
    }
}
