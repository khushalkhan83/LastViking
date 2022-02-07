using System.Linq;
using Game.Models;
using static Game.Models.InventoryCellsViewModelBase;

namespace Game.StateMachines.UseItemTutorial
{
    public class HilightItemsInInventory : HilightItemsInInventoryBase
    {
        public HilightItemsInInventory(UseItemTutorialStateMachine stateMachine) : base(stateMachine) { }

        #region IUseItemTutorialState
        public override void OnChangeSelectedCell(CellInfo cellInfo)
        {
            if(cellInfo.Reseted) return;

            if(SelectedTargetItem(cellInfo))
            {
                StateMachine.SetState(new HilightUseItemButton(StateMachine));
            }
        }
            
        #endregion
        private bool SelectedTargetItem(CellInfo cellInfo)
        {
            CellModel selecedCellModel = GetCellModel(cellInfo);

            if (selecedCellModel.IsEmpty) return false;

            return selecedCellModel.Item.ItemData.Name == TargetItemName;
        }
        private CellModel GetCellModel(CellInfo cellInfo)
        {
            var targetContiner = GetTargetContainer(cellInfo.ContainerID);
            return targetContiner?.GetCell(cellInfo.CellId);
        }

        private ItemsContainer GetTargetContainer(ContainerID containerID)
        {
            if(containerID == ContainerID.Inventory)
                return InventoryModel.ItemsContainer;
            else if(containerID == ContainerID.HotBar)
                return HotBarModel.ItemsContainer;

            else return null;
        }
    }
}