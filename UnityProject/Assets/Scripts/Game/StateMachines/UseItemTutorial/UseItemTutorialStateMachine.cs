using Core;
using Game.Controllers.Controllers.States;
using UnityEngine;

namespace Game.StateMachines.UseItemTutorial
{
    public class UseItemTutorialStateMachine : StateMachineBase<IUseItemTutorialState>
    {
        [Inject] public InjectionSystem InjectionSystem { get; private set; }
        public UseItemTutorialStateMachine(string targetItemName) => this.TargetItemName = targetItemName;

        protected readonly string TargetItemName;

        public override void SetState(IUseItemTutorialState state)
        {
            Debug.Log($"<color=orange> state {GetStateMessage(state)}  </color>");
            if(state != null)
            {
                InjectionSystem.Inject(state);
                state.SetTargetItem(TargetItemName);
            }

            base.SetState(state);
        }

        private string GetStateMessage(IUseItemTutorialState state)
        {
            return state == null ? "Null" : state.GetType().ToString();
        }
    }
}