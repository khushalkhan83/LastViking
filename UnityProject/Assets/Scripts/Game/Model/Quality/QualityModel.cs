using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class QualityModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase, IImmortal
        {
            public QualityID QualityID;

            public void SetQualityID(QualityID qualityId)
            {
                QualityID = qualityId;
                ChangeData();
            }

            public int VSyncCount = -1;

            public bool VSyncMustBeSetup => VSyncCount == -1;

            public void SetVSyncCount(int vSyncCount)
            {
                VSyncCount = vSyncCount;
                ChangeData();
            }

            public float AudioLevel = 1f;

            public void SetAudioLevel(float audioLevel)
            {
                AudioLevel = audioLevel;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private PlatformQualityConfiguration _androidQualityConfiguration;
        [SerializeField] private PlatformQualityConfiguration _iosQualityConfiguration;
        [SerializeField] private FrameRateChainDataProvider _frameRateProvider;

#pragma warning restore 0649
        #endregion

        public Data _Data => _data;

        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;


        public QualityID QualityID
        {
            get
            {
                return _data.QualityID;
            }
            private set
            {
                _data.SetQualityID(value);
            }
        }


        public int VSyncCount
        {
            get => _data.VSyncCount;
            set => _data.SetVSyncCount(value);
        }

        public bool VSyncNeverSeted => _data.VSyncMustBeSetup;

        public string VSyncText => _frameRateProvider.GetData(VSyncCount).Text;

        public Sprite TargetFrameRateIcon
        {
            get { return _frameRateProvider.GetData(VSyncCount).Icon; }
        }

        public FrameRateData TargetFrameRateData => _frameRateProvider.GetData(VSyncCount);
        public int VSyncRateWithHigestFPSPossible => _frameRateProvider.MaxFrameRateData.VSyncCount;

        public int GetDefaultVSyncForSelectedQualityConfig() => QualityConfig.DefaultVSync;

        public float AudioLevel
        {
            get => _data.AudioLevel;
            private set => _data.SetAudioLevel(value);
        }

        public PlatformQualityConfiguration PlatformQualityConfiguration =>
#if UNITY_ANDROID
            _androidQualityConfiguration
#else
            _iosQualityConfiguration
#endif
            ;

        private QualityConfig _qualityConfig;
        public QualityConfig QualityConfig
        {
            get
            {
#if UNITY_EDITOR
                if (_qualityConfig == null)
                {
                    _qualityConfig = PlatformQualityConfiguration.GetConfig(QualityID.Low);
                }
#endif
                return _qualityConfig;
            }
            private set
            {
                _qualityConfig = value;
            }
        }
        public event Action OnChangeQuality;

        public void IncreaseQuality()
        {
            if (QualityID < QualityID.High)
            {
                SetQuality(QualityID + 1);
            }
        }

        public void DecreaseQuality()
        {
            if (QualityID > QualityID.Low)
            {
                SetQuality(QualityID - 1);
            }
        }

        public void SetQuality(QualityID qualityID)
        {
            var last = QualityID;
            QualityID = qualityID;
            QualityConfig = PlatformQualityConfiguration.GetConfig(qualityID);

            if (qualityID != last)
            {
                OnChangeQuality?.Invoke();
            }
        }

        public event Action OnChangeFrameRate;

        public void IncreaseFrameRate()
        {
            var nextVSyncCount = _frameRateProvider.GetNextData(VSyncCount).VSyncCount;
            SetVSyncCount(nextVSyncCount);
        }

        public void DecreaseFrameRate()
        {
            var previouseVSyncCount = _frameRateProvider.GetPreviousData(VSyncCount).VSyncCount;
            SetVSyncCount(previouseVSyncCount);
        }

        public void SetVSyncCount(int vSyncCount)
        {
            int performanceTestVSyncTargetValue = 0;
            
            if (EditorGameSettings.IsPerformanceTest)
                vSyncCount = performanceTestVSyncTargetValue;

            VSyncCount = vSyncCount;

            if (vSyncCount != performanceTestVSyncTargetValue)
                OnChangeFrameRate?.Invoke();
        }

        public event Action OnChangeAudioLevel;

        public void SetAudioLevel(float level)
        {
            AudioLevel = level;
            OnChangeAudioLevel?.Invoke();
        }

    }
}
