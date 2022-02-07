using UnityEngine.SceneManagement;
using System.Linq;

namespace Helpers
{
    public static class ScenesHelper
    {
// #if UNITY_EDITOR

        public static bool IsSceneOpened(string sceneName)
        {
            var scene = GetLoadedSceneByName(sceneName);

            bool sceneIsOpened = !string.IsNullOrEmpty(scene.name); 
            return sceneIsOpened;
        }

        public static bool IsSceneOpenedAndLoaded(string sceneName)
        {
            var scene = GetLoadedSceneByName(sceneName);

            if(scene == null) return false;

            return scene.isLoaded;
        }

        public static Scene GetLoadedSceneByName(string sceneName)
        {
            Scene[] loadedScenes = GetLoadedScenes();

            var match = loadedScenes.ToList().Find(x => x.name == sceneName);

            return match;
        }

        public static Scene[] GetLoadedScenes()
        {
            int countLoaded = SceneManager.sceneCount;
            Scene[] loadedScenes = new Scene[countLoaded];

            for (int i = 0; i < countLoaded; i++)
            {
                loadedScenes[i] = SceneManager.GetSceneAt(i);
            }

            return loadedScenes;
        }
// #endif
    }

}