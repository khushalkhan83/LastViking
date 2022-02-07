using System;
using Game.Models;
using Game.ThirdPerson.TouchSensitivity.Interfaces;
using UnityEngine;

namespace Game.ThirdPerson.TouchSensitivity.Tests
{
    public class TouchSensitivityController : MonoBehaviour
    {
        private ITouchSensitivity touchSensitivity;

        private QualityModel qualityModel;
        private TouchpadModel touchpadModel;

        #region MonoBehaviour
        private void Awake()
        {
            touchSensitivity = GetComponent<ITouchSensitivity>();

            qualityModel = ModelsSystem.Instance._qualityModel;
            touchpadModel = ModelsSystem.Instance._touchpadModel;
        }

        private void Start()
        {
            HandleSensitivityChange();
            HandleScreenSizeChange();
        }

        private void OnEnable()
        {
            qualityModel.OnChangeQuality += HandleScreenSizeChange;
            touchpadModel.OnSensativityModificatorChanged += HandleSensitivityChange;
        }

        private void OnDisable()
        {
            qualityModel.OnChangeQuality -= HandleScreenSizeChange;
            touchpadModel.OnSensativityModificatorChanged -= HandleSensitivityChange;
        }

        private void HandleSensitivityChange()
        {
            touchSensitivity.Sensitivity = touchpadModel.SensativityModificator;
        }
        #endregion

        private void HandleScreenSizeChange()
        {
            SensativityPreset preset;

            switch (qualityModel.QualityID)
            {
                case QualityID.Low:
                    preset = SensativityPreset.Low;
                    break;
                case QualityID.Medium:
                    preset = SensativityPreset.Mid;
                    break;
                case QualityID.High:
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