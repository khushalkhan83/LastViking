using System;
using Game.Controllers.Controllers.States.Modificators;

namespace Game.Controllers.Controllers.States
{
    public class ModificatorStateMachine<T> : StateMachineBase<T> where T : ModificatorBase
    {
        public ModificatorStateMachine(Action<ControllerID> activateControllerStrategy,
                                       Action<ControllerID> deactivateControllerStrategy,
                                       Action<ControllerID> startControllerStrategy)
        {
            ActivateControllerStrategy = activateControllerStrategy;
            DeactivateControllerStrategy = deactivateControllerStrategy;
            StartControllerStrategy = startControllerStrategy;
        }

        public Action<ControllerID> ActivateControllerStrategy { get; }
        public Action<ControllerID> DeactivateControllerStrategy { get; }
        public Action<ControllerID> StartControllerStrategy { get; }

        private bool NoCurrentState => CurrentState == null;
        

        public override void SetState(T newState)
        {
            var oldState = CurrentState;

            Subscribe(oldState);
            Subscribe(newState);
            
            base.SetState(newState);

            UnSubscribe(oldState);
            UnSubscribe(newState);
        }

        protected virtual void Subscribe(T state)
        {
            if(state == null) return;

            state.OnActivateController += ActivateControllerStrategy;
            state.OnDeactivateController += DeactivateControllerStrategy;
            state.OnStartController += StartControllerStrategy;
        }
        
        protected virtual void UnSubscribe(T state)
        {
            if(state == null) return;

            state.OnActivateController -= ActivateControllerStrategy;
            state.OnDeactivateController -= DeactivateControllerStrategy;
            state.OnStartController -= StartControllerStrategy;
        }

        public bool ActivatedController(ControllerID controller)
        {
            if(NoCurrentState) return false;

            bool activated = CurrentState.CanActivate(controller);
            return activated;
        }

        public bool DeActivatedController(ControllerID controller)
        {
            if(NoCurrentState) return false;

            bool deactivated = CurrentState.CanDeactivate(controller);
            return deactivated;
        }
    }

    // public class DependentModificatorStateMachine<T> : ModificatorStateMachine<T> where T : StateModificatorAbstract where T2: ModificatorStateMachine<StateModificatorAbstract>
    // {
    //     public DependentModificatorStateMachine(
    //         Action<ControllerID> activateControllerStrategy,
    //         Action<ControllerID> deactivateControllerStrategy,
    //         ModificatorStateMachine<T> dependency)
    //         : base(activateControllerStrategy, deactivateControllerStrategy)
    //     {
    //         Dependency = dependency;
    //     }

    //     public T2 Dependency { get; }

    //     protected override void Subscribe(T state)
    //     {
    //         state.OnActivateController += ConditionalActivation;
    //         state.OnDeactivateController += ConditionalDeactivation;
    //     }

    //     protected override void UnSubscribe(T state)
    //     {
    //         state.OnActivateController -= ConditionalActivation;
    //         state.OnDeactivateController -= ConditionalDeactivation;
    //     }


    //     private void ConditionalActivation(ControllerID controller)
    //     {
    //         bool ignore = Dependency.DeActivatedController(controller);
    //         if(ignore) return;

    //         ActivateControllerStrategy?.Invoke(controller);
    //     }

    //     private void ConditionalDeactivation(ControllerID controller)
    //     {
    //         bool ignore = Dependency.ActivatedController(controller);
    //         if(ignore) return;

    //         DeactivateControllerStrategy?.Invoke(controller);
    //     }
    // }
}
