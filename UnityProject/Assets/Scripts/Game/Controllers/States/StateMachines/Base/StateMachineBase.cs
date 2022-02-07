namespace Game.Controllers.Controllers.States
{
    public abstract class StateMachineBase<T> where T : class, IState
    {
        private T currentState;
        public T CurrentState => currentState;
        public virtual void SetState(T newState)
        {
            if (currentState == newState) return;

            currentState?.Exit();
            currentState = newState;
            currentState?.Enter();
        }
    }
}
