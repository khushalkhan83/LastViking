
using Game.Controllers;
using Game.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomeEditorTools
{
    public static class RestartTools
    {
        private static EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;
        private static StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private static ControllersModel ControllersModel => ModelsSystem.Instance._controllersModel;

        [MenuItem("EditorTools/GameLoop/RestartCore _F6")]
        public static void RestartCore()
        {
            if(!Application.isPlaying) return;

            ControllersModel.ApplyState(ControllersStateID.None);

            var coreScene = EditorGameSettings.scenesProfile_Default.coreScene;
            var path = AssetDatabase.GetAssetPath(coreScene);
            SceneManager.LoadScene(path);
        }

        [MenuItem("EditorTools/GameLoop/EmulateRestartGame")]
        public static void EmulateRestartGame()
        {
            if(!Application.isPlaying) return;

            ControllersModel.ApplyState(ControllersStateID.None);

            var loadingScene = EditorGameSettings.scenesProfile_Default.loadingScene;
            var path = AssetDatabase.GetAssetPath(loadingScene);
            SceneManager.LoadScene(path);
        }
        [MenuItem("EditorTools/GameLoop/Save game _F5")]
        public static void SaveGame()
        {
            if(!Application.isPlaying) return;

            StorageModel.SaveChanged();
        }
    }
}