using System;
using Core.Storage;
using Game.Features.DailyRewards.Storage;
using Game.Models;
using NaughtyAttributes;
using Reactive;
using UnityEngine;

namespace Game.Features.DailyRewards.Storage
{
    [Serializable]
    public class TimeInGameModelDataBase : DataBase
    {
        public override SaveTime TimeSave { get; } = SaveTime.Instantly;

        [SerializeField] private float _ticksSpentInGameToday;
        [SerializeField] private SerializableDateTime _rewardsUpdateTime;

        public float SecondsSpentInGameToday { get => _ticksSpentInGameToday; private set { WriteData(() => _ticksSpentInGameToday = value); } }
        public SerializableDateTime RewardsUpdateTime { get => _rewardsUpdateTime; set { WriteData(() => _rewardsUpdateTime = value); } }

        public void AdjustTimeInGame(float value) => SecondsSpentInGameToday += value;
        public void ResetProgress()
        {
            RewardsUpdateTime = new SerializableDateTime(DateTime.UtcNow);
            SecondsSpentInGameToday = 0;
        }
    }
}

namespace Game.Features.DailyRewards.Models
{
    public class TimeInGameModel : ModelBase<TimeInGameModelDataBase>
    {
        [SerializeField] private float debugMultiplier = 2f;
        private bool isActiveDebugMultiplier = false;

        private RealTimeModel RealTimeModel => ModelsSystem.Instance._realTimeModel;

        public bool IsTrackTime { get; private set; }

        public int Seconds => (int)DataBase.SecondsSpentInGameToday;
        public int Minutes => (int)(DataBase.SecondsSpentInGameToday / 60);

        public DateTime StartedTrackingProgress => DataBase.RewardsUpdateTime.DateTime;

        public event Action OnProgressReset;

        public void StartTrackTime() => IsTrackTime = true;
        public void StopTrackTime() => IsTrackTime = false;

        public void AdjustTimeInGame(float value)
        {
            var formattedValue = isActiveDebugMultiplier ? debugMultiplier * value : value;
            DataBase.AdjustTimeInGame(formattedValue);
            TryResetProgress();
        }

        private bool TryResetProgress()
        {
            if(!RealTimeModel) return false;
            if(!RealTimeModel.isReady) return false;

            DateTime now = RealTimeModel.Now();
            DateTime lastRewardsUpdate = StartedTrackingProgress;

            if (now.DayOfYear > lastRewardsUpdate.DayOfYear)
            {
                ResetProgress();
            }
            else if (now.Year > lastRewardsUpdate.Year)
            {
                ResetProgress();
            }

            return true;

        }

        private void ResetProgress()
        {
            DataBase.ResetProgress();
            DataBase.RewardsUpdateTime = new SerializableDateTime(RealTimeModel.Now());
            OnProgressReset?.Invoke();
        }


        [Button]
        private void Test_ResetProgress() => ResetProgress();

        [Button]
        private void Test_SwitchDebugTimer() => isActiveDebugMultiplier = !isActiveDebugMultiplier;
    }
}
