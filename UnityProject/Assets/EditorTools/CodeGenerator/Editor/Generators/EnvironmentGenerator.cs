using Extensions;
using Game.Providers;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace CustomeEditorTools.CodeGenerator
{
    [CreateAssetMenu(fileName = "EnvironmentGenerator", menuName = "GeneratorData/SO_Generator_Environment", order = 0)]
    public class EnvironmentGenerator : CodeGeneratorBase
    {
        [SerializeField] private string environmentName;
        [SerializeField] private Settings settings;

        private string NewScenePath => settings.EnvironmentScenesFolderPath + "/" + environmentName + ".unity";


        [Button]
        private void STEPS_All()
        {
            /// add env id
            GenerateEnvironmentID(false);
            /// add "point" assets
            //TODO: implement ?
            
            // add scene name to scenes provider
            AddSceneNameToProvider();

            // create and add files for chunk configs
            //TODO: implement ?

            /// create scene asset
            CreateSceneAsset(false);
            /// add scene to build index
            AddSceneToBuildSettings();
            
            AssetDatabase.Refresh();
            CacheCreatedFile(NewScenePath);
        }

        [Button] void STEP_1_GenerateEnvironmentID() { GenerateEnvironmentID(true);}
        [Button] void STEP_2_AddSceneNameToProvider() { AddSceneNameToProvider();}
        [Button] void STEP_3_CreateSceneAsset() { CreateSceneAsset(true); CacheCreatedFile(NewScenePath);}
        [Button] void STEP_4_AddSceneToBuildSettings() { AddSceneToBuildSettings();}

        private void GenerateEnvironmentID(bool refresh)
        {
            var path = settings.EnvironmentSceneIDFilePath;
            bool error = !TryGetClosestNumInLineAboveTag("///CODE_GENERATION_IDS", path, out int lastIDNum);

            if (error)
            {
                Debug.LogError("Error here");
                return;
            }

            AddTextAboveTag("///CODE_GENERATION_IDS", path, GetIDLine(), "        ");
            if (refresh) AssetDatabase.Refresh();

            string GetIDLine()
            {
                var controllerIDName = this.environmentName;
                var id = lastIDNum + 1;
                var answer = controllerIDName + " = " + id + ",";
                return answer;
            }
        }

        private void AddSceneNameToProvider()
        {
            settings.SceneNamesProvider.ExpandAndAddSceneName(environmentName);
            EditorUtility.SetDirty(settings.SceneNamesProvider);
        }

        private void CreateSceneAsset(bool refresh)
        {
            FileUtil.CopyFileOrDirectory( settings.TeamplateEnvironmentScenePath, NewScenePath);
            if (refresh) AssetDatabase.Refresh();
        }

        private void AddSceneToBuildSettings(bool markAsEnabled = true)
        {
            var original = EditorBuildSettings.scenes;
            var newSettings = new EditorBuildSettingsScene[original.Length + 1];
            System.Array.Copy(original, newSettings, original.Length);
            var sceneToAdd = new EditorBuildSettingsScene(NewScenePath, markAsEnabled);
            newSettings[newSettings.Length - 1] = sceneToAdd;
            EditorBuildSettings.scenes = newSettings;
        }


        //TODO: add functionality to test in game
        //TODO: add play mode backed light preview ?


        // TODO: add test for scene transitions


        [System.Serializable]
        public class Settings
        {
            [SerializeField] private UnityEngine.Object environmentSceneIDFile;
            [SerializeField] private UnityEngine.Object teamplateEnvironmentScene;
            [SerializeField] private UnityEngine.Object environmentScenesFolder;
            [SerializeField] private SceneNamesProvider sceneNamesProvider;

            public string EnvironmentSceneIDFilePath => environmentSceneIDFile.Path();
            public string TeamplateEnvironmentScenePath => teamplateEnvironmentScene.Path();
            public string EnvironmentScenesFolderPath => environmentScenesFolder.Path();
            public SceneNamesProvider SceneNamesProvider => sceneNamesProvider;
        }
    }
}