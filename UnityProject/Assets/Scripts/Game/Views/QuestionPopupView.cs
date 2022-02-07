using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class QuestionPopupView : ViewBase
    {
         #region Data
#pragma warning disable 0649

        [SerializeField] private Text _titleText;
        [SerializeField] private Text _descriptionText;
        [SerializeField] private Text _okButtonText;
        [SerializeField] private Text _backButtonText;

#pragma warning restore 0649
        #endregion

        public void SetTextTitle(string value) => _titleText.text = value;
        public void SetTextDescription(string value) => _descriptionText.text = value;
        public void SetTextOkButton(string value) => _okButtonText.text = value;
        public void SetTextBackButton(string value) => _backButtonText.text = value;

        //UI
        public event Action OnApply;
        public void ActionApply() => OnApply?.Invoke();

        //UI
        public event Action OnClose;
        public void ActionClose() => OnClose?.Invoke();
    }
}
