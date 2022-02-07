using Core.Mapper;
using Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Audio
{
    public class EnvironmentAudioModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public EnvironmentTimeOfDayID EnvironmentTimeOfDayID;

            public void SetEnvironmentTimeOfDayID(EnvironmentTimeOfDayID environmentTimeOfDayID)
            {
                EnvironmentTimeOfDayID = environmentTimeOfDayID;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private EnvironmentAudioConfig[] _environmentAudioConfigs;

#pragma warning restore 0649
        #endregion

        public IEnumerable<EnvironmentAudioConfig> EnvironmentAudioConfigs => _environmentAudioConfigs;

        public EnvironmentTimeOfDayID EnvironmentTimeOfDayID
        {
            get => _data.EnvironmentTimeOfDayID;
            private set => _data.SetEnvironmentTimeOfDayID(value);
        }

        public event Action OnChangeEnvironmentTimeOfDayID;

        public EnvironmentTimeOfDayID GetEnvironmentTimeOfDayID(float timeOfDay)
        {
            foreach (var item in EnvironmentAudioConfigs)
            {
                if (TimeIntersect(timeOfDay, item.TimeStart, item.TimeEnd))
                {
                    return item.EnvironmentTimeOfDayID;
                }
            }

            return EnvironmentTimeOfDayID.None;
        }

        public void SetEnvironmentTimeOfDayID(EnvironmentTimeOfDayID environmentTimeOfDayID)
        {
            EnvironmentTimeOfDayID = environmentTimeOfDayID;
            OnChangeEnvironmentTimeOfDayID?.Invoke();
        }

        private static bool TimeIntersect(float time, float start, float end)
        {
            if (start > end)
            {
                return start <= time && time <= 24 || 0 <= time && time <= end;
            }

            return start <= time && time <= end;
        }

        //[OPTIMIZATION] add mapping
        public AudioID GetAudioID(EnvironmentTimeOfDayID environmentTimeOfDayID) => EnvironmentAudioConfigs.First(x => x.EnvironmentTimeOfDayID == environmentTimeOfDayID).AudioID;
    }

    [Serializable]
    public class EnvironmentAudioConfig
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private EnvironmentTimeOfDayID _environmentTimeOfDayID;
        [SerializeField] private AudioID _audioID;
        [SerializeField] private float _timeStart;
        [SerializeField] private float _timeEnd;

#pragma warning restore 0649
        #endregion

        public EnvironmentTimeOfDayID EnvironmentTimeOfDayID => _environmentTimeOfDayID;
        public AudioID AudioID => _audioID;
        public float TimeStart => _timeStart;
        public float TimeEnd => _timeEnd;
    }
}
