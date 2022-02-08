#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using CustomeEditorTools.EditorGameSettingsData;
using Extensions;
using UnityEditor;
using UnityEditor.Build;

class MainBuildPreprocessor : IPreprocessBuild
{
    private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;
    public int callbackOrder { get { return 1; } }
    LocalizationEditor window;

    private bool buildDeclined;
    private string buildDeclinedMessage;
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        buildDeclined = false;

        // HandleKeyStorePasswordCheck();
        // if (buildDeclined) throw new BuildFailedException(buildDeclinedMessage);

        // HandleDebugSettings();
        // if (buildDeclined) throw new BuildFailedException(buildDeclinedMessage);
        
        // HandleDebugSettings();
        // if (buildDeclined) throw new BuildFailedException(buildDeclinedMessage);

        // HandleAnalitics();
        // if (buildDeclined) throw new BuildFailedException(buildDeclinedMessage);

        // HandleOBBLoadingScene("OBB Loading scene name");
        // if (buildDeclined) throw new BuildFailedException(buildDeclinedMessage); // add correct scene here

        HandleArchitecture();
        if (buildDeclined) throw new BuildFailedException(buildDeclinedMessage);

        // LocalizationCheck();
        // if (buildDeclined) throw new BuildFailedException(buildDeclinedMessage);
    }

    private void LocalizationCheck()
    {
        if (buildDeclined) throw new BuildFailedException(buildDeclinedMessage);

        // window = EditorWindow.GetWindow<LocalizationEditor>();
        // window.StartLocalization();
        // do
        // {
        //     if(window.state == LocalizationEditor.State.ProcessingLocalizationFromURL)
        //     {
        //         window.WWWUpdate();
        //     }

        //     if(window.state == LocalizationEditor.State.Error)
        //     {
        //         buildDeclined = true;
        //         buildDeclinedMessage = "Ошибка при обновлении локализации";
        //     }

        // } while (window.state != LocalizationEditor.State.Idle);
    }

    private void HandleArchitecture()
    {
        #if UNITY_ANDROID

        bool releaseBuild = !EditorUserBuildSettings.development;

        if(releaseBuild)
        {
            var currentTargetArchitectures =  PlayerSettings.Android.targetArchitectures;
            var targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

            if(currentTargetArchitectures != targetArchitectures)
            {
                buildDeclined = true;
                buildDeclinedMessage = "For release build use all architectures";
            }
        }

        #endif
    }

    private void HandleDebugSettings()
    {
        bool useDebugControllers = EditorGameSettings.Instance.debugControllersSettings;
        bool releaseBuild = !EditorUserBuildSettings.development;

        
        if(releaseBuild)
        {
            if(!useDebugControllers) return;

            bool ok = EditorUtility.DisplayDialog("Внимание", "Билд release версии с debug контроллерами. Выключить debug контроллеры?", "Да", "Отмена билда");

            if(ok)
            {
                EditorGameSettings.Instance.debugControllersSettings = false;
            }
            else
            {
                buildDeclined = true;
                buildDeclinedMessage = "Пользователь отменил билд";
            }
        }
        else
        {
            if(useDebugControllers) return;

            bool ok = EditorUtility.DisplayDialog("Внимание", "Билд debug версии без debug контроллеров.", "Включить debug контроллеры", "Отмена билда");

            if(ok)
            {
                EditorGameSettings.Instance.debugControllersSettings = true;
            }
            else
            {
                buildDeclined = true;
                buildDeclinedMessage = "Пользователь отменил билд";
            }
        }
    }
    private void HandleAnalitics()
    {
        bool debugBuild = EditorUserBuildSettings.development;
        bool eneableAnalitics = EditorGameSettings.Instance.enableAnalitics;
        if (debugBuild && eneableAnalitics)
        {
            DisplayDialogWrapper(
                "Вы пытались сделать релизный билд с выключенной аналитикой.",
                positiveAction: null,"Продолжить");
        }
        else if (!debugBuild && !eneableAnalitics)
        {
            DisplayDialogWrapper(
                "Вы пытались сделать релизный билд с выключенной аналитикой.",
                positiveAction: () => {EditorGameSettings.Instance.enableAnalitics = true;},"Включить аналитику.");
        }
    }


    private void HandleKeyStorePasswordCheck()
    {
        bool notSigned = PlayerSettings.keystorePass == string.Empty || PlayerSettings.keyaliasPass == string.Empty;
        bool anroidBuild = EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;

        if(!EditorUserBuildSettings.development && anroidBuild && notSigned)
        {
            EditorUtility.DisplayDialog("Внимание", "Релиз билд отменен. Андроид версия требует подпись.", "ОК");

            buildDeclined = true;
            buildDeclinedMessage = "Автоматическая отмена билда";
        }
    }

    private void HandleOBBLoadingScene(string obbLoadingSceneName)
    {
        bool useABBFiles = EditorUserBuildSettings.buildAppBundle;
        if(useABBFiles)
        {
            EditorUtility.DisplayDialog("Внимание", $"Ошибка {useABBFiles.ToString()}: {useABBFiles} не поддерживается ", "ОК");

            buildDeclined = true;
            buildDeclinedMessage = "Отмена билда.";
            return;
        }
            
        bool androidBuild = EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;
        bool releaseBuild = !EditorUserBuildSettings.development;
        bool splitBinaryFiles = PlayerSettings.Android.useAPKExpansionFiles;

        if (androidBuild && releaseBuild && !splitBinaryFiles)
            DisplayDialogWrapperComplex("Попытка сделать релизный билд без разбивки на apk и obb файлы. Сделать билд с разбивкой на apk и obb файлы?",
            () => {
                PlayerSettings.Android.useAPKExpansionFiles = true;},
            null,
            "Нет (Amazon)");


        else if (androidBuild && !releaseBuild && splitBinaryFiles)
            DisplayDialogWrapperComplex(
            "Попытка сделать дебаг билд с разбивкой на apk и obb файлы. Сделать билд без разбивки?",
            () => {
                PlayerSettings.Android.useAPKExpansionFiles = false;},
            null);

        ChangeBuildSceneSettings();

        void ChangeBuildSceneSettings()
        {
            var original = EditorBuildSettings.scenes;
            var newSettings = new EditorBuildSettingsScene[EditorBuildSettings.scenes.Length];
            System.Array.Copy(original, newSettings, original.Length);

            for (int i = 0; i < newSettings.Length; i++)
            {
                var sceneSettings = newSettings[i];
                if (sceneSettings.path.Contains(obbLoadingSceneName))
                {
                    sceneSettings.enabled = PlayerSettings.Android.useAPKExpansionFiles;
                }
            }

            EditorBuildSettings.scenes = newSettings;
        }
    }

    private void DisplayDialogWrapper(string notification, Action positiveAction, string positiveMessage = "Да", string cancelMessage = "Отмена Билда")
    {
        bool ok = EditorUtility.DisplayDialog("Внимание",
            notification,
            positiveMessage,
            cancelMessage);

        if(ok)
        {
            positiveAction?.Invoke();
        }
        else
        {
            buildDeclined = true;
            buildDeclinedMessage = "Отмена билда";
        }
    }
    private void DisplayDialogWrapperComplex(string notification, Action positiveAction, Action alternativeAction, string alternativeMessage = "Нет", string positiveMessage = "Да", string cancelMessage ="Отмена Билда")
    {
        int option = EditorUtility.DisplayDialogComplex("Внимание",
            notification,
            positiveMessage,
            cancelMessage,
            alternativeMessage);

        switch (option)
        {
            // positiveMessage (Да)
            case 0:
                positiveAction?.Invoke();
                break;

            // cancelMessage (Отмена билда)
            case 1:
                buildDeclined = true;
                buildDeclinedMessage = "Отмена билда";
                break;

            // alternativeMessage (Нет)
            case 2:
                alternativeAction?.Invoke();
                break;

            default:
                break;
        }
    }
}
#endif
