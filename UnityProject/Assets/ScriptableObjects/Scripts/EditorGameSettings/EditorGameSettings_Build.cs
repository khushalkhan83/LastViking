using System;
using System.Linq;
using CustomeEditorTools.EditorGameSettingsData;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public partial class EditorGameSettings
{
    [TabGroup("Build")]
    [Tooltip("Включить для доступа к debug кнопкам и проч настройкам")]
    public bool debugControllersSettings;

    [TabGroup("Build")]
    [Tooltip("Позволяет отправлять аналитику в дебаг билдах")]
    public bool enableAnalitics;

    public bool googlePlayGamesServicesAreUsed;

    [TabGroup("Build")]
    [Button] void ImportDebugPackages() => settings.ManagePackages(false);
    [TabGroup("Build")]
    [Button] void ExcludeDebugPackages() => settings.ManagePackages(true);

    [TabGroup("Build")]
    public BuildSettings settings;

    private const int packagesCount = 0;
    [TabGroup("Build")]
    [ProgressBar(0, packagesCount)] [SerializeField] private int importedPackages;

#if UNITY_EDITOR
    private void OnEnable()
    {
        UpdateLoadedPackages();
        AssemblyReloadEvents.afterAssemblyReload += UpdateLoadedPackages;
        AssetDatabase.importPackageCompleted += ImportPackageCompleted;
        AssetDatabase.importPackageFailed += ImportPackageFailed;
    }


  
    private void OnDisable()
    {
        UpdateLoadedPackages();
        AssemblyReloadEvents.afterAssemblyReload -= UpdateLoadedPackages;
        AssetDatabase.importPackageCompleted -= ImportPackageCompleted;
        AssetDatabase.importPackageFailed -= ImportPackageFailed;
    }

    private void ImportPackageCompleted(string packageName) => UpdateLoadedPackages();
    private void ImportPackageFailed(string packageName, string errorMessage) => UpdateLoadedPackages();


    [TabGroup("Build")]
    [Button]
    private void UpdateLoadedPackages()
    {
        importedPackages = settings.DebugAssets.AssetsInPackages.Where(x => x.PackageImported).Count();

        if (packagesCount != settings.DebugAssets.AssetsInPackages.Count)
        {
            Debug.LogError("Update packages constant");
        }

        settings.DebugAssets.CachPackagesLinks();
    }
#endif
}