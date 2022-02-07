namespace Game.StateMachines.CraftItemTutorial
{
    public class TutorialStartCheck : CraftItemTutorialStateBase
    {
        public TutorialStartCheck(CraftItemTutorialStateMachine stateMachine) : base(stateMachine) { }

        #region ICraftItemTutorialState
        public override void Enter() => TryStartTutorial();
        public override void Exit() { }

        #endregion

        private void TryStartTutorial()
        {
            if (StartTutorial())
            {
                StateMachine.SetState(new MainScreen(StateMachine));
            }
            else
            {
                // we can subscribe for player items changed and try start tutorial when player have those items.
                // not implemented because hard to test.
            }
        }

        protected bool StartTutorial()
        {
            if(TeachBoostConditions) return true;

            bool canCraft = !ItemIsUnlocked && ItemCanBeCrafted;

            return canCraft;
        }
    }
}