using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using SOArchitecture;
using System;
using UnityEngine;

namespace Game.Models
{
    public class ShelterModel : InitableModel<ShelterModel.Data>
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredInt Level;
            public ObscuredULong StartAliveTimeTicks;

            public override SaveTime TimeSave => SaveTime.Instantly;

            public void SetLevel(int level)
            {
                Level = level;
                ChangeData();
            }

            public void SetStartAliveTimeTicks(ulong startAliveTimeTicks)
            {
                StartAliveTimeTicks = startAliveTimeTicks;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;

        [ObscuredID(typeof(ShelterModelID))]
        [SerializeField] private ObscuredInt _shelterID;

        [SerializeField] private ShelterCosts _costBuy;
        [SerializeField] private ShelterCosts[] _costUpgrades;
        [SerializeField] private int[] _coinsReward;

        [SerializeField] private LocalizationKeyID _nameKeyID;
        [SerializeField] private bool _spawnConins;
        [SerializeField] private GameObject _coinsDropPosition;
        [SerializeField] private GameEvent _gameEventShelterLevelChanged;

#pragma warning restore 0649
        #endregion

        protected override Data DataBase => _data;

        public ShelterModelID ShelterID => (ShelterModelID)(int)_shelterID;
        public LocalizationKeyID NameKeyID => _nameKeyID;
        public bool SpawnConins => _spawnConins;
        public GameObject CoinsDropPosition => _coinsDropPosition;

        public int Level
        {
            get
            {
                return _data.Level;
            }
            protected set
            {
                _data.SetLevel(value);
                _gameEventShelterLevelChanged?.Raise();
            }
        }

        public ulong StartAliveTimeTicks
        {
            get
            {
                return _data.StartAliveTimeTicks;
            }
            set
            {
                _data.SetStartAliveTimeTicks(value);
            }
        }

        public ShelterCosts CostBuy => _costBuy;
        public ShelterCosts[] CostUpgrades => _costUpgrades;
        public int[] CoinsReward => _coinsReward;
        public ShelterCosts CostUpgradeCurrent => CostUpgrades[Level];
        public int CoinsCurrent => CoinsReward[Level];
        public ObscuredBool IsMaxLevel => Level == CostUpgrades.Length;
        public bool IsChangeLevel { get; private set; }

        public event Action OnPreUpgrade;
        public event Action OnUpgrade;
        public event Action OnDeath;
        //public event Action OnDowngrade;
        public event Action OnBuy;
        public event Action OnActivate;

        public void Buy(ulong timeTicks)
        {
            StartAliveTimeTicks = timeTicks;
            OnBuy?.Invoke();
            ++Level;

            if (Level > CraftViewModel.ShelterLevelMax)
            {
                CraftViewModel.SetShelterMax(Level);
            }
            //++Level;

            _gameEventShelterLevelChanged?.Raise();
        }

        public void Activate() => OnActivate?.Invoke();

        private CraftViewModel CraftViewModel => ModelsSystem.Instance._craftViewModel;

        public Vector3 CorePosition { get; private set; }

        public void SetLevelData(Vector3 position)
        {
            CorePosition = position;
        }

        public void Death()
        {
            OnDeath?.Invoke();
        }

        public void Upgrade()
        {
            ++Level;

            OnPreUpgrade?.Invoke();

            if (Level > CraftViewModel.ShelterLevelMax)
            {
                CraftViewModel.SetShelterMax(Level);
            }
        }

        public void SetIsChangeLevel(bool value)
        {
            IsChangeLevel = value;
        }

        public void Upgraded()
        {
            OnUpgrade?.Invoke();
            SetIsChangeLevel(false);
        }

        // TODO: remove (only for debug?)
        internal void Temp_SetLevel(int level)
        {
            Level = level;
            OnUpgrade?.Invoke();
        }
    }
}
