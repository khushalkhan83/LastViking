using Core;
using Game.Controllers.Controllers.States;
using UnityEngine;

namespace Game.StateMachines.CraftItemTutorial
{
    public class CraftItemTutorialStateMachine : StateMachineBase<ICraftItemTutorialState>
    {
        [Inject] public InjectionSystem InjectionSystem { get; private set; }
        public CraftItemTutorialStateMachine(string targetItemID, bool teachBoost)
        {
            TargetItemName = targetItemID;
            TeachBoost = teachBoost;
        }

        protected readonly string TargetItemName;
        public bool TeachBoost {get; private set;}

        public override void SetState(ICraftItemTutorialState state)
        {
            Debug.Log($"<color=orange> state {GetStateMessage(state)}  </color>");
            if(state != null)
            {
                InjectionSystem.Inject(state);
                state.SetTargetItem(TargetItemName);
            }

            base.SetState(state);
        }

        private string GetStateMessage(ICraftItemTutorialState state)
        {
            return state == null ? "Null" : state.GetType().ToString();
        }
    }
}