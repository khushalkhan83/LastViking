using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    public class GameTimeModel : MonoBehaviour
    {
        //TicksPerSecond = 10000000;

        private const float SCALE_STOP = 0;
        private const float SCALE_NORMAL = 1;

        [Serializable]
        public class Data : DataBase
        {
            public ObscuredULong Ticks;
            public ObscuredULong EnviroTicks;

            public void SetTicks(ulong ticks)
            {
                Ticks = ticks;
                ChangeData();
            }

            public void SetEnviroTicks(ulong ticks)
            {
                EnviroTicks = ticks;
                ChangeData();
            }
        }

        [Serializable]
        public class ResetData : DataBase
        {
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private ResetData _resetData;

        [SerializeField] private ObscuredULong _dayDuration;
        [SerializeField] private ObscuredULong _startTicks;
        [SerializeField] private ObscuredULong _maxOneUpdateTicks;
        [SerializeField] private int _dayStartHour = 6;
        [SerializeField] private int _dayEndHour = 20;

#pragma warning restore 0649
        #endregion

        public ulong DayDurationTicks => _dayDuration;
        public ulong StartTicks => _startTicks;

        public float Scale
        {
            get => Time.timeScale;
            private set => Time.timeScale = value;
        }

        public ulong Ticks
        {
            get => _data.Ticks + StartTicks;
            set => _data.SetTicks(value - StartTicks);
        }

        public ulong EnviroTicks
        {
            get => _data.EnviroTicks + StartTicks;
            set => _data.SetEnviroTicks(value - StartTicks);
        }

        public ulong GameDurationTicks => _data.Ticks;
        public ulong RawEnviroTicks => _data.EnviroTicks;

        public ulong Days => GetDays(Ticks);
        public ulong Hours => GetHours(Ticks);
        public ulong Minutes => GetMinutes(Ticks);
        public ulong Seconds => GetSeconds(Ticks);
        public ulong Miliseconds => GetMiliseconds(Ticks);
        public float TimeOfDay => GetTimeOfDay(TimeOfDayTicks);

        public ulong GetDays(ulong ticks) => ticks / DayDurationTicks;
        public ulong GetHours(ulong ticks) => (ticks / (DayDurationTicks / 24)) % 24;
        public ulong GetMinutes(ulong ticks) => (ticks / (DayDurationTicks / 1440)) % 60;
        public ulong GetSeconds(ulong ticks) => (ticks / (DayDurationTicks / 86400)) % 60;
        public ulong GetMiliseconds(ulong ticks) => (ticks / (DayDurationTicks / 86400000)) % 1000;
        public float GetTimeOfDay(ulong ticks) => (float)((double)ticks / DayDurationTicks) * 24;

        public ulong GetHourTicks(float hours) => (ulong)(hours / 24 * DayDurationTicks);

        public ulong TimeOfDayTicks => Ticks % DayDurationTicks;
        public long RealTimeNowTick => DateTime.UtcNow.Ticks;
        public ulong TicksLast { get; private set; }
        public ulong DayLast { get; private set; }
        public long TicksRealLast { get; private set; }
        public long TicksRealNow { get; private set; }

        public ulong EnviroDays => GetDays(EnviroTicks);
        public ulong EnviroHours => GetHours(EnviroTicks);
        public ulong EnviroMinutes => GetMinutes(EnviroTicks);
        public float EnviroTimeOfDay => GetTimeOfDay(EnviroTimeOfDayTicks);

        public ulong EnviroTicksLast { get; private set; }
        public ulong EnviroTimeOfDayTicks => EnviroTicks % DayDurationTicks;
        public long EnviroTicksRealLast { get; private set; }
        public long EnviroTicksRealNow { get; private set; }
        public bool IsDayNow { get; private set; }

        public event Action OnChangeDay;
        public event Action OnChangeDayNight;
        //

        public long GetTicks(float seconds) => TimeSpan.FromSeconds(seconds).Ticks;
        public float GetSecondsTotal(long ticks) => (float)TimeSpan.FromTicks(ticks).TotalSeconds;

        public StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        public void UpdateProcess(float deltaTime)
        {
            TicksLast = Ticks;
            TicksRealNow = RealTimeNowTick;

            ulong updateTicks = (ulong)((TicksRealNow - TicksRealLast) * Scale);
            if (updateTicks > _maxOneUpdateTicks)
                updateTicks = _maxOneUpdateTicks;
            else if (updateTicks < 0)
                updateTicks = 0;

            Ticks += updateTicks;

            if (Days != DayLast)
            {
                OnChangeDay?.Invoke();
            }

            TicksRealLast = RealTimeNowTick;
            DayLast = Days;

            if (RemainingFadeTime > 0)
            {
                RemainingFadeTime -= deltaTime;

                if (RemainingFadeTime < 0)
                {
                    RemainingFadeTime = 0;
                }

                Scale = Mathf.Lerp(FadeScaleFrom, FadeScaleTo, 1 - RemainingFadeTime / FadeTime);
            }

            UpdateDayNightTime();
        }

        public void EnviroUpdateProcess(float deltaTime)
        {
            EnviroTicksLast = EnviroTicks;
            EnviroTicksRealNow = RealTimeNowTick;

            ulong updateTicks = (ulong)((EnviroTicksRealNow - EnviroTicksRealLast) * Scale);
            if (updateTicks > _maxOneUpdateTicks)
                updateTicks = _maxOneUpdateTicks;
            else if (updateTicks < 0)
                updateTicks = 0;

            EnviroTicks += updateTicks;

            EnviroTicksRealLast = RealTimeNowTick;
        }

        public bool IsInGame { get; private set; } = true;
        public bool IsOutGame => !IsInGame;

        public bool IsEnviroTimePaused { get; private set; }

        public void StartGame() => IsInGame = true;
        public void StopGame() => IsInGame = false;

        public void InGame()
        {
            if (IsInGame) return;

            TicksRealLast = RealTimeNowTick;
            ScaleRestore();
            StartGame();
        }

        public void OutGame()
        {
            ScaleSave();
            ScaleStop();
            StopGame();
        }

        public void SetGameOnPause(bool pause)
        {
            if (pause) OutGame();
            else InGame();
        }

        public void PauseEnviroTime()
        {
            IsEnviroTimePaused = true;
        }
        public void UnpauseEnviroTime()
        {
            IsEnviroTimePaused = false;
            EnviroTicksRealLast = RealTimeNowTick;
        }

        public Stack<float> StackScales { get; } = new Stack<float>();

        public float RemainingFadeTime { get; private set; }
        public float FadeScaleFrom { get; private set; }
        public float FadeScaleTo { get; private set; }
        public float FadeTime { get; private set; }

        public void ScaleReset()
        {
            StackScales.Clear();
            ScaleNormal();
        }

        public void ScaleSave()
        {
            StackScales.Push(Scale);
        }

        public void ScaleRestore(float time = 0)
        {
            if (StackScales.Count > 0)
            {
                ScaleTo(StackScales.Pop(), time);
            }
            else
            {
                ScaleNormal();
            }
        }

        public void ScaleNormal(float time = 0) => ScaleTo(SCALE_NORMAL, time);

        public void ScaleStop(float time = 0) => ScaleTo(SCALE_STOP, time);

        public void ScaleTo(float to, float time = 0)
        {
            if (to < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(to));
            }

            if (time < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(time));
            }

            if (time == 0)
            {
                RemainingFadeTime = 0;
                FadeTime = 0;
                FadeScaleFrom = to;
                FadeScaleTo = to;

                Scale = to;
            }
            else
            {
                RemainingFadeTime = time;
                FadeTime = time;
                FadeScaleFrom = Scale;
                FadeScaleTo = to;
            }
        }

        private void UpdateDayNightTime()
        {
            bool isDay = (int)Hours >= _dayStartHour && (int)Hours < _dayEndHour;
            SetIsDayNow(isDay);
        }

        private void SetIsDayNow(bool value)
        {
            if(IsDayNow != value)
            {
                IsDayNow = value;
                OnChangeDayNight?.Invoke();
            }
        }

        private void Awake()
        {
            TicksRealLast = RealTimeNowTick;
            TicksRealNow = RealTimeNowTick;

            StorageModel.TryProcessing(_data);
            if (RawEnviroTicks == default)
            {
                EnviroTicks = Ticks;
            }

            DayLast = Days;
            ScaleReset();
        }
    }
}
