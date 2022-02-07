using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class AddCellGoldView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _goldValueText;

#pragma warning restore 0649
        #endregion

        public void SetTextGoldValue(string text) => _goldValueText.text = text;

        //UI
        public event Action OnClick;
        public void ActionClick() => OnClick?.Invoke();
    }
}
