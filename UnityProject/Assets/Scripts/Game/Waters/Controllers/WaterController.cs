using System;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class WaterController : MonoBehaviour
    {
        public int PropertyIDLightDirection { get; } = Shader.PropertyToID("_LightDirection");

        #region Data
#pragma warning disable 0649

        [SerializeField] private WaterConfig[] _waterConfigs;
        [SerializeField] private WaterQualityProvider _waterQualityProvider;
        [SerializeField] private WaterReflectionController _mirrorReflection;
        [SerializeField] private Transform _sunObject;
        [SerializeField] private Camera _cameraWorld;

#pragma warning restore 0649
        #endregion

        private QualityModel QualityModel => ModelsSystem.Instance._qualityModel;
        private WaterConfig[] WaterConfigs => _waterConfigs;
        private WaterQualityProvider WaterQualityProvider => _waterQualityProvider;
        private WaterReflectionController MirrorReflection => _mirrorReflection;
        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;
        public Transform SunObject => _sunObject;

        private Camera CameraWorld => _cameraWorld;
        private WorldCameraModel WorldCameraModel => ModelsSystem.Instance._worldCameraModel;
        private CoreEnvironmentModel CoreEnvironmentModel => ModelsSystem.Instance._coreEnvironmentModel;

        private Camera TargetCamera { get; set; }
        private DepthTextureMode DepthTextureModeLast { get; set; }

        #region MonoBehaviour
        private void OnEnable()
        {
            WorldCameraModel.OnChangeCamera += OnChangeCameraHandler;
            QualityModel.OnChangeQuality += OnChangeQuaityHandler;
            CoreEnvironmentModel.OnWaterActiveChanged += OnWaterActiveChanged;
        }

        private void Start()
        {
            OnWaterActiveChanged();
            UpdateWaterQuality();
        }

        private void OnDisable()
        {
            WorldCameraModel.OnChangeCamera -= OnChangeCameraHandler;
            QualityModel.OnChangeQuality -= OnChangeQuaityHandler;
            GameUpdateModel.OnUpdate -= OnUpdateSunGlossines;
            CoreEnvironmentModel.OnWaterActiveChanged -= OnWaterActiveChanged;
        }

        #endregion
        private void OnWaterActiveChanged()
        {
            bool showWater = CoreEnvironmentModel.WaterActive;

            foreach (var waterConfig in WaterConfigs)
            {
                waterConfig.Renderer.gameObject.SetActive(showWater);
            }
        }


        private void OnUpdateSunGlossines()
        {
            foreach (var water in WaterConfigs)
            {
                water.Renderer.material.SetVector(PropertyIDLightDirection, SunObject.rotation * Vector3.back);
            }
        }

        private void OnChangeCameraHandler()
        {
            SetWaterQuality(QualityModel.QualityConfig.WaterQualitySettingsID);
            UpdateWaterQuality();
        }

        private void OnChangeQuaityHandler() => UpdateWaterQuality();

        private void UpdateWaterQuality() => SetWaterQuality(QualityModel.QualityConfig.WaterQualitySettingsID);

        private void SetWaterQuality(WaterQualitySettingsID waterConfigID)
        {
            var config = WaterQualityProvider[waterConfigID];

            if (config.IsHasReflctions)
            {
                MirrorReflection.SetTargetCamera(WorldCameraModel.WorldCamera);
                MirrorReflection.SetRenderTextureSize(config.SizeWaterReflectionTexture);
                MirrorReflection.enabled = true;
            }
            else
            {
                MirrorReflection.SetTargetCamera(null);
                MirrorReflection.enabled = false;
            }

            foreach (var water in WaterConfigs)
            {
                var quality = water.QualityProvider[waterConfigID];
                water.Renderer.material = quality.Material;
            }

            if (config.IsHasSunGlossines)
            {
                GameUpdateModel.OnUpdate -= OnUpdateSunGlossines;
                GameUpdateModel.OnUpdate += OnUpdateSunGlossines;
            }
            else
            {
                GameUpdateModel.OnUpdate -= OnUpdateSunGlossines;
            }

            SetTargetCamera(WorldCameraModel.WorldCamera, config.IsHasDepth);
        }

        private void SetTargetCamera(Camera camera, bool isDepth)
        {
            if (TargetCamera != null)
            {
                TargetCamera.depthTextureMode = DepthTextureModeLast;
            }

            if (isDepth)
            {
                if (camera != null)
                {
                    DepthTextureModeLast = camera.depthTextureMode;
                    camera.depthTextureMode = DepthTextureMode.Depth;
                }
            }

            TargetCamera = camera;
        }
    }
}
