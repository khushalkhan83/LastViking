using Game.Models;

namespace Game.StateMachines.CraftItemTutorial
{
    // success is a dead end with no transitions to other states
    public class Success : ICraftItemTutorialState
    {
        private readonly CraftItemTutorialStateMachine StateMachine;
        private string TargetItemName;
        public Success(CraftItemTutorialStateMachine stateMachine) => StateMachine = stateMachine;

        #region ICraftItemTutorialState
        public void Enter() { }
        public void Exit() { }
        public void SetTargetItem(string targetItemName) => TargetItemName = targetItemName;
        public void OnShowCraft() { }
        public void OnHideCraft() { }
        public void OnCraftItemStarted(int itemId) { }
        public void OnBoost() { }
        public void OnChangeSelectedHandler(CraftViewModel.CellInfo cellInfo) { }
        public void OnPlayerDeath() { }
        public void OnPlayerRevival() { }

        #endregion
    }
}