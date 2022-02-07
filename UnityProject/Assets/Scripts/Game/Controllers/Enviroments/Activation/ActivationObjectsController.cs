using Core;
using Core.Controllers;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public abstract class ActivationObjectsController : MonoBehaviour
    {
        protected QualityModel QualityModel => ModelsSystem.Instance._qualityModel;

        private void OnEnable()
        {
            QualityModel.OnChangeQuality += OnChangeQuality;
        }

        private void Start()
        {
            InitActivationObjects(QualityModel.QualityConfig);
        }

        private void OnDisable()
        {
            QualityModel.OnChangeQuality -= OnChangeQuality;
        }

        private void OnChangeQuality()
        {
            ChangeActivationObjects(QualityModel.QualityConfig);
        }

        protected abstract void ChangeActivationObjects(QualityConfig qualityConfig);
        protected abstract void InitActivationObjects(QualityConfig qualityConfig);
    }
}
