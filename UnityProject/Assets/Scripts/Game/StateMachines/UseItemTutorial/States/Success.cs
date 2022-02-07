using Game.Models;

namespace Game.StateMachines.UseItemTutorial
{
    // success is a dead end with no transitions to other states
    public class Success : IUseItemTutorialState
    {
        private readonly UseItemTutorialStateMachine StateMachine;
        private string TargetItemName;
        public Success(UseItemTutorialStateMachine stateMachine) => StateMachine = stateMachine;

        #region IUseItemTutorialState
        public void SetTargetItem(string targetItemName) => TargetItemName = targetItemName;
        public void Enter() { }
        public void Exit() { }
        public void OnApplyItem(ItemsContainer container, CellModel cell) { }
        public void OnChangeSelectedCell(InventoryCellsViewModelBase.CellInfo cellInfo) { }
        public void OnInventoryClosed() { }
        public void OnInventoryOpened() { }
        public void OnPlayerDeath() { }
        public void OnPlayerRevival() { }

        #endregion
    }
}