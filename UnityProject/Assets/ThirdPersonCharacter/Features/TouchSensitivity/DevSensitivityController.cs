using Game.ThirdPerson.TouchSensitivity.Interfaces;
using UnityEngine;
using PerfomanceTesting;
using PresetChangedMessage = PerfomanceTesting.QualityControl.PresetChangedMessage;
using Preset = PerfomanceTesting.QualityControl.Preset;

namespace Game.ThirdPerson.TouchSensitivity
{
    public class DevSensitivityController : MonoBehaviour
    {
        [SerializeField] private QualityControl qualityControl;

        private ITouchSensitivity touchSensitivity;

        #region MonoBehaviour
        private void Awake()
        {
            touchSensitivity = GetComponent<ITouchSensitivity>();
        }

        private void OnEnable()
        {
            qualityControl.OnQualityChanged += OnQualityChanged;
        }

        private void OnDisable()
        {
            qualityControl.OnQualityChanged -= OnQualityChanged;
        }
        #endregion

        private void OnQualityChanged(PresetChangedMessage message)
        {
            SensativityPreset preset;

            switch (message.qualityPreset)
            {
                case Preset.Low:
                    preset = SensativityPreset.Low;
                    break;
                case Preset.Mid:
                    preset = SensativityPreset.Mid;
                    break;
                case Preset.High:
                    preset = SensativityPreset.High;
                    break;
                default:
                    preset = SensativityPreset.High;
                    break;
            }

            touchSensitivity.SetResolutionPreset(preset);
        }
    }
}