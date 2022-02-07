using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class BlackView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _image;

#pragma warning restore 0649
        #endregion

        public void SetColor(Color color) => _image.color = color;
    }
}
