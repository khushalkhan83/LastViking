using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class InventoryIsFillPopupView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _countCoin;
        [SerializeField] private Text _messageText;
        [SerializeField] private Text _backButtonText;
        [SerializeField] private Text _expandGoldButtonText;
        [SerializeField] private Text _expandWatchButtonText;

        [VisibleObject]
        [SerializeField] private GameObject _expandButtonGoldObject;

        [VisibleObject]
        [SerializeField] private GameObject _expandButtonWatchObject;

#pragma warning restore 0649
        #endregion

        public void SetIsVisibleExpandGoldButton(bool isVisible) => _expandButtonGoldObject.SetActive(isVisible);
        public void SetIsVisibleExpandWatchButton(bool isVisible) => _expandButtonWatchObject.SetActive(isVisible);

        public void SetCountCoin(string value) => _countCoin.text = value;

        public void SetTextMessageText(string text) => _messageText.text = text;

        public void SetTextBackButton(string text) => _backButtonText.text = text;

        public void SetTextExpandGoldButton(string text) => _expandGoldButtonText.text = text;
        public void SetTextExpandWatchButton(string text) => _expandWatchButtonText.text = text;

        //UI
        public event Action OnBack;
        public void Back() => OnBack?.Invoke();

        //UI
        public event Action OnExpand;
        public void Expand() => OnExpand?.Invoke();

        //

    }
}
