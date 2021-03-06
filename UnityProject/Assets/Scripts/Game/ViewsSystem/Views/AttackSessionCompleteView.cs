using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class AttackSessionCompleteView : ViewBase, INotification
    {
        public static int TriggerShow { get; } = Animator.StringToHash("Show");
        public static int TriggerShowTop { get; } = Animator.StringToHash("ShowTop");
        public static int TriggerHideTop { get; } = Animator.StringToHash("HideTop");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _descriptionText;
        [SerializeField] private Animator _animator;

#pragma warning restore 0649
        #endregion

        public Text DescriptionText => _descriptionText;
        public Animator Animator => _animator;

        public void SetDescriptionText(string text) => _descriptionText.text = text;

        public void PlayShow() => Animator.SetTrigger(TriggerShow);
        public void PlayShowTop() => Animator.SetTrigger(TriggerShowTop);
        public void PlayHideTop() => Animator.SetTrigger(TriggerHideTop);
        public void SetAsLast() => transform.SetAsLastSibling();
        public void SetAsFirst() => transform.SetAsFirstSibling();
    }
}
