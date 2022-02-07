using UnityEngine;

namespace Game.Models
{
    public enum SkyboxSunKeywordID
    {
        _SUNDISK_NONE = 0,
        _SUNDISK_SIMPLE = 1, 
        _SUNDISK_HIGH_QUALITY = 2,
    }

    [ExecuteInEditMode]
    public class LightModel : MonoBehaviour
    { 
        public int PropertyIDAdditiveColor { get; } = Shader.PropertyToID("_AdditiveColor");
        public int PropertyIDAlpha { get; } = Shader.PropertyToID("_Alpha");
        public int PropertyIDAtmosphereThickness { get; } = Shader.PropertyToID("_AtmosphereThickness");
        public int PropertyIDSkyTint { get; } = Shader.PropertyToID("_SkyTint");
        public int PropertyIDExposure { get; } = Shader.PropertyToID("_Exposure");
        public int PropertyIDCloudsColor { get; } = Shader.PropertyToID("_ColorAdd");
        public int PropertyIDLightColor { get; } = Shader.PropertyToID("Light_Color");
        public int PropertyIDLightDirection { get; } = Shader.PropertyToID("Light_Direction");
        public int PropertyIDTerrainShadowDistance { get; } = Shader.PropertyToID("Terrain_ShadowDistance");
        public int PropertyIDEnvironmentColor { get; } = Shader.PropertyToID("Environment_Color");
        public int PropertyIDTerrainColorShadow { get; } = Shader.PropertyToID("Terrain_ColorShadow");
        public int PropertyIDTerrainColorEdge { get; } = Shader.PropertyToID("Terrain_ColorEdge");
        public int PropertyIDTerrainColorAddition { get; } = Shader.PropertyToID("Terrain_ColorAddition");
        public int PropertyIDTerrainColorMultiplier { get; } = Shader.PropertyToID("Terrain_ColorMultiplier");
        public int PropertyIDOceanColorReflection { get; } = Shader.PropertyToID("Ocean_ColorReflection");
        public int PropertyIDOceanColorBase { get; } = Shader.PropertyToID("Ocean_ColorBase");

        #region Data
#pragma warning disable 0649

        [ColorUsage(false, true)]
        [SerializeField] private Color _skyColor;
        [ColorUsage(false, true)]
        [SerializeField] private Color _equatorColor;
        [ColorUsage(false, true)]
        [SerializeField] private Color _groundColor;
        [ColorUsage(false, true)]
        [SerializeField] private Color _fogColor;

        [ColorUsage(false, true)]
        [SerializeField] private Color _baseOceanColor;
        [ColorUsage(false, true)]
        [SerializeField] private Color _reflectionOceanColor;

        [ColorUsage(false, true)]
        [SerializeField] private Color _multiplierTerrainInfluence;
        [ColorUsage(false, true)]
        [SerializeField] private Color _additionTerrainInfluence;
        [ColorUsage(false, true)]
        [SerializeField] private Color _edgeTerrainInfluence;
        [ColorUsage(false, true)]
        [SerializeField] private Color _shadowTerrainColor;
        [ColorUsage(false, true)]
        [SerializeField] private Color _environmentTerrainColor;

        [ColorUsage(false, true)]
        [SerializeField] private Color _skyboxSkyTint;

        [SerializeField] private Color _skyboxAdditiveSkyboxColor;
        [SerializeField] private Color _cloudsColor;
        [SerializeField] private Material _skyboxMaterial;
        [SerializeField] private Material _moonMaterial;
        [SerializeField] private Material _starsMaterial;
        [SerializeField] private Material _cloudsMaterial;
        [SerializeField] private int _fogStart;
        [SerializeField] private int _fogEnd;

        [SerializeField] private bool _isVisibleSun;
        [SerializeField] private float _moonAlpha;
        [SerializeField] private float _starsAlpha;
        [SerializeField] private float _skyboxAtmosphereThickness;
        [SerializeField] private float _skyboxExposure;

        [SerializeField] private Animation _animation;

        [SerializeField] private Light _light;

#pragma warning restore 0649
        #endregion

        public bool IsVisibleSun => _isVisibleSun;
        public float MoonAlpha => _moonAlpha;
        public float StarsAlpha => _starsAlpha;
        public float SkyboxAtmosphereThickness => _skyboxAtmosphereThickness;
        public float SkyboxExposure => _skyboxExposure;
        public Color SkyColor => _skyColor;
        public Color SkyboxSkyTint => _skyboxSkyTint;
        public Color EquatorColor => _equatorColor;
        public Color GroundColor => _groundColor;
        public Color FogColor => _fogColor;
        public Color OceanBaseColor => _baseOceanColor;
        public Color OceanReflectionColor => _reflectionOceanColor;
        public Color TerrainMultiplierColor => _multiplierTerrainInfluence;
        public Color TerrainAdditionColor => _additionTerrainInfluence;
        public Color TerrainEdgeColor => _edgeTerrainInfluence;
        public Color TerrainShadowColor => _shadowTerrainColor;
        public Color TerrainEnvironmentColor => _environmentTerrainColor;
        public Color SkyboxAdditiveSkyboxColor => _skyboxAdditiveSkyboxColor;
        public Color CloudsColor => _cloudsColor;
        public Material SkyboxMaterial => _skyboxMaterial;
        public Material MoonMaterial => _moonMaterial;
        public Material StarsMaterial => _starsMaterial;
        public Material CloudsMaterial => _cloudsMaterial;

        public int FogStart => _fogStart;
        public int FogEnd => _fogEnd;

        public Animation Animation => _animation;
        public Light Light => _light;

        private ModelsSystem modelsSystem;
        private ModelsSystem ModelsSystem {
            get{
                if(modelsSystem == null)
                    modelsSystem = ModelsSystem.Instance;
                if(modelsSystem == null) 
                    modelsSystem = FindObjectOfType<ModelsSystem>();
                return modelsSystem;
            }
        }
        public GameTimeModel GameTimeModel => ModelsSystem._gameTimeModel;
        public GameUpdateModel GameUpdateModel {get => ModelsSystem?._gameUpdateModel;}
        public QualityModel QualityModel => ModelsSystem._qualityModel; 

        public float TerrainShadowDistance => QualitySettings.shadowDistance;

        public AnimationState AnimState { get; private set; }

        SkyboxSunKeywordID SkyboxSunKeywordID =>
            IsVisibleSun ?
            QualityModel.QualityConfig.SkyboxSunKeywordID :
            SkyboxSunKeywordID._SUNDISK_NONE;

        private void Awake()
        {
            AnimState = Animation[Animation.clip.name];
            AnimState.speed = 0.0f;
        }

        private void OnEnable()
        {
            if(GameUpdateModel != null)
                GameUpdateModel.OnUpdate += OnUpdate;
        }

        private void OnDisable()
        {
            if(GameUpdateModel != null)
                GameUpdateModel.OnUpdate -= OnUpdate;
        }

        float __normalizedTime;

#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying)
            {
                UpdateLight();
            }
        }
#endif

        private void OnUpdate()
        {
#if UNITY_EDITOR
            if (AnimState == null)
                return;
#endif
            __normalizedTime = (float)GameTimeModel.EnviroTimeOfDayTicks / GameTimeModel.DayDurationTicks;
            AnimState.normalizedTime = __normalizedTime;

            UpdateLight();
        }

        private void UpdateLight()
        {
            SetAmbientSkyColor();
            SetAmbientEquatorColor();
            SetAmbientGroundColor();
            SetFogColor();
            SetFogStart();
            SetCloudsColor();
            SetFogEnd();
            SetOceanBaseColor();
            SetOceanReflectionColor();
            SetTerrainMultiplierColor();
            SetTerrainAdditionColor();
            SetTerrainEdgeColor();
            SetTerrainShadowColor();
            SetTerrainEnvironmentColor();
            SetTerrainShadowDistance();
            SetLightDirection();
            SetLightColor();
            SetSkyboxAdditiveSkyboxColor();
            SetSkyboxSunKeywordID();
            SetMoonAlpha();
            SetStarsAlpha();
            SetSkyboxAtmosphereThickness();
            SetSkyboxExposure();
            SetSkyboxSkyTint();
        }

        private void SetSkyboxSkyTint() => SkyboxMaterial.SetColor(PropertyIDSkyTint, SkyboxSkyTint);

        private void SetSkyboxExposure() => SkyboxMaterial.SetFloat(PropertyIDExposure, SkyboxExposure);

        private void SetSkyboxAtmosphereThickness() => SkyboxMaterial.SetFloat(PropertyIDAtmosphereThickness, SkyboxAtmosphereThickness);

        private void SetStarsAlpha() => StarsMaterial.SetFloat(PropertyIDAlpha, StarsAlpha);

        private void SetMoonAlpha() => MoonMaterial.SetFloat(PropertyIDAlpha, MoonAlpha);

        private void SetCloudsColor() => CloudsMaterial.SetColor(PropertyIDCloudsColor, CloudsColor);

        private void SetFogStart() => RenderSettings.fogStartDistance = FogStart;

        private void SetFogEnd() => RenderSettings.fogEndDistance = FogEnd;

        SkyboxSunKeywordID __skyboxSunKeywordIDLast;
        SkyboxSunKeywordID __skyboxSunKeywordIDCurrent;
        private void SetSkyboxSunKeywordID()
        {
            __skyboxSunKeywordIDCurrent = SkyboxSunKeywordID;
            Shader.DisableKeyword(__skyboxSunKeywordIDLast.ToString());
            Shader.EnableKeyword(__skyboxSunKeywordIDCurrent.ToString());
            __skyboxSunKeywordIDLast = __skyboxSunKeywordIDCurrent;
        }

        private void SetSkyboxAdditiveSkyboxColor() => SkyboxMaterial.SetColor(PropertyIDAdditiveColor, SkyboxAdditiveSkyboxColor);

        private void SetLightColor() => Shader.SetGlobalVector(PropertyIDLightColor, Light.color * Light.intensity);

        private void SetLightDirection() => Shader.SetGlobalVector(PropertyIDLightDirection, Light.transform.rotation * Vector3.back);

        private void SetTerrainShadowDistance() => Shader.SetGlobalFloat(PropertyIDTerrainShadowDistance, TerrainShadowDistance);

        private void SetTerrainEnvironmentColor() => Shader.SetGlobalColor(PropertyIDEnvironmentColor, TerrainEnvironmentColor);

        private void SetTerrainShadowColor() => Shader.SetGlobalColor(PropertyIDTerrainColorShadow, TerrainShadowColor);

        private void SetTerrainEdgeColor() => Shader.SetGlobalColor(PropertyIDTerrainColorEdge, TerrainEdgeColor);

        private void SetTerrainAdditionColor() => Shader.SetGlobalColor(PropertyIDTerrainColorAddition, TerrainAdditionColor);

        private void SetTerrainMultiplierColor() => Shader.SetGlobalColor(PropertyIDTerrainColorMultiplier, TerrainMultiplierColor);

        private void SetOceanReflectionColor() => Shader.SetGlobalColor(PropertyIDOceanColorReflection, OceanReflectionColor);

        private void SetOceanBaseColor() => Shader.SetGlobalColor(PropertyIDOceanColorBase, OceanBaseColor);

        private void SetFogColor() => RenderSettings.fogColor = FogColor;

        private void SetAmbientGroundColor() => RenderSettings.ambientGroundColor = GroundColor;

        private void SetAmbientEquatorColor() => RenderSettings.ambientEquatorColor = EquatorColor;

        private void SetAmbientSkyColor() => RenderSettings.ambientSkyColor = SkyColor;
    }
}
