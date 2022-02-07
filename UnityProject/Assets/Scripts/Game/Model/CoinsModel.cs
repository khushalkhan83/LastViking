using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class CoinsModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase, IImmortal
        {
            public ObscuredInt Coins;
            public ObscuredBool IsFirstStart = true;

            public override SaveTime TimeSave => SaveTime.Instantly;

            public void SetCoins(int coins)
            {
                Coins = coins;
                ChangeData();
            }

            public void SetIsFirstStart(bool isFirstStart)
            {
                IsFirstStart = isFirstStart;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private StorageModel _storageModel;

#pragma warning restore 0649
        #endregion

        public StorageModel StorageModel => _storageModel;

        public int Coins
        {
            get => _data.Coins;
            protected set => _data.SetCoins(value);
        }

        public bool IsFirstStart
        {
            get => _data.IsFirstStart;
            protected set => _data.SetIsFirstStart(value);
        }

        public event Action OnChange;

        private void Change() => OnChange?.Invoke();

        public void Adjust(int adjustment)
        {
            Coins += adjustment;

            Change();
        }

        private void OnEnable()
        {
            if (StorageModel.TryProcessing(_data))
            {
                IsFirstStart = false;
            }
        }
    }
}
