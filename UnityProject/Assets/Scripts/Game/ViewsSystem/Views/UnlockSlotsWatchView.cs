using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class UnlockSlotsWatchView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _expandButtonTitle;
        [SerializeField] private Text _expandForeverButtonTitle;
        [SerializeField] private Text _expandForeverCoinValue;

#pragma warning restore 0649
        #endregion

        public void SetTextExpandButton(string text) => _expandButtonTitle.text = text;
        public void SetTextExpandForeverButton(string text) => _expandForeverButtonTitle.text = text;
        public void SetTextExpandForeverCoin(string text) => _expandForeverCoinValue.text = text;

        //UI
        public event Action OnUnlockOnce;
        public void UnlockOnce() => OnUnlockOnce?.Invoke();

        public event Action OnUnlockForever;
        public void UnlockForever() => OnUnlockForever?.Invoke();
    }
}
