using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class NoSpaceForSavePopupView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _messageText;
        [SerializeField] private Text _okButtonText;
        [SerializeField] private Text _timerText;
        [SerializeField] private Button _okButton;

#pragma warning restore 0649
        #endregion

        public void SetTextMessageText(string text) => _messageText.text = text;
        public void SetTextOkButtonText(string text) => _okButtonText.text = text;
        public void SetTextTimerText(string text) => _timerText.text = text;
        public void SetOkButtonInteractable(bool value) => _okButton.interactable = value;

        //UI
        public event Action OnOk;
        public void Ok() => OnOk?.Invoke();
    }
}
