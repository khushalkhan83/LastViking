using System.Collections;
using System.Collections.Generic;
using Extensions;
using Game.Models;
using UnityEngine;


namespace Game.Components
{

    public class EffectQuality : MonoBehaviour
    {
        [System.Serializable]
        public class EffectSettings
        {
            [SerializeField] private QualityID qualityID;
            [SerializeField] private List<GameObject> activate = new List<GameObject>();
            [SerializeField] private List<GameObject> deactivate = new List<GameObject>();

            public QualityID QualityID => qualityID;

            public void Apply()
            {
                foreach (var item in activate)
                {
                    item.CheckNull()?.SetActive(true);
                }
                foreach (var item in deactivate)
                {
                    item.CheckNull()?.SetActive(false);
                }
            }
        }
        
        #region Data
        [SerializeField] private EffectSettings defaultSettings = default;
        [SerializeField] private List<EffectSettings> settings = new List<EffectSettings>();
        #endregion

        private QualityModel QualityModel => ModelsSystem.Instance._qualityModel;

        #region MonoBehaviour

        private void Awake()
        {
            QualityModel.OnChangeQuality += UpdateView;
            UpdateView();
        }

        private void OnDestroy()
        {
            QualityModel.OnChangeQuality -= UpdateView;
        }

        private void OnValidate()
        {
            // TODO: add logic to check that all settings are present
        }

        #endregion

        private void UpdateView()
        {
            var match = settings.Find(x => x.QualityID == QualityModel.QualityID);

            // TODO: move to on validate check logic
            if (match == null)
            {
                match = defaultSettings;
            }

            match.Apply();
        }
    }
}