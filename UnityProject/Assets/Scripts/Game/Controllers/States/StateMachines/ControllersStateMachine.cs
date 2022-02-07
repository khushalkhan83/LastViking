using System;
using Game.Controllers.Controllers.States;

namespace Game.Controllers.Controllers
{
    public class ControllersStateMachine : StateMachineBase<IControllersState>
    {
        public ControllersStateMachine(Action<ControllerProcessingID, ControllerID[]> processStateStrategy)
        {
            ProcessStateStrategy = processStateStrategy;
        }
        private Action<ControllerProcessingID, ControllerID[]> ProcessStateStrategy {get;}

        public Action<ControllerID> ActivateControllerStrategy { get; }
        public Action<ControllerID> DeactivateControllerStrategy { get; }
        public Action<ControllerID> StartControllerStrategy { get; }


        public override void SetState(IControllersState newState)
        {
            var oldState = CurrentState;

            Subscribe(oldState);
            Subscribe(newState);
            
            base.SetState(newState);

            UnSubscribe(oldState);
            UnSubscribe(newState);
        }

        protected virtual void Subscribe(IControllersState state)
        {
            if(state == null) return;

            state.OnProcessState += ProcessStateStrategy;
        }
        
        protected virtual void UnSubscribe(IControllersState state)
        {
            if(state == null) return;

            state.OnProcessState -= ProcessStateStrategy;
        }

    }
}
