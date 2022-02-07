using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    public class ExpandsModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public List<ExpandModelInfo> Expands;

            public void Initialize()
            {
                foreach (var e in Expands)
                {
                    e.ExpandModel.OnExpand += OnExpand;
                }
            }

            private void OnExpand(int obj) => ChangeData();
        }

        [Serializable]
        public struct ExpandModelInfo
        {
            [ObscuredID(typeof(ExpandGroup))] public ObscuredInt ExpandGroupID;
            public ExpandModel ExpandModel;
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private StorageModel _storageModel;

#pragma warning restore 0649
        #endregion

        public List<ExpandModelInfo> Expands => _data.Expands;

        public ExpandModel Get(ExpandGroup key)
        {
            if (key == ExpandGroup.None)
            {
                return new ExpandModel();
            }

            return Expands[(int)key - 1].ExpandModel;
        }

        public StorageModel StorageModel => _storageModel;

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
            _data.Initialize();
        }
    }
}
