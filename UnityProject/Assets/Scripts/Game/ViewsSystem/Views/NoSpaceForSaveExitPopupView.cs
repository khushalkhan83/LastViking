using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class NoSpaceForSaveExitPopupView : ViewBase
    {
          #region Data
#pragma warning disable 0649

        [SerializeField] private Text _messageText;
        [SerializeField] private Text _exitButtonText;
        [SerializeField] private Text _timerText;
        [SerializeField] private Button _exitButton;

#pragma warning restore 0649
        #endregion

        public void SetTextMessageText(string text) => _messageText.text = text;
        public void SetTextExitButtonText(string text) => _exitButtonText.text = text;
        public void SetTextTimerText(string text) => _timerText.text = text;
        public void SetExitButtonInteractable(bool value) => _exitButton.interactable = value;

        //UI
        public event Action OnExit;
        public void Exit() => OnExit?.Invoke();
    }
}
