using System;
using Core;
using Core.Controllers;
using Game.Models;
using Game.VillageBuilding;

namespace Game.Controllers
{
    public class VillageCinematicsController : IVillageCinematicsController, IController
    {
        [Inject] public VillageBuildingModel VillageBuildingModel { get; private set; }
        [Inject] public VillageCinematicsModel VillageCinematicsModel { get; private set; }

        private HouseBuilding Townhall => VillageBuildingModel.Townhall;

        void IController.Enable() 
        {
            Townhall.OnCompleteBuildingProcess += OnTownHallUpgradeCompleated;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            Townhall.OnCompleteBuildingProcess -= OnTownHallUpgradeCompleated;
        }

        private void OnTownHallUpgradeCompleated(HouseBuilding houseBuilding)
        {
            VillageCinematicsModel.HallCinematicStart();
        }
    }
}
