using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using UnityEngine;

namespace Game.Models
{
    public class FixMovePlayerFromDungeonModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            [SerializeField] private ObscuredBool _fixApplied;

            public bool FixApplied
            {
                get { return _fixApplied; }
                set { _fixApplied = value; ChangeData(); }
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;

#pragma warning restore 0649
        #endregion

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        public bool FixApplied
        {
            get => _data.FixApplied;
            private set => _data.FixApplied = value;
        }
        public void SetFixApplied() => FixApplied = true;

        private bool _dataInited = false;

        // called at PreInitState
        internal void Init()
        {
            if(_dataInited)
            {
                return;
            }
            _dataInited = true;

            StorageModel.TryProcessing(_data);
        }

        private void OnEnable()
        {
            Init();
        }
    }
}
