using System;
using Core;
using Core.Controllers;
using Game.Models;

namespace Game.Controllers
{
    public class EnvironmentTransitionsController : IEnvironmentTransitionsController, IController
    {
        [Inject] public EnvironmentTransitionsModel EnvironmentTransitionsModel { get; private set; }
        [Inject] public ShelterAttackModeModel ShelterAttackModeModel { get; private set; }
        void IController.Enable() 
        {
            EnvironmentTransitionsModel.OnGetCanGoToOtherLocation += GetCanGoToOtherLocation;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            EnvironmentTransitionsModel.OnGetCanGoToOtherLocation -= GetCanGoToOtherLocation;
        }

        private bool GetCanGoToOtherLocation()
        {
            bool canGoToOtherLocation = !ShelterAttackModeModel.AttackModeActive;
            return canGoToOtherLocation;
        }
    }
}
