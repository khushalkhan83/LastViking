namespace Game.StateMachines.UseItemTutorial
{
    public class PlayerDeath : UseItemTutorialStateBase
    {
        public PlayerDeath(UseItemTutorialStateMachine stateMachine) : base(stateMachine) { }

        #region IUseItemTutorialState
        public override void Enter() { }
        public override void Exit() { }
            
        #endregion
    }
}