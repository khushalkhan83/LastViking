using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;
using UnityEngine.SceneManagement;

namespace Game.Controllers
{
    public class DisableLightController : IDisableLightController, IController
    {
        [Inject] public LightModel LightModel { get; private set; }
        [Inject] public PlayerScenesModel PlayerScenesModel { get; private set; }
        [Inject] public SceneNamesProvider SceneNamesProvider { get; private set; }

        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;

        void IController.Enable() 
        {
            LightModel.gameObject.SetActive(false);

            if(EditorGameSettings.EmulateSceneLight)
            {
                SetEnvironmetnSceneAsMain();
            }
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            LightModel.gameObject.SetActive(true);

            if(EditorGameSettings.EmulateSceneLight)
            {
                SetCoreSceneAsMain();
            }
        }

        private void SetEnvironmetnSceneAsMain()
        {
            string sceneName = SceneNamesProvider[PlayerScenesModel.ActiveEnvironmentSceneID];
            Scene scene = Helpers.ScenesHelper.GetLoadedSceneByName(sceneName);
            SceneManager.SetActiveScene(scene);
        }

        private void SetCoreSceneAsMain()
        {
            Scene scene = Helpers.ScenesHelper.GetLoadedSceneByName("CoreScene");
            SceneManager.SetActiveScene(scene);
        }
    }
}