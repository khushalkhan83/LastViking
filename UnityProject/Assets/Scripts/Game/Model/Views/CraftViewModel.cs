using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Models
{
    public class CraftViewModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase, IImmortal
        {
            public ObscuredInt shelterLevelMax;
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private StorageModel _storageModel;

#pragma warning restore 0649
        #endregion

        public struct CellInfo
        {
            public ContainerID containerID;
            public int cellId;
            public int itemId;
        }

        public enum CategoryID
        {
            None = 0,
            Tools = 1,
            Items = 2,
            Weapons = 3,
            Defence = 4,
            Medical = 5,
        }

        public enum ContainerID
        {
            None = 0,
            Craft = 1,
            Queue = 2,
            Resource = 3,
        }

        public StorageModel StorageModel => _storageModel;

        public int ShelterLevelMax
        {
            get => _data.shelterLevelMax;
            protected set => _data.shelterLevelMax = value;
        }

        public CategoryID CategorySelected { get; private set; }
        public CategoryID CategorySelectedLast { get; private set; }

        public CellInfo SelectedCellCraftLast { get; private set; }
        public CellInfo SelectedCell { get; private set; }
        public CellInfo SelectedCellLast { get; private set; }
        public CellInfo[] SelectionsCraft { get; } = new CellInfo[6];

        public event Action OnChangeCatgory;
        public event Action<CellInfo> OnChangeSelected;

        public void SetShelterMax(int level)
        {
            _data.shelterLevelMax = level;
        }

        private CellInfo GetCellIdCraftSelection()
        {
            return SelectionsCraft[(int)CategorySelected];
        }

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
        }

        public void SelectCategory(CategoryID catogoryID)
        {
            CategorySelectedLast = CategorySelected;
            CategorySelected = catogoryID;
            var index = CategoryIdByIndex.FirstOrDefault(x => x.Value == catogoryID).Key;
            SelectedCategoryIndex = index;

            SelectionCell(GetCellIdCraftSelection());

            OnChangeCatgory?.Invoke();
        }

        public void SelectionCell(CellInfo cellInfo) => SetSelectedCell(cellInfo.containerID, cellInfo.cellId, cellInfo.itemId);

        public void SelectCell(ContainerID containerID, int cellId, int itemId) => SetSelectedCell(containerID, cellId, itemId);

        public void RemoveSelectCell() => SetSelectedCell(ContainerID.None, -1, -1);

        private void SetSelectedCell(ContainerID containerID, int cellId, int itemId)
        {
            if (SelectedCell.containerID == ContainerID.Craft)
            {
                SelectedCellCraftLast = SelectedCell;
            }

            SelectedCellLast = SelectedCell;
            SelectedCell = new CellInfo()
            {
                containerID = containerID,
                cellId = cellId,
                itemId = itemId
            };

            if (containerID == ContainerID.Craft)
            {
                SelectionsCraft[(int)CategorySelected] = SelectedCell;
            }

            ChangeSelectCell();
        }

        private void ChangeSelectCell()
        {
            OnChangeSelected?.Invoke(SelectedCell);
        }

        public event Func<ContainerID,int,GameObject> OnGetCell;
        public GameObject GetCell(ContainerID containerID, int cellId)
        {
           var v = SelectionsCraft[(int)ContainerID.Craft];
           return OnGetCell.Invoke(containerID,cellId);
        }
        
        public event Func<GameObject> OnGetCraftButton;
        public event Func<GameObject> OnGetBoostButton;
        public GameObject GetCraftButton() => OnGetCraftButton.Invoke();
        public GameObject GetBoostButton() => OnGetBoostButton.Invoke();

        public int SelectedCategoryIndex {get; private set;}

        public int MaxCategoryIndex => CategoryIdByIndex.Keys.Count -1;

        public Dictionary<int,CategoryID> CategoryIdByIndex {get;} = new Dictionary<int,CategoryID>()
        {
            {0,CategoryID.None},
            {1,CategoryID.Tools},
            {2,CategoryID.Items},
            {3,CategoryID.Weapons},
            {4,CategoryID.Defence},
            {5,CategoryID.Medical},
        };
    }
}
