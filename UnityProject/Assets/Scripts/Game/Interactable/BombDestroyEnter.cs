using Core.Storage;
using Game.Models;
using SOArchitecture;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Interactables
{
    public class BombDestroyEnter : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public bool IsOpen;
            public long DetonateTimeTicks;
            public bool BombPlanted;

            public void SetIsOpen(bool isOpen) {
                IsOpen = isOpen;
                ChangeData();
            }

            public void SetDetonateTimeTicks(long ticks)
            {
                DetonateTimeTicks = ticks;
                ChangeData();
            }

            public void SetBombPlanted(bool planted)
            {
                BombPlanted = planted;
                ChangeData();
            }
        }

        public event Action OnBombDetonate;

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private GameObject _doorObject;
        [SerializeField] private GameObject _ghostBombObject;
        [SerializeField] private GameObject _bombPlantedObject;
        [SerializeField] private float _detonateTimeout;
        [SerializeField] private GameEvent _gameEventBombDetonate;
        [SerializeField] private UnityEvent _unityEventBombDetonate;

#pragma warning restore 0649
        #endregion

        public Data Data_ => _data;
        public GameObject DoorObject => _doorObject;
        public GameObject GhostBombObject => _ghostBombObject;
        public GameObject BombPlantedObject => _bombPlantedObject;
        public float DetonateTimeout => _detonateTimeout;

        public StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        public GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;
        public GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;

        public bool IsOpen {
            get => Data_.IsOpen;
            private set => Data_.SetIsOpen(value);
        }

        public long DetonateTimeTicks
        {
            get => Data_.DetonateTimeTicks;
            set => Data_.SetDetonateTimeTicks(value);
        }

        public bool BombPlanted
        {
            get => Data_.BombPlanted;
            set => Data_.SetBombPlanted(value);
        }

        private void OnEnable()
        {
            StorageModel.TryProcessing(Data_);
            GameUpdateModel.OnUpdate += OnUpdate;
            SetActiveDoor(!IsOpen);
            SetActiveGhostBomb(false);
            SetActiveBombPlanted(BombPlanted && GetRemainingBombTime() > 0);
        }

        private void OnDisable()
        {
            StorageModel.Untracking(Data_);
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void OnUpdate()
        {
            if (BombPlanted && GameTimeModel.RealTimeNowTick > DetonateTimeTicks)
            {
                DetonateBomb();
            }
        }

        private void SetActiveDoor(bool on) => _doorObject.SetActive(on);
        public void SetActiveGhostBomb(bool on) => _ghostBombObject.SetActive(on);
        private void SetActiveBombPlanted(bool on) => _bombPlantedObject.SetActive(on);

        public float GetRemainingBombTime()
        {
            long remainingTicks = DetonateTimeTicks - GameTimeModel.RealTimeNowTick;
            return GameTimeModel.GetSecondsTotal(remainingTicks);
        }

        public void ShowGhostBomb()
        {
            if (!BombPlanted)
            {
                SetActiveGhostBomb(true);
            }
        }

        public void HideGhostBomb()
        {
            SetActiveGhostBomb(false);
        }

        public void PlantBomb()
        {
            SetActiveGhostBomb(false);
            SetActiveBombPlanted(true);
            BombPlanted = true;
            DetonateTimeTicks = GameTimeModel.RealTimeNowTick + GameTimeModel.GetTicks(DetonateTimeout);
        }

        private void DetonateBomb()
        {
            BombPlanted = false;
            SetActiveDoor(false);
            IsOpen = true;
            OnBombDetonate?.Invoke();
            _gameEventBombDetonate?.Raise();
            _unityEventBombDetonate?.Invoke();
        }
    }
}
