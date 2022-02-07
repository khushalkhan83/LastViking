using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Models
{
    [Serializable]
    public class QualityConfig
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private QualityID _quaityID;
        [SerializeField] private bool _isHasGrass;
        [SerializeField] private bool _isHasButterflies;
        [SerializeField] private bool _isHasClouds;
        [SerializeField] private bool _isHasStars;
        [SerializeField] private bool _isHasMoon;
        [SerializeField] private int _sizeWaterReflectionTexture;
        [SerializeField] private SkyboxSunKeywordID _skyboxSunKeywordID;
        [SerializeField] private ShaderLODEnvironmentID _shaderLODEnvironmentID;
        [SerializeField] private ShaderLODVegetationID _shaderLODVegetationID;
        [SerializeField] private ShaderLODToolsID _shaderLODToolsID;
        [SerializeField] private WaterQualitySettingsID _waterQualitySettingsID;
        [SerializeField] private int _defaultVSync = 0;
        [SerializeField] private int _targetResolution = kNativeResolution;

        public const int kNativeResolution = -1;
#pragma warning restore 0649
        #endregion

        public int targetResolution => _targetResolution;
        public QualityID QuaityID => _quaityID;
        public bool IsHasGrass => _isHasGrass;
        public bool IsHasButterflies => _isHasButterflies;
        public bool IsHasClouds => _isHasClouds;
        public bool IsHasStars => _isHasStars;
        public bool IsHasMoon => _isHasMoon;
        public int DefaultVSync => _defaultVSync;
        public SkyboxSunKeywordID SkyboxSunKeywordID => _skyboxSunKeywordID;
        public ShaderLODEnvironmentID ShaderLODEnvironmentID => _shaderLODEnvironmentID;
        public ShaderLODVegetationID ShaderLODVegetationID => _shaderLODVegetationID;
        public ShaderLODToolsID ShaderLODToolsID => _shaderLODToolsID;
        public WaterQualitySettingsID WaterQualitySettingsID => _waterQualitySettingsID;
    }
}
