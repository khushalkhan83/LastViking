using Core;
using Core.Controllers;
using Game.Models;

namespace Game.Controllers
{
    public class RespawnPlayerOnGameStart : IRespawnPlayerOnGameStart, IController
    {
        [Inject] public PlayerRespawnPointsModel PlayerRespawnPointsModel { get; private set; }
        void IController.Enable() 
        {
            PlayerRespawnPointsModel.SetRespawnPlayerOnGameStart(true);
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            PlayerRespawnPointsModel.SetRespawnPlayerOnGameStart(false);
        }

    }
}
