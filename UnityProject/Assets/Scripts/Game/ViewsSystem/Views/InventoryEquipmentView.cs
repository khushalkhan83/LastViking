using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Views
{
    public class InventoryEquipmentView : MonoBehaviour
    {
        [SerializeField] private CellView[] _equipmentCells = default;
        [SerializeField] private GameObject[] _equipmentCellDarkIcons = default;

        public CellView[] EquipmentCells => _equipmentCells;
        
        private void OnEnable() 
        {
            foreach(var cell in _equipmentCells)
            {
                cell.OnDataChanged += OnCellDataChanged;
            }
        }

        private void OnDisable() 
        {
            foreach(var cell in _equipmentCells)
            {
                cell.OnDataChanged -= OnCellDataChanged;
            }
        }

        private void OnCellDataChanged(CellView cell, CellData data)
        {
            int cellID = cell.Id;
            if(cellID >= 0 && cellID < _equipmentCellDarkIcons.Length)
            {
                bool showDarkIcon = data.Icon == null;
               _equipmentCellDarkIcons[cellID].SetActive(showDarkIcon);
            }
        }
    }
}
