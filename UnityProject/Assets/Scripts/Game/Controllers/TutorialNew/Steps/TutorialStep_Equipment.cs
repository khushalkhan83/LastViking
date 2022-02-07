using Core;
using Game.Models;
using Game.Views;
using UnityEngine;
using System.Linq;
using Extensions;
using Game.Components;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_Equipment : TutorialStepBase
    {
        [Inject] public TutorialSimpleDarkViewModel TutorialSimpleDarkViewModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public InventoryEquipmentModel InventoryEquipmentModel { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public InventoryButtonViewModel InventoryButtonViewModel { get; private set; }
        [Inject] public InventoryPlayerViewModel InventoryPlayerViewModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public DragItemViewModel DragItemViewModel { get; private set; }

        private const string k_equipItemName = "Shoes2";
        private const int k_requiaredItemCount = 1;
        private int slotCellId;
        private CellModel slotCell;
        private GameObject inventoryHighlightCell;
        private GameObject slotHighlightCell;

        public TutorialStep_Equipment(TutorialEvent StepStartedEvent) : base(StepStartedEvent) { }

        public override void OnStart()
        {
            slotCellId = InventoryEquipmentModel.GetSlotIdByCategory(EquipmentCategory.Shoes);
            slotCell = InventoryEquipmentModel.ItemsContainer.GetCell(slotCellId);
            ViewsSystem.OnEndShow.AddListener(ViewConfigID.InventoryPlayer, OnEndShowInentoryView);
            slotCell.OnChange += OnSlotCellChanged;
            var icon = ItemsDB.ItemDatabase.GetItemByName(k_equipItemName).Icon;
            ShowTaskMessage(true, "Open inventory and equip shoes", icon);
            ProcessState();
        }
           
        public override void OnEnd()
        {
            ViewsSystem.OnEndShow.RemoveListener(ViewConfigID.InventoryPlayer, OnEndShowInentoryView);
            slotCell.OnChange -= OnSlotCellChanged;
            FinishStep();
        }

        private void ProcessState()
        {
            ShowDarkScreen(false);

            bool inventoryWindowIsOpened = ViewsSystem.IsShow(ViewConfigID.InventoryPlayer);
            if(!inventoryWindowIsOpened)
            {
                OpenInventoryWindowState();
                return;
            }

            bool shoesEquiped = slotCell.IsHasItem;
            if(!shoesEquiped)
            {
                EquipShoesState();
                return;
            }

            FinishStep();

            CheckConditions();
        }

        private void OpenInventoryWindowState()
        {
            InventoryButtonViewModel.SetPulseAnimation(true);
        }

        private void EquipShoesState()
        {
            EnsureEnoughItem(k_equipItemName,k_requiaredItemCount);
            InventoryButtonViewModel.SetPulseAnimation(false);
            ShowDarkScreen(true);

            GetTargetCell(out ContainerID containerID, out int cellID);
            inventoryHighlightCell = InventoryPlayerViewModel.GetCell(containerID, cellID);
            inventoryHighlightCell.SafeActivateComponent<TutorialHilightAndAnimation>();
            InventoryPlayerViewModel.SelectCell(containerID, cellID);
            InventoryPlayerViewModel.HighlightCell(containerID, cellID);

            slotHighlightCell = InventoryPlayerViewModel.GetCell(ContainerID.Equipment, slotCellId);
            slotHighlightCell.SafeActivateComponent<TutorialHilightAndAnimation>();

            DragItemViewModel.SetDragData(inventoryHighlightCell.transform, slotHighlightCell.transform);
            DragItemViewModel.SetShow(true);
        }

        private void FinishStep()
        {
            ShowDarkScreen(false);
            InventoryButtonViewModel.SetPulseAnimation(false);
            if(inventoryHighlightCell != null) inventoryHighlightCell.SafeDeactivateComponent<TutorialHilightAndAnimation>();
            if(slotHighlightCell != null) slotHighlightCell.SafeDeactivateComponent<TutorialHilightAndAnimation>();
            DragItemViewModel.SetShow(false);
        }

        protected bool GetTargetCell(out ContainerID containerID, out int cellID)
        {
            var shoesCell = InventoryModel.ItemsContainer.Cells.FirstOrDefault(c => c.IsHasItem && c.Item.Name == k_equipItemName);
            if(shoesCell != null)
            {
                containerID = ContainerID.Inventory;
                cellID = shoesCell.Id;
                return true;
            }

            shoesCell = HotBarModel.ItemsContainer.Cells.FirstOrDefault(c => c.IsHasItem && c.Item.Name == k_equipItemName);
            if(shoesCell != null)
            {
                containerID = ContainerID.HotBar;
                cellID = shoesCell.Id;
                return true;
            }

            containerID = ContainerID.None;
            cellID = 0;
            return false;
        }

        private void OnEndShowInentoryView()
        {
            ProcessState();
        }

        private void OnSlotCellChanged(CellModel cell)
        {
            ProcessState();
        }

        private void ShowDarkScreen(bool show) => TutorialSimpleDarkViewModel.SetShow(show);

        private void CheckConditions()
        {
            bool nextStep = slotCell.IsHasItem;

            if(nextStep) TutorialNextStep();
        }
    }
}
