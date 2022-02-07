using UnityEngine;

namespace Game.GraphicsQuality
{
    // based on LightModel from LastPirate
    [ExecuteInEditMode]
    public class SimpleLightModel : MonoBehaviour
    {
        private const string kGlobalShaderProperty_Terrain_ColorMultiplier = "Terrain_ColorMultiplier";
        private const string kGlobalShaderProperty_Terrain_ColorAddition = "Terrain_ColorAddition";
        #region Data
#pragma warning disable 0649
        [ColorUsage(false, true)]
        [SerializeField] private Color _terrainAdditionColor;
        [ColorUsage(false, true)]
        [SerializeField] private Color _terrainMultiplierColor;

        [SerializeField] private bool _updateColor;
#pragma warning restore 0649
        #endregion
        public int PropertyIDTerrainColorMultiplier { get; } = Shader.PropertyToID(kGlobalShaderProperty_Terrain_ColorMultiplier);
        public int PropertyIDTerrainColorAddition { get; } = Shader.PropertyToID(kGlobalShaderProperty_Terrain_ColorAddition);

        #region MonoBehaviour
        private void OnEnable()
        {
            UpdateLight();
        }

        private void Update()
        {
            if (!_updateColor) return;

            UpdateLight();
        }
        #endregion

        private void UpdateLight()
        {
            SetTerrainAdditionColor();
            SetTerrainMultiplierColor();
        }

        private void SetTerrainAdditionColor() => Shader.SetGlobalColor(PropertyIDTerrainColorAddition, _terrainAdditionColor);
        private void SetTerrainMultiplierColor() => Shader.SetGlobalColor(PropertyIDTerrainColorMultiplier, _terrainMultiplierColor);
    }
}