using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;

namespace Game.Controllers
{
    public class SceneControllerActivationController : ISceneControllerActivationController, IController
    {
        [Inject] public PlayerScenesModel PlayerScenesModel { get; private set; }
        [Inject] public ControllersModel ControllersModel { get; private set; }
        [Inject] public LocationModificatorsProvider LocationModificatorsProvider {get ; private set; }

        void IController.Enable() 
        {
            PlayerScenesModel.OnPreEnvironmentLoaded += OnPreEnvironmentLoaded;
            PlayerScenesModel.OnPreEnvironmentChange += OnPreEnvironmentChange;
        }

        void IController.Start()
        {
        }

        void IController.Disable() 
        {
            PlayerScenesModel.OnPreEnvironmentLoaded -= OnPreEnvironmentLoaded;
            PlayerScenesModel.OnPreEnvironmentChange -= OnPreEnvironmentChange;
        }

        private void OnPreEnvironmentChange()
        {
            var noModificator = LocationModificatorsProvider.GetNullValue();
            ControllersModel.ApplyModificator(noModificator);
        }

        private void OnPreEnvironmentLoaded()
        {
            var modificator = LocationModificatorsProvider[PlayerScenesModel.ActiveEnvironmentSceneID];
            ControllersModel.ApplyModificator(modificator);
        }
    }
}
