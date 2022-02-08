using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class QualityController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Shader _shaderVegetation;
        [SerializeField] private Shader _shaderEnvironment;
        [SerializeField] private Shader _shaderTools;

#pragma warning restore 0649
        #endregion

        private QualityModel QualityModel => ModelsSystem.Instance._qualityModel;
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;
        public Shader ShaderVegetation => _shaderVegetation;
        public Shader ShaderEnvironment => _shaderEnvironment;
        public Shader ShaderTools => _shaderTools;

        private int _nativeResolution = -1;
        private int _nativeResolutionOther = -1;

        private void OnEnable()
        {
            if (_nativeResolution < 0)
            {
                _nativeResolution = Mathf.Min(Screen.width, Screen.height);
                _nativeResolutionOther = Mathf.Max(Screen.width, Screen.height);
            }

            StorageModel.TryProcessing(QualityModel._Data);

            if (QualityModel.QualityID == QualityID.None)
            {
                QualityModel.SetQuality(QualityModel.PlatformQualityConfiguration.DefaultQuaityID);
            }
            else
            {
                QualityModel.SetQuality(QualityModel.QualityID);
            }

            bool targetFrameRateFirstInit = QualityModel.VSyncCount <= 0;
            if (QualityModel.VSyncNeverSeted)
            {
                QualityModel.SetVSyncCount(QualityModel.GetDefaultVSyncForSelectedQualityConfig());
            }

            SetupQualitySettings(QualityModel.PlatformQualityConfiguration.GetConfig(QualityModel.QualityID));
            SetUpVSCountAndFrameRate();
            SetUpAudioLevel(QualityModel.AudioLevel);

            QualityModel.OnChangeQuality += OnChangeQuaityHandler;
            QualityModel.OnChangeFrameRate += OnChagneTargetFrameRateHandler;
            QualityModel.OnChangeAudioLevel += OnAudioLevelChangedHandler;
        }

        private void OnDisable()
        {
            QualityModel.OnChangeQuality -= OnChangeQuaityHandler;
            QualityModel.OnChangeFrameRate -= OnChagneTargetFrameRateHandler;
            QualityModel.OnChangeAudioLevel -= OnAudioLevelChangedHandler;
        }

        private void OnChangeQuaityHandler()
        {
            SetupQualitySettings(QualityModel.QualityConfig);
        }

        private void OnChagneTargetFrameRateHandler()
        {
            SetUpVSCountAndFrameRate();
        }

        private void SetupQualitySettings(QualityConfig qualityConfig)
        {
            QualitySettings.SetQualityLevel((int)qualityConfig.QuaityID - 1, true);

            ShaderVegetation.maximumLOD = (int)qualityConfig.ShaderLODVegetationID;
            ShaderEnvironment.maximumLOD = (int)qualityConfig.ShaderLODEnvironmentID;
            ShaderTools.maximumLOD = (int)qualityConfig.ShaderLODToolsID;


            float t = qualityConfig.targetResolution / (float)_nativeResolution;
            if (qualityConfig.targetResolution == QualityConfig.kNativeResolution)
                t = 1f;
            if (t > 1f)
                t = 1f;

            // IMPORTANT: handle VSync before changing resolution to prevent bugs with target frame rate
            HandleVSyncAndMaxFrameRate();
            SetResolution(t);
            ChangeBattaryConsumptionIfLowSettings();

            void ChangeBattaryConsumptionIfLowSettings()
            {
                if (qualityConfig.QuaityID != QualityID.Low) return;

                var defaultVSyncValue = QualityModel.GetDefaultVSyncForSelectedQualityConfig();
                var selectedNowVSyncValue = QualityModel.VSyncCount;
                // example, default 2, selected value 0
                // 0 is more expensive
                bool selectedValueMoreExpensive = selectedNowVSyncValue < defaultVSyncValue;
                if (selectedValueMoreExpensive)
                    QualityModel.SetVSyncCount(QualityModel.GetDefaultVSyncForSelectedQualityConfig());
            }
        }

        private void SetUpVSCountAndFrameRate()
        {
            bool isPerformanceTest = EditorGameSettings.IsPerformanceTest;

            int vs = isPerformanceTest ? 0 : QualityModel.VSyncCount;
            int frameRate = isPerformanceTest ? 120 : QualityModel.TargetFrameRateData.FrameRate;
            SetUpVSCountAndFrameRate(vs,frameRate);
        }

        private void SetUpVSCountAndFrameRate(int vSyncCount, int frames)
        {
            Debug.Log("before: " + QualitySettings.vSyncCount + " " + Application.targetFrameRate);
            QualitySettings.vSyncCount = vSyncCount;
#if UNITY_IOS
                Application.targetFrameRate = frames;
#endif
            Debug.Log("after: " + QualitySettings.vSyncCount + " " + Application.targetFrameRate);

            Debug.Log("Ref rate: " + Screen.currentResolution.refreshRate);
            // HandleVSync();
        }


        private void HandleVSyncAndMaxFrameRate()
        {
            Application.targetFrameRate = 120;
            SetUpVSCountAndFrameRate();
        }

        private void OnAudioLevelChangedHandler()
        {
            SetUpAudioLevel(QualityModel.AudioLevel);
        }


        private void SetUpAudioLevel(float level)
        {
            AudioListener.volume = level;
        }

        void SetResolution(float t)
        {
            if (Screen.width > Screen.height)
            {
                Screen.SetResolution((int)(_nativeResolutionOther * t), (int)(_nativeResolution * t), true);
            }
            else
            {
                Screen.SetResolution((int)(_nativeResolution * t), (int)(_nativeResolutionOther * t), true);
            }
        }
    }
}
