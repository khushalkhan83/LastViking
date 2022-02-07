using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Linq;
using CustomeEditorTools.EditorGameSettingsData;

[InitializeOnLoadAttribute]

// Unload scenes except active scene before Play Mode
// Load them back when enter Editor Mode 

//original script https://answers.unity.com/questions/1134925/cannot-closeunload-a-scene-that-is-open-in-editor.html
public static class EditorScenes
{
    static EditorScenes()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if(EditorGameSettings.multiScenePlayMode == false) return;

        if(enterPlayModeWasCanceled)
        {
            enterPlayModeWasCanceled = false;
            return;
        }

        if (state == PlayModeStateChange.EnteredEditMode) GoToEditMode();
        if (state == PlayModeStateChange.ExitingEditMode) GoToPlayMode();
    }

    private static EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;
    private static List<string> UnloadedScenesBeforeStartPlayMode {get => EditorGameSettings.unloadedScenesBeforeStartPlayMode;}
    private static string ActiveSceneNameBefourePlayMode {get => EditorGameSettings.activeSceneBefourePlayMode; set => EditorGameSettings.activeSceneBefourePlayMode = value;}
    private static List<Scene> allScenes = new List<Scene>();

    private static bool enterPlayModeWasCanceled;

    // -----------------------------------------------------

    private static void GoToEditMode()
    {
        bool customePlayMode = TryCustomePlayModeRestore();
        if(customePlayMode) return;


        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (!IsActiveScene(scene))
            {
                if (UnloadedScenesBeforeStartPlayMode.Contains(scene.name))
                {
                    OpenScene(scene);
                    continue;
                }
            }
        }
    }

    private static void GoToPlayMode()
    {
        bool unsavedScenes = false;
        UnloadedScenesBeforeStartPlayMode.Clear();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            allScenes.Add(scene);

            if(IsActiveScene(scene)) ActiveSceneNameBefourePlayMode = scene.name;

            if (!IsActiveScene(scene) && scene.isLoaded)
            {
                UnloadedScenesBeforeStartPlayMode.Add(scene.name);
            }

            // кор выбран как главная сцена и не выгражается. Мы не далаем проверку для него
            if(scene.isDirty && scene.name != "CoreScene") unsavedScenes = true;
        }


        if(unsavedScenes)
        {
            var answer = EditorUtility.DisplayDialogComplex("Внимание", SomeScenesNotSavedMessage(), "Сохранить и запустить", "Отмена","Запустить без сохранения");

            switch (answer)
            {
                case 0: //"Сохранить и запустить"
                    UnloadScenesButSaveThemBefore();
                    break;
                case 1: //"Отмена" или Escape button
                    Cancel(); 
                    break;
                case 2: //"Запустить без сохранения"
                    UnloadScenesWithoutSave();
                    break;
            }
        }
        else
        {
            UnloadActiveScenesAndFinalize();
        }

        void UnloadScenesButSaveThemBefore()
        {
            EditorSceneManager.SaveScenes(GetScenesByNames(UnloadedScenesBeforeStartPlayMode).ToArray());
            UnloadActiveScenesAndFinalize();
        }

        void Cancel()
        {
            enterPlayModeWasCanceled = true;
            EditorApplication.isPlaying = false;
        }

        void UnloadScenesWithoutSave()
        {
            UnloadActiveScenesAndFinalize();
        }


        void UnloadActiveScenesAndFinalize()
        {
            // unload active scenes
            foreach (var sceneNameToUnload in UnloadedScenesBeforeStartPlayMode)
            {
                CloseScene(sceneNameToUnload);
            }

            // finalize
            bool custome = TryCustomePlayModeEntery();

            if(!custome && EditorGameSettings.startInSelectedEnvironment)
            {
                // if()
            }
        }
    }

    private static bool TryCustomePlayModeEntery()
    {
        bool custome = EditorGameSettings.customePlayMode;
        if(!custome) return custome;

        var scenesData = EditorGameSettings.scenesProfile_Default;
        var targetScene =  EditorGameSettings.playModeStartPoint == CustomePlayModeStartPoint.CoreScene ? scenesData.coreScene : scenesData.loadingScene;

        var scenePath = AssetDatabase.GetAssetPath(targetScene);

        OpenSceneSingle(scenePath);

        return custome;
    }

    private static bool TryCustomePlayModeRestore()
    {
        if(!EditorGameSettings.customePlayMode) return false;

        bool sceneExist = TryGetActiveSceneBefourePlayMode(out var activeSceneBefourePlayMode);

        if(!sceneExist) return false;


        OpenSceneSingle(activeSceneBefourePlayMode);

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (!IsActiveScene(scene))
            {
                if (UnloadedScenesBeforeStartPlayMode.Contains(scene.name))
                {
                    OpenScene(scene);
                    continue;
                }
            }
        }
        
        return true;
    }

    private static bool TryGetActiveSceneBefourePlayMode(out Scene result)
    {
        result = new Scene();
        var scenesWithThisName = GetScenesByNames(new List<string>{ActiveSceneNameBefourePlayMode});
        if(scenesWithThisName == null || scenesWithThisName.Count == 0) return false;

        result = scenesWithThisName.First();
        return true;
    }

    // -----------------------------------------------------

    private static void OpenSceneSingle(string path)
    {
        EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
    }
    private static void OpenSceneSingle(Scene scene)
    {
        EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
    }
    private static void OpenScene(Scene scene)
    {
        EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
    }
    private static void OpenScene(string path)
    {
        EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
    }

    private static void CloseScene(Scene scene)
    {
        EditorSceneManager.CloseScene(scene, false);
    }

    private static void CloseScene(string sceneName)
    {
        var scene = EditorSceneManager.GetSceneByName(sceneName);
        EditorSceneManager.CloseScene(scene, false);
    }

    private static void UnloadScene(Scene scene)
    {
        EditorSceneManager.UnloadSceneAsync(scene);
    }

    // -----------------------------------------------------

    private static Scene activeScene
    {
        get { return SceneManager.GetActiveScene(); }
    }

    private static bool IsActiveScene(Scene scene)
    {
        return scene == activeScene;
    }

    private static List<Scene> GetScenesByNames(List<string> scenesNames)
    {
        var answer = new List<Scene>();
        
        foreach (var sceneName in scenesNames)
            answer.Add(EditorSceneManager.GetSceneByName(sceneName));
        
        return answer;
    }

    private static string SomeScenesNotSavedMessage()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Сцены не сохранены");

        GetScenesByNames(UnloadedScenesBeforeStartPlayMode).Where(x => x.isDirty).ToList().ForEach(x => sb.AppendLine(x.name));

        return sb.ToString();
    }
}