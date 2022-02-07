using Core.Controllers;
using Game.Models;
using Game.Views;
using Core;
using UnityEngine.EventSystems;
using UnityEngine;
using UltimateSurvival;
using Game.Audio;
using System.Collections.Generic;
using System.Linq;

namespace Game.Controllers
{
    public class BuildingHotBarViewController : ViewControllerBase<BuildingHotBarView>
    {
        [Inject] public BuildingHotBarModel BuildingHotBarModel { get; set; }
        [Inject] public ItemsDB ItemsDB { get; set; }
        [Inject] public AudioSystem AudioSystem { get; set; }
        
        private BuildingCellView clickCell = null;

        protected override void Show()
        {
            SetupCells();

            foreach (var cell in View.Cells)
            {
                cell.PointerDown += OnCellPointerDown;
                cell.PointerUp += OnCellPointerUp;
                cell.Hold += OnCellHold;
                foreach (var optionCell in cell.OptionCells) 
                {
                    optionCell.OnClick += OnOptionCellClick;
                }
            }

            OnDisabledCategoryChanged(BuildingHotBarModel.DisabledCategories);
            BuildingHotBarModel.OnDisabledCategoryChanged += OnDisabledCategoryChanged;
        }

        protected override void Hide()
        {
            foreach (var cell in View.Cells)
            {
                cell.PointerDown -= OnCellPointerDown;
                cell.PointerUp -= OnCellPointerUp;
                cell.Hold -= OnCellHold;
                foreach (var optionCell in cell.OptionCells)
                {
                    optionCell.OnClick -= OnOptionCellClick;
                }
            }

            BuildingHotBarModel.ResetLastSelected();

            BuildingHotBarModel.OnDisabledCategoryChanged -= OnDisabledCategoryChanged;
        }

        private void SetupCells()
        {
            int i = 0;
            for (; i < BuildingHotBarModel.Categories.Count; i++)
            {
                var cell = View.Cells[i];
                var category = BuildingHotBarModel[i];

                SetupCell(cell, i);

                if (cell.OptionCells == null || cell.OptionCells.Count == 0)
                {
                    SetupOptionCells(cell);
                }
            }
            for (; i < View.Cells.Count; i++)
            {
                View.Cells[i].SetIsVisible(false);
            }
        }

        private void SetupCell(BuildingCellView cell, int cellID)
        {
            int selectedOption = BuildingHotBarModel.GetSelectedOption(cellID);
            SavableItem item = BuildingHotBarModel[cellID][selectedOption];
            Sprite icon = item.ItemData.Icon;
            bool isSelected = cellID == BuildingHotBarModel.SelectedCell;

            var cellData = new BuildingCellData()
            {
                CellID = cellID,
                Icon = icon,
                IsSelected = isSelected,
                IsActive = true,
            };

            cell.SetData(cellData);
        }

        private void SetupOptionCells(BuildingCellView cell)
        {
            var category = BuildingHotBarModel[cell.CellID];
            for (int i = 0; i < category.Items.Count; i++)
            {
                BuildingCellOptionView optionCell = Instantiate(View.BuildingOptionCellPref, cell.CellsHolder).GetComponent<BuildingCellOptionView>();
                var optionCellData = new BuildingCellData()
                {
                    CellID = i,
                    CategoryCellID = cell.CellID,
                    Icon = category.Items[i].ItemData.Icon,
                    IsSelected = false,
                    IsActive = false,
                };

                optionCell.SetData(optionCellData);
                cell.OptionCells.Add(optionCell);
            }
        }

        private void OnCellPointerDown(BuildingCellView cell, PointerEventData eventData)
        {
            clickCell = cell;
        }

        private void OnCellPointerUp(BuildingCellView cell, PointerEventData eventData)
        {
            if(clickCell == cell)
            {
                SetSelectedCell(cell);
                clickCell = null;
            }
        }

        private void OnOptionCellClick(BuildingCellOptionView optionCell)
        {
            BuildingHotBarModel.SetSelectedOptionCell(optionCell.CategoryCellID, optionCell.CellID);

            var cell = View.Cells[optionCell.CategoryCellID];
            foreach (var c in cell.OptionCells)
            {
                c.SetIsVisible(false);
            }

            cell.SetItemIcon(optionCell.ItemSprite);

            AudioSystem.PlayOnce(AudioID.Button);
        }

        private void OnCellHold(BuildingCellView cell)
        {
            SetSelectedCell(cell);
            clickCell = null;
            int selectedOptionCell = BuildingHotBarModel.GetSelectedOption(cell.CellID);
            for (int i = 0; i < cell.OptionCells.Count; i++)
            {
                var optionCell = cell.OptionCells[i];
                bool isSelected = optionCell.CellID == selectedOptionCell;
                optionCell.SetIsVisible(true);
                optionCell.SetIsVisibleSelectionBorder(isSelected);
            }

            AudioSystem.PlayOnce(AudioID.WindowOpen);
        }

        private void SetSelectedCell(BuildingCellView cell)
        {
            foreach (var buildingCell in View.Cells)
            {
                foreach (var optionCell in buildingCell.OptionCells)
                {
                    optionCell.SetIsVisible(false);
                }
            }

            BuildingHotBarModel.SelectedCell = cell.CellID;
            foreach (var buildingCell in View.Cells)
            {
                buildingCell.SetIsVisibleSelectionBorder(buildingCell == cell);
            }
        }

        private void OnDisabledCategoryChanged(List<ConstructionCategoryID> categories)
        {
            foreach (var cell in View.Cells)
            {
                cell.SetInteractable(true);
            }

            if(categories != null && categories.Count > 0)
            {
                foreach(var category in categories)
                {
                    int cellIndex = BuildingHotBarModel.Categories.IndexOf(BuildingHotBarModel.Categories.FirstOrDefault(c => c.CategoryID == category));
                    if(cellIndex < View.Cells.Count)
                        View.Cells[cellIndex].SetInteractable(false);
                }
            }
        }
    }
}
