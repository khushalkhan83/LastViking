using Game.Controllers.Controllers.States;
using Game.Models;

namespace Game.StateMachines.CraftItemTutorial
{
    public interface ICraftItemTutorialState : IState
    {
        void SetTargetItem(string targetItemName);
        void OnShowCraft();
        void OnHideCraft();
        void OnCraftItemStarted(int itemId);
        void OnBoost();
        void OnChangeSelectedHandler(CraftViewModel.CellInfo cellInfo);
        void OnPlayerDeath();
        void OnPlayerRevival();
    }
}
