using Core;
using Core.Controllers;
using Game.Models;
using UltimateSurvival;

namespace Game.Controllers
{
    public class FixMovePlayerFromDungeonController : IFixMovePlayerFromDungeonController, IController
    {
        [Inject] public FixMovePlayerFromDungeonModel FixMovePlayerFromDungeonModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public PlayerScenesModel PlayerScenesModel { get; private set; }

        private EnvironmentSceneID DefaultSceneID => PlayerScenesModel.DefaultSceneID;

        void IController.Enable() 
        {
            FixMovePlayerFromDungeonModel.Init();

            if(FixMovePlayerFromDungeonModel.FixApplied) return;


            PlayerScenesModel.Init();

            bool playerInDungeonLocation = !PlayerScenesModel.PlayerOnLocation(DefaultSceneID);

            if(playerInDungeonLocation)
            {
                PlayerScenesModel.ActiveEnvironmentSceneID = DefaultSceneID;
                PlayerEventHandler.Init(defaultPosition:true);
            }

            FixMovePlayerFromDungeonModel.SetFixApplied();
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
        }
    }
}
