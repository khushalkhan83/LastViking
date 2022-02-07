using UnityEditor;
using UnityEditor.SceneManagement;
using Helpers;

namespace CustomeEditorTools
{
    public static class OpenScenesWorkspace
    {
        [MenuItem("EditorTools/Scene/Open scenes workspace")]
        public static void OpenScenes()
        {
            if(EditorApplication.isPlaying)
            {
                EditorUtility.DisplayDialog("Error", "Действует только в Edit mode", "OK");
                return;
            }

            var loadedScenes = ScenesHelper.GetLoadedScenes();
            bool cancel = !EditorSceneManager.SaveModifiedScenesIfUserWantsTo(loadedScenes);

            if(cancel) return;



            var coreSceneObject = EditorGameSettings.Instance.scenesProfile_Default.coreScene;
            var coreScenePath = AssetDatabase.GetAssetPath(coreSceneObject);

            EditorSceneManager.OpenScene(coreScenePath, OpenSceneMode.Single);

            var environmentScenes = EditorGameSettings.Instance.scenesProfile_Default.environmentScenes;

            foreach (var sceneObject in environmentScenes)
            {
                var scenePath = AssetDatabase.GetAssetPath(sceneObject);
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.AdditiveWithoutLoading);
            }
        }
    }
}
