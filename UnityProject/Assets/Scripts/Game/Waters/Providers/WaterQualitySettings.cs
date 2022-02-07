using System;
using UnityEngine;

namespace Game.Controllers
{
    [Serializable]
    public class WaterQualitySettings
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private bool _isHasReflections;
        [SerializeField] private bool _isHasSunGlossines;
        [SerializeField] private bool _isHasDepth;
        [SerializeField] private int _sizeWaterReflectionTexture;
        [SerializeField] private Material _material;

#pragma warning restore 0649
        #endregion

        public bool IsHasReflctions => _isHasReflections;
        public bool IsHasSunGlossines => _isHasSunGlossines;
        public bool IsHasDepth => _isHasDepth;
        public int SizeWaterReflectionTexture => _sizeWaterReflectionTexture;
        public Material Material => _material;
    }
}
