using Core;
using Game.Models;

namespace Game.StateMachines.UseItemTutorial
{
    public abstract class UseItemTutorialStateBase : IUseItemTutorialState
    {
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        
        protected string TargetItemName {get; private set;}
        protected readonly UseItemTutorialStateMachine StateMachine;
        public UseItemTutorialStateBase(UseItemTutorialStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }
        #region IUseItemTutorialState
            
        public abstract void Enter();
        public abstract void Exit();

        public virtual void SetTargetItem(string targetItemName) => TargetItemName = targetItemName;
        public virtual void OnApplyItem(ItemsContainer container, CellModel cell)
        {
            if(cell.IsEmpty) return;

            if(cell.Item.Name == TargetItemName) StateMachine.SetState(new Success(StateMachine));
        }
        public virtual void OnChangeSelectedCell(InventoryCellsViewModelBase.CellInfo cellInfo) { }
        public virtual void OnInventoryOpened() { }
        public virtual void OnInventoryClosed() => ResetState();
        public virtual void OnPlayerDeath() => ResetState();
        public virtual void OnPlayerRevival() => ResetState();

        #endregion

        protected void ResetState()
        {
            if(PlayerIsDead)
            {
                StateMachine.SetState(new PlayerDeath(StateMachine));
            }
            else
            {
                StateMachine.SetState(new TutorialStartCheck(StateMachine));
            }
        }

        protected bool PlayerIsDead => PlayerHealthModel.IsDead;
    }
}