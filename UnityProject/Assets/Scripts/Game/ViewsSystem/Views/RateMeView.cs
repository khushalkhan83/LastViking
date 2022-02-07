using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class RateMeView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] Button _updateButton;
        [SerializeField] Text _updateRateText;
        [SerializeField] Text _updateRateButtonText;

#pragma warning restore 0649
        #endregion

        public void SetTextTitle(string text) => _updateRateText.text = text;
        public void SetTextRateButton(string text) => _updateRateButtonText.text = text;

        //UI
        public event Action OnClose;
        public void Close() => OnClose?.Invoke();

        //UI
        public event Action OnRatePressed;
        public void ActionRateMe() => OnRatePressed?.Invoke();
    }
}



