using System;
using Game.Colors;
using UnityEngine;

namespace Game.Colors
{
    public enum ColorPreset
    {
        green,
        blue,
        darkGray,
        gray,
        spectra
    }
}

namespace Game.Models
{
    public class ColorsViewModel : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private ColorColorPresetDictionary colorByColorPresetDictionary;
        #pragma warning restore 0649
        #endregion

        public Color GetColor(ColorPreset colorPreset)
        {
            if(colorByColorPresetDictionary.TryGetValue(colorPreset, out var color))
            {
                return color;
            }
            else
            {
                return Color.white;
            }
        }
    }
}
