using Core.Views;
using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class ObjectiveView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ObjectiveRewardView[] _rewards;
        [SerializeField] private DisablableButtonView _rewardButton;
        [SerializeField] private DisablableButtonView _rerollButton;
        [SerializeField] private Text _objectiveDescription;
        [SerializeField] private Text _objectiveProgress;
        [SerializeField] private Slider _slider;
        [SerializeField] [VisibleObject] private GameObject _sliderHandle;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Animator _animator;
        [SerializeField] private string _fadeOutStateName;

#pragma warning restore 0649
        #endregion

        public ObjectiveRewardView[] RewardViews => _rewards;
        public DisablableButtonView RewardButton => _rewardButton;
        public DisablableButtonView RerollButton => _rerollButton;

        public void SetIsVisibleSliderHandle(bool isVisible) => _sliderHandle.SetActive(isVisible);

        public void SetTextObjectiveDescription(string text) => _objectiveDescription.text = text;
        public void SetTextObjectiveProgress(string text) => _objectiveProgress.text = text;

        public void SetColorBackground(Color color) => _backgroundImage.color = color;
        public void SetColorDescriptionText(Color color) => _objectiveDescription.color = color;
        public void SetPColorProgressText(Color color) => _objectiveProgress.color = color;

        public void SetFillAmountSlider(float value) => _slider.value = value;

        [Button]
        public void PlayFadeOutAnimation() => _animator.Play(_fadeOutStateName);
        //UI
        public event Action OnGetReward;
        public void ActionGetReward() => OnGetReward?.Invoke();
        //UI
        public event Action OnReroll;
        public void ActionReroll() => OnReroll?.Invoke();
    }
}
