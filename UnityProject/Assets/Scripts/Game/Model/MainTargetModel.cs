using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class MainTargetModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredVector3 Position;

            public void SetPosition(Vector3 position)
            {
                Position = position;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
#pragma warning restore 0649
        #endregion

        public ObscuredVector3 Position
        {
            get
            {
                return _data.Position;
            }
            private set
            {
                _data.SetPosition(value);
            }
        }

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
        }

        public event Action OnChangePosition;
        public void SetPosition(Vector3 position)
        {
            Position = position;
            OnChangePosition?.Invoke();
        }
    }
}
