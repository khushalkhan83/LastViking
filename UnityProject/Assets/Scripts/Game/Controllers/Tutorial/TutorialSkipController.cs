using System;
using Core;
using Core.Controllers;
using Game.Models;

namespace Game.Controllers
{
    public class TutorialSkipController : ITutorialSkipController, IController
    {
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public SheltersModel SheltersModel { get; private set; }
        [Inject] public ShelterUpgradeModel ShelterUpgradeModel { get; private set; }
        [Inject] public ConstructionDockModel ConstructionDockModel { get; private set; }

        void IController.Enable() 
        {
            TutorialModel.OnSkipTutorial += SkipTutorialHandler;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            TutorialModel.OnSkipTutorial -= SkipTutorialHandler;
        }

        private void SkipTutorialHandler()
        {
            InitializeShip();
            ConstructionDockModel.BuildDock();
        }

        private void InitializeShip()
        {
            bool shelterNotCreated = SheltersModel.ShelterActive == ShelterModelID.None;
            if(shelterNotCreated)
            {
                ShelterUpgradeModel.CompleteUpgrad();
            }
        }
    }
}
