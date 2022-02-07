using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class TutorialDarkView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [VisibleObject, SerializeField] private GameObject[] _stepObjects;

        [Header("Localization")]
        [SerializeField] private Text _attackText;
        [SerializeField] private Text _moveText;
        [SerializeField] private Text _jumpText;
        [SerializeField] private Text _runText;
        [SerializeField] private Text _missionText;

#pragma warning restore 0649
        #endregion

        public void SetActiveStep(int index) => _stepObjects[index].SetActive(true);

        public void SetTextAttack(string text) => _attackText.text = text;
        public void SetMoveAttack(string text) => _moveText.text = text;
        public void SetJumpAttack(string text) => _jumpText.text = text;
        public void SetRunAttack(string text) => _runText.text = text;
        public void SetMissionAttack(string text) => _missionText.text = text;

        public event Action OnClick;
        public void Click() => OnClick?.Invoke(); 
    }
}
