using Core;
using Extensions;
using Game.Models;
using static Game.Models.CraftViewModel;
using ContainerID = Game.Models.CraftViewModel.ContainerID;


namespace Game.StateMachines.CraftItemTutorial
{
    public class HilightTargetItem : HilightTargetItemBase
    {
        public HilightTargetItem(CraftItemTutorialStateMachine stateMachine) : base(stateMachine) { }

        #region ICraftItemTutorialState

        public override void OnChangeSelectedHandler(CellInfo cellInfo)
        {
            if(IsTargetItem(cellInfo))
            {
                if(TeachBoostConditions)
                    StateMachine.SetState(new HilightBoostButton(StateMachine));
                else
                    StateMachine.SetState(new HilightCraftButton(StateMachine));
            }
        }

        #endregion

        private bool IsTargetItem(CellInfo cellInfo)
        {
            var index = GetTargetCellIndex();
            return cellInfo.cellId == index;
        }
    }
}