using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class BluePrintsModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase, IImmortal
        {
            public ObscuredInt BluePrints;

            public override SaveTime TimeSave => SaveTime.Instantly;

            public void SetBluePrints(int bluePrints)
            {
                if (BluePrints != bluePrints)
                {
                    BluePrints = bluePrints;
                    ChangeData();
                }
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private StorageModel _storageModel;

#pragma warning restore 0649
        #endregion

        public StorageModel StorageModel => _storageModel;

        public int BluePrints
        {
            get => _data.BluePrints;
            protected set => _data.SetBluePrints(value);
        }

        public event Action OnChange;
        public event Action<int> OnAdd;

        private void Change() => OnChange?.Invoke();
        private void Add(int count) => OnAdd?.Invoke(count);

        public void Adjust(int adjustment)
        {
            BluePrints += adjustment;

            Change();
            if (adjustment > 0)
                Add(adjustment);
        }

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
        }
    }
}
