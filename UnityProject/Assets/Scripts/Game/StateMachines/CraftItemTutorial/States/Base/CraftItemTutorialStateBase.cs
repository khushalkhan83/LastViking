using System.Linq;
using Core;
using Game.Models;
using Game.Purchases;
using UltimateSurvival;

namespace Game.StateMachines.CraftItemTutorial
{
    public abstract class CraftItemTutorialStateBase : ICraftItemTutorialState
    {
        private const PurchaseID BoostPurchase = PurchaseID.CraftBoostGold;

        [Inject] public CraftModel CraftModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public InventoryOperationsModel InventoryOperationsModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }

        protected string TargetItemName { get; private set; }
        protected readonly CraftItemTutorialStateMachine StateMachine;
        public CraftItemTutorialStateBase(CraftItemTutorialStateMachine stateMachine)
        {
            StateMachine = stateMachine;
        }
        #region ICraftItemTutorialState
        public abstract void Enter();
        public abstract void Exit();

        public virtual void SetTargetItem(string targetItemName) => TargetItemName = targetItemName;
        public virtual void OnApplyItem(ItemsContainer container, CellModel cell)
        {
            if (cell.IsEmpty) return;

            if (cell.Item.Name == TargetItemName) StateMachine.SetState(new Success(StateMachine));
        }
        public virtual void OnShowCraft() { }
        public virtual void OnHideCraft() => ResetState();
        public virtual void OnCraftItemStarted(int itemId)
        {
            ICraftItemTutorialState nextStage;

            if(TeachBoostConditions)
                nextStage = new HilightBoostButton(StateMachine);
            else
                nextStage = new Success(StateMachine);

            StateMachine.SetState(nextStage);
        }
        public virtual void OnBoost() => StateMachine.SetState(new Success(StateMachine));
        public virtual void OnChangeSelectedHandler(CraftViewModel.CellInfo cellInfo) { }
        public virtual void OnPlayerDeath() => ResetState();
        public virtual void OnPlayerRevival() => ResetState();

        #endregion

        protected bool IsTargetItem(int itemId) => GetTargetItemData().Id == itemId;
        protected bool ItemIsCrafting
        {
            get
            {
                var itemId = GetTargetItemID();
                bool crafting = CraftModel.Craft.Where(x => x.ItemData.Id == itemId).Count() > 0;
                if(crafting) return true;
                else
                {
                    bool inQueue = CraftModel.Queue.Where(x => x.Id == itemId).Count() > 0;
                    return inQueue;
                }
            }
        }
        protected bool ItemIsUnlocked => CraftModel.IsUnlocked(GetTargetItemID());
        protected bool ItemCanBeCrafted => InventoryOperationsModel.PlayerHasItems(GetTargetItemData().Recipe.RequiredItems);

        private ItemData GetTargetItemData() => ItemsDB.GetItem(TargetItemName);
        private int GetTargetItemID() => GetTargetItemData().Id;

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

        private bool PlayerIsDead => PlayerHealthModel.IsDead;
        private bool TeachBoost => StateMachine.TeachBoost;
        private bool IsBoostNow => CraftModel.IsBoostNow;
        protected bool TeachBoostConditions => CanPurchaseBoost && ItemIsCrafting && TeachBoost && !IsBoostNow;
        private bool CanPurchaseBoost => PurchasesModel.IsCanPurchase(BoostPurchase);
    }
}