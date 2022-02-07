using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class HotRepairingView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _countCoinsText;

#pragma warning restore 0649
        #endregion

        public void SetCountText(string text) => _countCoinsText.text = text;

        //UI
        public event Action OnClick;
        public void ActionClick() => OnClick?.Invoke();
    }
}
