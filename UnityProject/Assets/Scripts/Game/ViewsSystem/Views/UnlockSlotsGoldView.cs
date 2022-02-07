using UnityEngine.UI;
using UnityEngine;
using Core.Views;
using System;

namespace Game.Views
{
    public class UnlockSlotsGoldView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _expandOnceButtonTitle;
        [SerializeField] private Text _expandOnceCoinValue;
        [SerializeField] private Text _expandForeverButtonTitle;
        [SerializeField] private Text _expandForeverCoinValue;

#pragma warning restore 0649
        #endregion

        public void SetTextExpandOnceButton(string text) => _expandOnceButtonTitle.text = text;
        public void SetTextExpandOnceCoin(string text) => _expandOnceCoinValue.text = text;
        public void SetTextExpandForeverButton(string text) => _expandForeverButtonTitle.text = text;
        public void SetTextExpandForeverCoin(string text) => _expandForeverCoinValue.text = text;

        //UI
        public event Action OnUnlockOnce;
        public void UnlockOnce() => OnUnlockOnce?.Invoke();

        public event Action OnUnlockForever;
        public void UnlockForever() => OnUnlockForever?.Invoke();
    }
}
