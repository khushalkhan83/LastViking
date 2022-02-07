using Core.Views;
using Game.Models;
using Game.Progressables;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class DebugTimeView : ViewBase
    {

        #region Data
#pragma warning disable 0649

        [SerializeField] private Slider _timeline;
        [SerializeField] private Text _buttonText;
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _waterfallResetTime;

#pragma warning restore 0649
        #endregion

        private ulong ticks;
        private ulong envTicks;
        private double lastValue;

        public bool IsStopedTime { get; private set; }
        public bool IsDrag { get; private set; }

        private void SetButtonText(string value) => _buttonText.text = value;
        private void SetTitleText(string value) => _titleText.text = value;
        private void SetWaterfallResetTime(string value) => _waterfallResetTime.text = value;

        //Dev Mode
        public event Action<bool> OnChangeGodMode;
        public event Action OnAdjustHealth;
        public event Action OnAdjustHunger;
        public event Action OnAdjustWater;
        public event Action OnKillPlayer;
        public event Action OnAddCoins;
        public event Action OnBuildShelter;
        public event Action OnKillShelter;
        public event Action<bool> OnSwitchControllerActions;
        public event Action OnSkipTutorial;
        public event Action OnNextTier;
        public event Action OnTimeNormal;
        public event Action OnTimeFaster;

        public void ActionChangeGodMode(bool IsGodMode) => OnChangeGodMode?.Invoke(IsGodMode);
        public void ActionAdjustHealth() => OnAdjustHealth?.Invoke();
        public void ActionAdjustHunger() => OnAdjustHunger?.Invoke();
        public void ActionAdjustWater() => OnAdjustWater?.Invoke();
        public void ActionKillPlayer() => OnKillPlayer?.Invoke();
        public void ActionAddCoins() => OnAddCoins?.Invoke();
        public void ActionBuildShelter() => OnBuildShelter?.Invoke();
        public void ActionKillShelter() => OnKillShelter?.Invoke();
        public void ActionSwitchControllerActions(bool IsKeyboard) => OnSwitchControllerActions?.Invoke(IsKeyboard);
        public void ActionSkipTutorial() => OnSkipTutorial?.Invoke();
        public void ActionNextTier() => OnNextTier?.Invoke();
        public void ActionTimeNormal() => OnTimeNormal?.Invoke();
        public void ActionTimeFaster() => OnTimeFaster?.Invoke();

        //Dev Mode


        
        private GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;
        private WaterfallProgressModel WaterfallProgressModel => ModelsSystem.Instance._waterfallProgressModel;

        float Value
        {
            get => _timeline.value;
            set => _timeline.value = value;
        }

        public float TimeOfDayNormalized
        {
            get => (float)GameTimeModel.TimeOfDayTicks / GameTimeModel.DayDurationTicks;
            set => GameTimeModel.Ticks = GameTimeModel.Days * GameTimeModel.DayDurationTicks + (ulong)((double)value * GameTimeModel.DayDurationTicks);
        }

        void Update()
        {
            if (!(IsDrag && IsStopedTime))
            {
                Value = TimeOfDayNormalized;
            }

            var tickTime = $"{GameTimeModel.Hours:00h}:{GameTimeModel.Minutes:00m} {(GameTimeModel.Days + 1):00 days}";
            var envTime = $"{GameTimeModel.EnviroHours:00h}:{GameTimeModel.EnviroMinutes:00m} {(GameTimeModel.EnviroDays + 1):00 env days}";
            SetTitleText($"Time: {tickTime}, env: {envTime}");

            if (WaterfallProgressModel.ProgressStatus == ProgressStatus.WaitForResetProgress)
            {
                var dif = WaterfallProgressModel.NextProgressResetTime - GameTimeModel.RealTimeNowTick;
                SetWaterfallResetTime($"Waterfall reset time: {GameTimeModel.GetSecondsTotal(dif):00}");
            }
        }

        //UI
        public void SetTime(float time)
        {
            TimeOfDayNormalized = Mathf.Clamp01(time);
        }

        //UI
        public void BeginDrag()
        {
            IsDrag = true;
            GameTimeModel.enabled = false;

            ticks = GameTimeModel.Ticks;
            envTicks = GameTimeModel.EnviroTicks;
            lastValue = Value;
        }

        //UI
        public void Drag()
        {
            GameTimeModel.Ticks = (ulong) (ticks + (Value - lastValue) * GameTimeModel.DayDurationTicks);
            GameTimeModel.EnviroTicks = (ulong) (envTicks + (Value - lastValue) * GameTimeModel.DayDurationTicks);
        }

        //UI
        public void EndDrag()
        {
            IsDrag = false;
            GameTimeModel.enabled = true;
        }

        //UI
        public void StopTimeButton()
        {
            IsStopedTime = !IsStopedTime;
            if (IsStopedTime)
            {
                SetButtonText("Play");
                GameTimeModel.PauseEnviroTime();
            }
            else
            {
                SetButtonText("Stop");
                GameTimeModel.UnpauseEnviroTime();
            }
        }

    }
}
