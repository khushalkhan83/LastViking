using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class TutorialObjectivesDarkScreenView : ViewBase
    {
        #region Data
        #pragma warning disable 0649
        [VisibleObject, SerializeField] private GameObject[] _stepObjects;

        [Header("Localization")]
        [SerializeField] private Text _objectivesButtonText;
        [SerializeField] private Text _takeRewardText;
        
        #pragma warning restore 0649
        #endregion
        
        public void SetActiveStep(int index)
        {
            for (int i = 0; i < _stepObjects.Length; i++)
            {
                _stepObjects[i].SetActive(index == i);
            }
        }

        public event Action OnClick;
        public void Click() => OnClick?.Invoke();

        public void SetObjectivesButtonText(string text) => _objectivesButtonText.text = text;
        public void SetTextTakeReward(string text) => _takeRewardText.text = text;
    }
}
