using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;
using System;

namespace Game.Models
{
    public class DeadManLootChestModel : MonoBehaviour
    {
        [System.Serializable]
        public class Data : DataBase
        {
            public ObscuredBool Unlock;
            public ObscuredBool NeedResetItems;
            public void SetUnlock(bool value) {
                Unlock = value;
                ChangeData();
            }

            public void SetNeedResetItems(bool value) {
                NeedResetItems = value;
                ChangeData();
            }
        }

        public event Action ChestOpen;

        [SerializeField] Data _data;
        [SerializeField] private StorageModel _storageModel;

        public Data _Data => _data;
        private StorageModel StorageModel => _storageModel;

        public event Action OnUnlockChanged;

        public bool Unlock {
            get { return _data.Unlock; }
            set { _data.SetUnlock(value); OnUnlockChanged?.Invoke();}
        }

        public bool NeedResetItems
        {
            get { return _data.NeedResetItems; }
            set { _data.SetNeedResetItems(value); }
        }

        public void OnChestOpen()
        {
            ChestOpen?.Invoke();
        }

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
        }

    }
}
