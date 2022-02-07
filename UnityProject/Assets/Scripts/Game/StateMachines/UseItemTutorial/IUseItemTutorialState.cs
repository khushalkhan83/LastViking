using System;
using Game.Controllers.Controllers.States;
using Game.Models;
using static Game.Models.InventoryCellsViewModelBase;

namespace Game.StateMachines.UseItemTutorial
{
    public interface IUseItemTutorialState : IState
    {
        void SetTargetItem(string targetItemName);
        void OnApplyItem(ItemsContainer container, CellModel cell);
        void OnChangeSelectedCell(CellInfo cellInfo);
        void OnInventoryOpened();
        void OnInventoryClosed();
        void OnPlayerDeath();
        void OnPlayerRevival();
    }
}