#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Build;
using System.Linq;

class DebugAssetsBuildPreprocessor : IPreprocessBuild
{
    private static EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;
    public int callbackOrder { get { return 2; } }

    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        bool debugMode = EditorGameSettings.debugControllersSettings;

        var settings = EditorGameSettings.settings;
        bool stopBuild = false;
        string stopBuildMessage = string.Empty;

        var debugAssetsLocatedInPackages = settings.DebugAssets.Assets.Where(x => x.IsLocataedInPackage).ToList();
        if(debugMode)
        {
            var packagesToLoad = debugAssetsLocatedInPackages.FindAll(x => !x.PackageImported);
            if(packagesToLoad.Count > 0)
            {
                var packagesToLoadCountText = debugAssetsLocatedInPackages.Count - packagesToLoad.Count + "/" + debugAssetsLocatedInPackages.Count;
                stopBuildMessage = "Билд отменен. Импортируйте все debug пакеты (EditorGameSettings, секция Build, ImportDebugPackages button)";
                EditorUtility.DisplayDialog("Загружено " + packagesToLoadCountText, stopBuildMessage, "Ок");
                stopBuild = true;
            }
        }
        else
        {
            var packagesToRemove = debugAssetsLocatedInPackages.FindAll(x => x.PackageImported);
            if(packagesToRemove.Count > 0)
            {
                var packagesToRemoveCountText = debugAssetsLocatedInPackages.Count - packagesToRemove.Count + "/" + debugAssetsLocatedInPackages.Count;
                stopBuildMessage = "Билд отменен. Удалите все debug пакеты (EditorGameSettings, секция Build, ExcludeDebugPackages button)";
                EditorUtility.DisplayDialog("Выгружено " + packagesToRemoveCountText, stopBuildMessage , "Ок");
                stopBuild = true;
                
            }
        }

        // IMPORTANT !. call all operations in settings befoure calling RemoveDebugReferences, because they can set DebugAssets property
        settings.ManageDevFolders(excludeDevFolders: !debugMode);
        
        if(stopBuild) throw new BuildFailedException(stopBuildMessage);


        // not working as exepted, call manualy via EditorGameSettings
        // settings.ManagePackages(exclude: !debugMode);

        if(debugMode)
        {
            settings.AddReferencesForDebugBuild();
        }
        else
        {
            settings.RemoveDebugReferences();
        }
    }
}
#endif
