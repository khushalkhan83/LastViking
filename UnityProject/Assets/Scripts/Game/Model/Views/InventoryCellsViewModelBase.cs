using System;
using NaughtyAttributes;
using UnityEngine;

namespace Game.Models
{
    public abstract class InventoryCellsViewModelBase : MonoBehaviour
    {
        public class CellInfo
        {
            protected internal ContainerID containerID;
            protected internal int cellId;

            public ContainerID ContainerID => containerID;
            public int CellId => cellId;

            protected internal void Reset()
            {
                containerID = ContainerID.None;
                cellId = -1;
            }

            public bool Reseted => cellId == -1;
            protected internal void CopyFrom(CellInfo cellInfo)
            {
                containerID = cellInfo.containerID;
                cellId = cellInfo.cellId;
            }
        }


        [SerializeField] bool disableItemsWithoutProperties = false;
        [ShowIf("disableItemsWithoutProperties")]
        [SerializeField] string[] activeItemsProperties = default; 

        public event Action<CellInfo> OnChangeSelected;
        public event Action<CellInfo> OnChangeHighlighted;
        public event Action<bool,CellInfo> OnSetShowOnTopCell;

        public bool DisableItemsWithoutProperties => disableItemsWithoutProperties;
        public string[] ActiveItemsProperties => activeItemsProperties;
        public int DragId { get; private set; }
        public bool IsHasDrag { get; private set; }

        public event Action OnIsHasDragChanged;
        public CellInfo HighlightedLast { get; } = new CellInfo();
        public CellInfo SelectedCell { get; } = new CellInfo();
        public CellInfo HighlightedCell { get; } = new CellInfo();

        // public List<CellInfo> ShowOnTopCells {get; private set;}  = new List<CellInfo>();

        public void SelectCell(ContainerID containerID, int cellId)
        {
            SelectedCell.containerID = containerID;
            SelectedCell.cellId = cellId;

            ChangeSelect();
        }

        public void RemoveSelectCell()
        {
            SelectedCell.Reset();

            ChangeSelect();
        }

        private void ChangeSelect()
        {
            OnChangeSelected?.Invoke(SelectedCell);
        }

        public void HighlightCell(ContainerID containerID, int cellId)
        {
            HighlightedLast.CopyFrom(HighlightedCell);

            HighlightedCell.containerID = containerID;
            HighlightedCell.cellId = cellId;

            ChangeHighlight();
        }

        public event Func<ContainerID,int,GameObject> OnGetCell;

        public GameObject GetCell(ContainerID containerID, int cellId)
        {
            var cell = OnGetCell?.Invoke(containerID,cellId);

            return cell;
        }

        public event Func<GameObject> OnGetApplyButton;
        public GameObject GetApplyButton()
        {
            return OnGetApplyButton?.Invoke();
        }

        public void RemoveHighlightCell()
        {
            HighlightedLast.CopyFrom(HighlightedCell);
            HighlightedCell.Reset();

            ChangeHighlight();
        }

        private void ChangeHighlight()
        {
            OnChangeHighlighted?.Invoke(HighlightedCell);
        }

        public void DragBegin(int id)
        {
            DragId = id;
            IsHasDrag = true;
            OnIsHasDragChanged?.Invoke();
        }

        public void DragEnd()
        {
            DragId = -1;
            IsHasDrag = false;
            OnIsHasDragChanged?.Invoke();
        }
    }
}
