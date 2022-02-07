using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class SpecialMessageView : ViewTimelineBase
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private Image _icon;
        [SerializeField] private Text _upperText;
        [SerializeField] private Text _bottomText;
        #pragma warning restore 0649
        #endregion

        public void SetIcon(Sprite sprite) => _icon.sprite = sprite;
        public void SetUpperText(string text) => _upperText.text = text;
        public void SetBottomText(string text) => _bottomText.text = text;
    }
}
