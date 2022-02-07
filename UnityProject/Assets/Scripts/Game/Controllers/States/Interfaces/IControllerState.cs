using System;
namespace Game.Controllers.Controllers.States
{
    public interface IControllersState : IState
    {
        event Action<ControllerProcessingID, ControllerID[]> OnProcessState;
    }
}
