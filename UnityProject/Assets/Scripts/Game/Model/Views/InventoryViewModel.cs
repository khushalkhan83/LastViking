using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class InventoryViewModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase, IImmortal
        {
            public ObscuredInt ExpandCountLevel;
            public ObscuredInt ForeverExpandedCountLevel;

            public void SetExpandCountLevel(int level)
            {
                ExpandCountLevel = level;
                ChangeData();
            }

            public void SetForeverExpandedCountLevel(int level)
            {
                ForeverExpandedCountLevel = level;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private StorageModel _storageModel;

        [SerializeField] private ObscuredInt _cellsExpanded;
        [SerializeField] private ObscuredInt _maxExpandLevels;

#pragma warning restore 0649
        #endregion

        public int CellsExpanded => _cellsExpanded;
        public int MaxExpandLevels => _maxExpandLevels;

        public int TotalExpandedCountLevel => ExpandCountLevel + ForeverExpandedCountLevel;
        public bool IsMaxExpandLevel => TotalExpandedCountLevel == MaxExpandLevels;
        
        public int ExpandCountLevel
        {
            get => _data.ExpandCountLevel;
            set => _data.SetExpandCountLevel(value);
        }

        public int ForeverExpandedCountLevel
        {
            get => _data.ForeverExpandedCountLevel;
            set => _data.SetForeverExpandedCountLevel(value);
        }

        public event Action<int> OnExpand;

        public Data _Data => _data;
        private StorageModel StorageModel => _storageModel;
        private InventoryModel InventoryModel => ModelsSystem.Instance._inventoryModel;

        public void Reset()
        {
            ExpandCountLevel = 0;

            OnExpand?.Invoke(ExpandCountLevel);
        }

        public void ExpandCellsOnce()
        {
            if (ExpandCountLevel < MaxExpandLevels)
            {
                ++ExpandCountLevel;
                OnExpand?.Invoke(TotalExpandedCountLevel);
            }
        }

        public void ExpandCellsForever()
        {
            if (!IsMaxExpandLevel)
            {
                ++ForeverExpandedCountLevel;
                OnExpand?.Invoke(TotalExpandedCountLevel);
            }
        }
    }
}
