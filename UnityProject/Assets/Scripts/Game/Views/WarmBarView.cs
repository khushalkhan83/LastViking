using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class WarmBarView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _valueText;

        [SerializeField] private Image _iconImage;

        [VisibleObject]
        [SerializeField] private GameObject _watchVideoButtonObject;

        [VisibleObject]
        [SerializeField] private GameObject _buyAddonButtonObject;

        [SerializeField] private Text _coinsCountText;

        [SerializeField] private Color _normalWarmColor;
        [SerializeField] private Color _lowWarmColor;

#pragma warning restore 0649
        #endregion

        public Color NormalWarmColor => _normalWarmColor;
        public Color LowWarmColor => _lowWarmColor;

        public void SetValue(string value) => _valueText.text = value;
        public void SetValueTextColor(Color color) => _valueText.color = color;
        public void SetIconColor(Color color) => _iconImage.color = color;
        public void SetIsVisibleWatchVideoButton(bool value) => _watchVideoButtonObject.SetActive(value);
        public void SetIsVisibleBuyAddonButton(bool value) => _buyAddonButtonObject.SetActive(value);

        public void SetTextBuyCoinsCount(string text) => _coinsCountText.text = text;

        //

        //UI
        public event Action OnAddAddon;
        public void AddAddon() => OnAddAddon?.Invoke();

        //
    }
}
