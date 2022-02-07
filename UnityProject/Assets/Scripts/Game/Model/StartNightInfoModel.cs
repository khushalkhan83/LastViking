using CodeStage.AntiCheat.ObscuredTypes;
using Game.Providers;
using System;
using UnityEngine;

namespace Game.Models
{
    public class StartNightInfoModel : MonoBehaviour
    {
#pragma warning disable 0649

        [SerializeField] private ObscuredFloat _timeShowEnd;
        [SerializeField] private ObscuredFloat _messageShowDelay;

        [SerializeField, Tooltip("In game hours")] private float _showAttackMessagePredelay = 1;

        [SerializeField] private WaveInfoProvider WaveInfos;

#pragma warning restore 0649

        public float TimeShowEnd => _timeShowEnd;
        public float MessageShowDelay => _messageShowDelay;
        public float ShowAttackMessagePredelay => _showAttackMessagePredelay;

        public event Action OnStartNight;
        public event Action OnEndNight;
        public event Action OnEndShow;

        public float TimeShowRemaining { get; private set; }

        public WaveInfoData GetWaveInfoData(WaveInfoId waveInfoId) => WaveInfos[waveInfoId];

        public void BeginShowProcess()
        {
            TimeShowRemaining = TimeShowEnd;
        }

        public void ShowProcess(float deltaTime)
        {
            TimeShowRemaining -= deltaTime;

            if (TimeShowRemaining < 0)
            {
                TimeShowRemaining = 0;
            }

            if (TimeShowRemaining == 0)
            {
                OnEndShow?.Invoke();
            }
        }

        public void StartNight() => OnStartNight?.Invoke();
        public void EndNight() => OnEndNight?.Invoke();
    }
}
