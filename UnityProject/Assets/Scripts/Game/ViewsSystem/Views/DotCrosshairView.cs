using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class DotCrosshairView : CursorViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _croshairImage;

        [SerializeField] private Color _activeColor;
        [SerializeField] private Color _passiveColor;

#pragma warning restore 0649
        #endregion

        public Color ActiveColor => _activeColor;
        public Color PassiveColor => _passiveColor;

        public void SetColorCrosshair(Color color) => _croshairImage.color = color;
    }
}
