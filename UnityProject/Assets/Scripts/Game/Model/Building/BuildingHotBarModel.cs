using Core.Storage;
using System;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class BuildingHotBarModel : MonoBehaviour
    {
        public string BuildingCategory { get; } = "Construction";
        public string ItemBuildingCategory { get; } = "BuildingCategory";
        public string ItemBuildingIndex { get; } = "BuildingIdx";

        [Serializable]
        public class Data : DataBase
        {
            public int selectedCell;
            public List<int> selectedOptionCells = new List<int>();

            public void SetSelectedCell(int cell)
            {
                selectedCell = cell;
                ChangeData();
            }

            public void SetSelectedOptionCell(int cell, int optionCell)
            {

                selectedOptionCells[cell] = optionCell;
                ChangeData();
            }
        }

        [SerializeField] Data _data;

        public Data _Data => _data;

        public List<BuildingCategory> Categories { get; } = new List<BuildingCategory>();
        public List<ConstructionCategoryID> DisabledCategories { get; private set;}

        public event Action PartSelected;
        public event Action PartSelectedChanged;
        public event Action<List<ConstructionCategoryID>> OnDisabledCategoryChanged;

        // Actually used only for situation where user switches between modes, and select current cell again first time
        public int LastSelectedCell { get; private set; } = -1;
        public int LastSelectedOption { get; private set; } = -1;

        public int SelectedCell {
            get { return _data.selectedCell; }
            set
            {
                if (value != LastSelectedCell)
                {
                    _data.SetSelectedCell(value);
                    PartSelectedChanged?.Invoke();
                    LastSelectedOption = -1;
                }
                LastSelectedCell = value;
                PartSelected?.Invoke();
            }
        }

        public int GetSelectedOption(int cell) {
            if (cell < _data.selectedOptionCells.Count)
            {
                return _data.selectedOptionCells[cell];
            }
            else
            {
                return 0;
            }
        }

        public void SetSelectedOptionCell(int cell, int optionCell) {
            if (cell >= _data.selectedOptionCells.Count)
            {
                while (cell >= _data.selectedOptionCells.Count)
                {
                    _data.selectedOptionCells.Add(0);
                }
            }

            if ((cell != SelectedCell) || (cell == SelectedCell && optionCell != LastSelectedOption))
            {
                _data.SetSelectedOptionCell(cell, optionCell);
                LastSelectedOption = GetSelectedOption(cell);
                PartSelectedChanged?.Invoke();
            }
            PartSelected?.Invoke();
        }

        public void ResetLastSelected()
        {
            LastSelectedCell = -1;
            LastSelectedOption = -1;
        }

        public SavableItem SelectedItem
        {
            get
            {
                int option = GetSelectedOption(SelectedCell);
                return this[SelectedCell][option];
            }
        }
        public void SetDisabledCategories(List<ConstructionCategoryID> categories)
        {   
            DisabledCategories = categories;
            OnDisabledCategoryChanged?.Invoke(categories);
        }

        public void EnableAllCategories()
        {
            if(DisabledCategories != null)
                DisabledCategories.Clear();
            OnDisabledCategoryChanged?.Invoke(DisabledCategories);
        }

        public BuildingCategory this[int index] => Categories[index];
    }

    public class BuildingCategory
    {
        public ConstructionCategoryID CategoryID;
        public List<SavableItem> Items;

        public SavableItem this[int index] => Items[index];
    }
}
