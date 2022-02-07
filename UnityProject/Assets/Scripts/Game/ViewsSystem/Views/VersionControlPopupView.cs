using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class VersionControlPopupView : ViewBase
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] Button _updateButton;
        [SerializeField] Text _newVersionText;
        [SerializeField] Text _messageText;
        [SerializeField] Text _updateButtonText;
#pragma warning restore 0649
        #endregion

        public void SetTextVersion(string text) => _newVersionText.text = text;
        public void SetTextMesssage(string text) => _messageText.text = text;
        public void SetTextUpdateButton(string text) => _updateButtonText.text = text;

        //UI
        public event Action OnUpdatePressed;
        public void PressUpdate() => OnUpdatePressed?.Invoke();
    }
}

