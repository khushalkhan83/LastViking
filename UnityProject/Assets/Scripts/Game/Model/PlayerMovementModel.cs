using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class PlayerMovementModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public PlayerMovementID PlayerMovementID;
            public float WaterOffset;

            public void SetPlayerMoventID(PlayerMovementID playerMovementID)
            {
                PlayerMovementID = playerMovementID;
                ChangeData();
            }

            public void SetWaterOffset(float offset)
            {
                WaterOffset = offset;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private StorageModel _storageModel;
        [SerializeField] float _waterLevel;

#pragma warning restore 0649
        #endregion

        public float InitWaterLevel => _waterLevel;
        public float WaterLevel => InitWaterLevel + WaterOffset;
        public StorageModel StorageModel => _storageModel;

        public bool BlockPlayerMovement {get;private set;} = false;
        public bool IgnoreSlopes {get;private set;} = false;

        public float WaterOffset
        {
            get => _data.WaterOffset;
            set => _data.SetWaterOffset(value);
        }

        public PlayerMovementID MovementID
        {
            get => _data.PlayerMovementID;
            private set => _data.SetPlayerMoventID(value);
        }

        public event Action OnChangeMovementID;
        public event Action OnInitializeData;
        public event Action<bool> OnBlockPlayerMovement;

        public void ChangeWaterOffset(float offset)
        {
            WaterOffset = offset;
        }

        public void SetMevementID(PlayerMovementID playerMovementID)
        {
            MovementID = playerMovementID;
            OnChangeMovementID?.Invoke();
        }

        public void SetBlockPlayerMovement(bool block)
        {
            BlockPlayerMovement = block;
            PlayerInput.Instance.playerControllerInputBlocked = block;
            OnBlockPlayerMovement?.Invoke(block);
        }

        public void SetIgnoreSlopes(bool ignore)
        {
            IgnoreSlopes = ignore;
        }

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
            OnInitializeData?.Invoke();
            OnChangeMovementID?.Invoke();
        }
    }
}
