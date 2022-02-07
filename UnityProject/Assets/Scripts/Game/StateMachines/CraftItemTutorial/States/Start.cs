using Core;
using Game.Models;

namespace Game.StateMachines.CraftItemTutorial
{
    public class Start : CraftItemTutorialStateBase
    {
        [Inject] public PlayerScenesModel PlayerScenesModel { get; private set; }
      
        public Start(CraftItemTutorialStateMachine stateMachine) : base(stateMachine) { }

        #region IUseItemTutorialState
        public override void Enter()
        {
            if(PlayerScenesModel.SceneLoading)
                PlayerScenesModel.OnEnvironmentLoaded += ResetState;
            else
                ResetState();
        }

        public override void Exit()
        {
            PlayerScenesModel.OnEnvironmentLoaded -= ResetState;
        }

        #endregion

    }
}