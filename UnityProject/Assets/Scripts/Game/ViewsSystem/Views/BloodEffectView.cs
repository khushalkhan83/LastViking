using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class BloodEffectView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _image;

#pragma warning restore 0649
        #endregion

        public void SetAlpha(float alpha) => _image.CrossFadeAlpha(alpha, 0.1f, true);
    }
}
