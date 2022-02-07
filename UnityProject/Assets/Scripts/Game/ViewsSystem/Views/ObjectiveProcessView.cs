using Core.Views;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class ObjectiveProcessView : ViewBase, INotification
    {
        public static int TriggerShow { get; } = Animator.StringToHash("Show");
        public static int TriggerShowTop { get; } = Animator.StringToHash("ShowTop");
        public static int TriggerHideTop { get; } = Animator.StringToHash("HideTop");
        public static int TriggerComplete { get; } = Animator.StringToHash("Complete");
        public static int TriggerHidden { get; } = Animator.StringToHash("Hidden");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _objectiveIcon;
        [SerializeField] private Text _descriptionText;
        [SerializeField] protected Text _timeText;
        [SerializeField] protected Slider _slider;
        [SerializeField] protected Image _amountImage;
        [SerializeField] private Animator _animator;
        [SerializeField] private float fillAnimationDurtion = 0.3f;

#pragma warning restore 0649
        #endregion

        public Image ObjectiveIcon => _objectiveIcon;
        public Text DescriptionText => _descriptionText;
        public Text TimeText => _timeText;
        public Slider Slider => _slider;
        public Image AmountImage => _amountImage;
        public Animator Animator => _animator;

        private Tweener fillTween = null;

        public void SetObjecvtiveIcon(Sprite sprite) => _objectiveIcon.sprite = sprite;
        public void SetDescriptionText(string text) => _descriptionText.text = text;
        public void SetTimeText(string text) => _timeText.text = text;
        public void SetSliderValue(float value)
        {
            if(fillTween != null)
                fillTween.Kill();
            fillTween = DOTween.To(() => _slider.value, x => {_slider.value = x; _amountImage.fillAmount = x;}, value, fillAnimationDurtion);
        }

        public void PlayShow() => Animator.SetTrigger(TriggerShow);
        public void PlayShowTop() => Animator.SetTrigger(TriggerShowTop);
        public void PlayHideTop() => Animator.SetTrigger(TriggerHideTop);
        public void PlayComplete() => Animator.SetTrigger(TriggerComplete);
        public void PlayHidden() => Animator.SetTrigger(TriggerHidden);
        public void SetAsLast() => transform.SetAsLastSibling();
        public void SetAsFirst() => transform.SetAsFirstSibling();
    }
}
