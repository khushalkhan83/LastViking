using System;
using UnityEngine;
using UnityEngine.UI;

namespace PerfomanceTesting
{
    public class QualityControl : MonoBehaviour
    {
        [SerializeField] private Button increaseButton;
        [SerializeField] private Button decreaseButton;

        [SerializeField] private Slider lodBiaseSlider;
        [SerializeField] private Text loadBiaseTextField;

        [SerializeField] private Slider shadowsSlider;
        [SerializeField] private Text shadowsDistanceValue;

        [SerializeField] private Slider maxLodLevelSlider;
        [SerializeField] private Text maxLodLevelValue;

        [Header("Screen size")]
        [SerializeField] private Button lowScreenSize;
        [SerializeField] private Button midScreenSize;
        [SerializeField] private Button highScreenSize;

        void Awake()
        {
            // Application.targetFrameRate = -1;
            // QualitySettings.vSyncCount = 0;
            // Application.targetFrameRate = 60;
            SetFrameRate();
        }


        private void OnEnable()
        {
            increaseButton.onClick.AddListener(IncreaseQuality);
            decreaseButton.onClick.AddListener(DecreaseQuality);

            lodBiaseSlider.onValueChanged.AddListener(OnLoadBiasChanged);
            lodBiaseSlider.value = QualitySettings.lodBias;

            shadowsSlider.onValueChanged.AddListener(OnShadowsChanged);
            shadowsSlider.value = QualitySettings.shadowDistance;

            maxLodLevelSlider.onValueChanged.AddListener(OnMaxLodLevelChanged);
            maxLodLevelSlider.value = QualitySettings.maximumLODLevel;

            lowScreenSize.onClick.AddListener(LowScreenSizePreset);
            midScreenSize.onClick.AddListener(MidScreenSizePreset);
            highScreenSize.onClick.AddListener(HighScreenSizePreset);
        }

        private void OnDisable()
        {
            increaseButton.onClick.RemoveAllListeners();
            decreaseButton.onClick.RemoveAllListeners();

            lodBiaseSlider.onValueChanged.RemoveAllListeners();
            shadowsSlider.onValueChanged.RemoveAllListeners();

            lowScreenSize.onClick.RemoveAllListeners();
            midScreenSize.onClick.RemoveAllListeners();
            highScreenSize.onClick.RemoveAllListeners();
        }

        private void DecreaseQuality()
        {
            QualitySettings.DecreaseLevel();
        }

        private void IncreaseQuality()
        {
            QualitySettings.IncreaseLevel();
        }

        private void OnLoadBiasChanged(string newValue)
        {
            var newBias = decimal.Parse(newValue);
            QualitySettings.lodBias = (float)newBias;
        }

        private void OnShadowsChanged(float shadowsDistance)
        {
            QualitySettings.shadowDistance = shadowsDistance;
            shadowsDistanceValue.text = shadowsDistance.ToString();
        }

        private void OnLoadBiasChanged(float lodBias)
        {
            QualitySettings.lodBias = lodBias;
            loadBiaseTextField.text = lodBias.ToString();
        }

        private void OnMaxLodLevelChanged(float maxLodLevel)
        {
            QualitySettings.maximumLODLevel = (int)maxLodLevel;
            maxLodLevelValue.text = maxLodLevel.ToString();
        }

        private void SetFrameRate()
        {
            // Application.targetFrameRate = -1;
            // QualitySettings.vSyncCount = 0;
            // #if UNITY_IOS 
            //     Application.targetFrameRate = frameRate;
            // #endif

            Application.targetFrameRate = -1;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        private void LowScreenSizePreset()
        {
            Screen.SetResolution(640,480,true);
            SetFrameRate();
            OnQualityChanged?.Invoke(new PresetChangedMessage(Preset.Low));
        }

        private void MidScreenSizePreset()
        {
            Screen.SetResolution(1280,720,true);
            SetFrameRate();
            OnQualityChanged?.Invoke(new PresetChangedMessage(Preset.Mid));
        }

        private void HighScreenSizePreset()
        {
            Screen.SetResolution(1920,1080,true);
            SetFrameRate();
            OnQualityChanged?.Invoke(new PresetChangedMessage(Preset.High));
        }

        public event Action<PresetChangedMessage> OnQualityChanged;

        public class PresetChangedMessage
        {
            public readonly Preset qualityPreset;

            public PresetChangedMessage(Preset qualityPreset)
            {
                this.qualityPreset = qualityPreset;
            }
        }

        public enum Preset {Low,Mid,High}
    }
}