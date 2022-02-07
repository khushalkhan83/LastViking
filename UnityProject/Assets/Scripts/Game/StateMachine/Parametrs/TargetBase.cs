using Core.States.Parametrs;
using Game.AI;

namespace Game.StateMachine.Parametrs
{
    public abstract class TargetBase : ParametrBase
    {
        public abstract Target Target { get; }
    }
}
