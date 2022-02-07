namespace Game.StateMachines.CraftItemTutorial
{
    public class PlayerDeath : CraftItemTutorialStateBase
    {
        public PlayerDeath(CraftItemTutorialStateMachine stateMachine) : base(stateMachine) { }

        #region IUseItemTutorialState
        public override void Enter() { }
        public override void Exit() { }
            
        #endregion
    }
}