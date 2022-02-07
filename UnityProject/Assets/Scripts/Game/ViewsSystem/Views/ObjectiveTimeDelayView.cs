using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class ObjectiveTimeDelayView : ViewBase, INotification
    {
        public static int TriggerShow { get; } = Animator.StringToHash("Show");
        public static int TriggerShowTop { get; } = Animator.StringToHash("ShowTop");
        public static int TriggerHideTop { get; } = Animator.StringToHash("HideTop");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _objectiveIcon;
        [SerializeField] private Color _backgroundIconColor;
        [SerializeField] private Color _backgroundTextColor;
        [SerializeField] private Text _descriptionText;
        [SerializeField] private Text _timeText;
        [SerializeField] private Animator _animator;

#pragma warning restore 0649
        #endregion

        public Image ObjectiveIcon => _objectiveIcon;
        public Color BackgroundIconColor => _backgroundIconColor;
        public Color BackgroundTextColor => _backgroundTextColor;
        public Text DescriptionText => _descriptionText;
        public Text TimeText => _timeText;
        public Animator Animator => _animator;

        public void SetObjecvtiveIcon(Sprite sprite) => _objectiveIcon.sprite = sprite;
        public void SetBackgroundIconColor(Color color) => _backgroundIconColor = color;
        public void SetBackgroundTextColor(Color color) => _backgroundTextColor = color;
        public void SetDescriptionText(string text) => _descriptionText.text = text;
        public void SetTimeText(string text) => _timeText.text = text;

        public void PlayShow() => Animator.SetTrigger(TriggerShow);
        public void PlayShowTop() => Animator.SetTrigger(TriggerShowTop);
        public void PlayHideTop() => Animator.SetTrigger(TriggerHideTop);
        public void SetAsLast() => transform.SetAsLastSibling();
        public void SetAsFirst() => transform.SetAsFirstSibling();
    }
}