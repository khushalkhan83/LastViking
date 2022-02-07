using System;
using Core.Storage;
using EnemiesAttack;
using UnityEngine;
using SOArchitecture;

namespace Game.Models
{
    public class ShelterAttackModeModel : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [Serializable]
        public class Data: DataBase
        {
            [SerializeField] private bool _attackModeActive;
            [SerializeField] private int _waveIndex;


            public bool AttackModeActive
            {
                get => _attackModeActive;
                set { _attackModeActive = value; ChangeData();}
            }
            public int WaveIndex
            {
                get => _waveIndex;
                set { _waveIndex = value; ChangeData();}
            }
        }


        [SerializeField] private Data _data;
        [SerializeField] private EnemiesAttackConfig _config;
        [SerializeField] private GameEvent _gameEventFinishedAttackMode;
        [SerializeField] private Sprite _attackFailedIcon;

        
        #pragma warning restore 0649
        #endregion
        public bool AttackModeAvaliable {get; private set;}

        private QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;

        public DataBase _Data => _data;
        public bool AttackModeActive
        {
            get { return _data.AttackModeActive;}
            private set { _data.AttackModeActive = value; OnAttackModeActiveChanged?.Invoke();}
        }
        public int WaveIndex
        {
            get{ return _data.WaveIndex; }
            set{ _data.WaveIndex = value; }
        }

        public EnemiesAttackConfig EnemiesAttackConfig => QuestsModel.EnemiesAttackConfig;
        public Sprite AttackFailedIcon => _attackFailedIcon;

        public event Action OnAttackModeStart;
        public event Action OnAttackModeFinish;
        public event Action OnAttackModeCancel;
        public event Action OnAttackModeFailed;
        public event Action OnAttackModeActiveChanged;
        public event Action<bool> OnAttackModeAvaliableChanged;
        public event Action OnWaveStart;
        public event Action OnWaveComplete;

        public void StartAttackMode()
        {
            AttackModeActive = true;
            OnAttackModeStart?.Invoke();
        }

        public void FinishAttackMode()
        {
            AttackModeActive = false;
            OnAttackModeFinish?.Invoke();
            _gameEventFinishedAttackMode?.Raise();
        }

        public void CancelAttackMode()
        {
            AttackModeActive = false;
            OnAttackModeCancel?.Invoke();
        }

        public void AttackModeFailed()
        {
            AttackModeActive = false;
            OnAttackModeFailed?.Invoke();
        }

        public void SetAttackModeAvaliable(bool avaliable)
        {
            AttackModeAvaliable = avaliable;
            OnAttackModeAvaliableChanged?.Invoke(avaliable);
        }

        public void WaveStart()
        {
            OnWaveStart?.Invoke();
        }

        public void SetWaveCompleated()
        {
            WaveIndex++;
            OnWaveComplete?.Invoke();
        }

        public int EnemiesLeft {get; set;}
        public int EnemiesTotal {get; set;}
    }
}
