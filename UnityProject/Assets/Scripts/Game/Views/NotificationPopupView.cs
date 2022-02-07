using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class NotificationPopupView : ViewBase
    {
         #region Data
#pragma warning disable 0649

        [SerializeField] private Text _titleText;
        [SerializeField] private Text _descriptionText;
        [SerializeField] private Text _okButtonText;

#pragma warning restore 0649
        #endregion

        public void SetTextTitle(string value) => _titleText.text = value;
        public void SetTextDescription(string value) => _descriptionText.text = value;
        public void SetTextOkButton(string value) => _okButtonText.text = value;

        //UI
        public event Action OnOk;
        public void ActionOk() => OnOk?.Invoke();
    }
}
