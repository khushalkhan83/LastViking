using Core;
using Core.Controllers;
using Game.Models;

namespace Game.Controllers
{
    public class SpawnTombAtPlayerRespawnPoint : ISpawnTombAtPlayerRespawnPoint, IController
    {
        [Inject] public TombsModel TombsModel { get; private set; }
        void IController.Enable() 
        {
            TombsModel.SetTombCreationType(TombsModel.CreationType.RespawnPoint);
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            TombsModel.SetTombCreationType(TombsModel.CreationType.Default);
        }
    }
}
