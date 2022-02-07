using Core.Storage;
using Game.Models;
using System;
using UnityEngine;

namespace UltimateSurvival
{
    public class ItemPickUpTimeDelayModel : InteractableObject
    {
        [Serializable]
        public class Data : DataBase
        {
            public float RemainingTime;

            public void SetRemainingTime(float remainingTime)
            {
                RemainingTime = remainingTime;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private string _itemName;
        [SerializeField] private int _itemCount;
        [SerializeField] private float _spawnTime;

#pragma warning restore 0649
        #endregion

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        public string ItemName => _itemName;
        public int ItemCount => _itemCount;
        public float SpawnTime => _spawnTime;

        public bool IsHasItem => RemainingTime == 0;

        public event Action OnBeginSpawn;

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
        }

        public float RemainingTime
        {
            get
            {
                return _data.RemainingTime;
            }
            private set
            {
                _data.SetRemainingTime(value);
            }
        }

        public void UpdateTimeRemaining(float deltaTime)
        {
            RemainingTime -= deltaTime;

            if (RemainingTime < 0)
            {
                RemainingTime = 0;
            }
        }

        public void StartSpawn()
        {

            RemainingTime = SpawnTime;

            OnBeginSpawn?.Invoke();
        }
    }
}
