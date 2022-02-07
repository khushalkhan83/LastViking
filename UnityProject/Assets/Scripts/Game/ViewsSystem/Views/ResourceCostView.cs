using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class ResourceCostView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _text;
        [SerializeField] private Image _icon;

#pragma warning restore 0649
        #endregion

        public void SetText(string text) => _text.text = text;
        public void SetIcon(Sprite sprite) => _icon.sprite = sprite;
    }
}
