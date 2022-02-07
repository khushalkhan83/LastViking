using System;
using System.Collections;
using Core.Storage;
using Game.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Game.OpenWorld.EnemiesCamp
{
    public class EnemiesCampModel : InitableModel<EnemiesCampModel.Data>
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private Data _data;
        [SerializeField] private UnityEvent _onZoneCleared;
        [SerializeField] private float _durationRespawn;
#pragma warning restore 0649
        #endregion
        protected override Data DataBase => _data;

        #region Dependencies
        private GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;
        #endregion
        

        [Serializable]
        public class Data : DataBase
        {
            [SerializeField] private bool isCleared;
            [SerializeField] private int counter;
            [SerializeField] private long timeSpawnTicks;
            
            public bool IsCleared
            {
                get => isCleared;
                set {isCleared = value; ChangeData();}
            }
            public int Counter
            {
                get => counter;
                set {counter = value; ChangeData();}
            }
            public long TimeSpawnTicks
            {
                get => timeSpawnTicks;
                set {timeSpawnTicks = value; ChangeData();}
            }
        }

        public bool IsCleared
        {
            get => _data.IsCleared;
            private set => _data.IsCleared = value;
        }

        public int Counter
        {
            get => _data.Counter;
            private set => _data.Counter = value;
        }

        public long TimeSpawnTicks
        {
            get
            {
                return _data.TimeSpawnTicks;
            }
            private set
            {
                _data.TimeSpawnTicks = value;
            }
        }

        public bool EnemiesActive {get => GameTimeModel.RealTimeNowTick >= TimeSpawnTicks;}
        public bool TryGetEnemiesCooldownTime(out float seconds)
        {
            seconds = 0;

            if(EnemiesActive) return false;

            seconds = GameTimeModel.GetSecondsTotal(TimeSpawnTicks - GameTimeModel.RealTimeNowTick);
            return true;
        }

        public event Action OnIsClearedChanged;
        
        public void SetCleared()
        {
            IsCleared = true;
            Counter++;
            TimeSpawnTicks = GameTimeModel.TicksRealNow + GameTimeModel.GetTicks(_durationRespawn);
            
            OnIsClearedChanged?.Invoke();
            _onZoneCleared?.Invoke();
        }
    }
}