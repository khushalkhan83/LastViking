using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Extensions;
using NaughtyAttributes;
using System.Linq;

[CreateAssetMenu(fileName = "SO_debugAssets_default", menuName = "Debug/DebugAssets", order = 0)]
public class DebugAssets : ScriptableObject
{
    [SerializeField] private List<DebugAsset> _debugAssets;

    [SerializeField] private List<Object> _resourceFolderToIgnore;

    public GameObject DevChest => GetPrefab("devChest");
    public GameObject ProjectView => GetPrefab("projectView");

    public List<DebugAsset> Assets => _debugAssets;
    public List<Object> ResourceFolderToExcludeFromBuild => _resourceFolderToIgnore;
    public List<DebugAsset> AssetsInPackages => _debugAssets.Where(x => x.IsLocataedInPackage).ToList();


    public GameObject GetPrefab(string debugAssetName)
    {
        var target = _debugAssets.Find(x => x.debugAssetName == debugAssetName);
        if(target == null) throw new Exception("debug asset not exist");
        return target.Prefab;
    }

    // TODO: implement this logic inside import package
    [Button]
    public void CachPackagesLinks()
    {
        #if UNITY_EDITOR
        foreach (var asset in Assets)
        {
            if(!asset.IsLocataedInPackage) continue;

            if(asset.PackageImported)
            {
                Debug.Log("prefab " + asset.Prefab.ToString() + " from " + asset.PackageName +  " cached");
            }
        }
        #endif
    }

    [System.Serializable]
    public class DebugAsset
    {
        public string debugAssetName;
        [SerializeField] private GameObject prefab;
        [SerializeField] private  Settings settings;

        public bool IsLocataedInPackage => settings.isLocatedInPackage;

        public bool PackageExist => settings.packageAsset != null;

        #if UNITY_EDITOR
        public bool PackageImported
        {
            get
            {
                var loadedPrefab = GetPrefabByPath();
                return loadedPrefab != null;
            }
        }
        #endif
        public GameObject Prefab
        {
            get
            {   
                if(settings.isLocatedInPackage)
                {
                    if(prefab == null)
                    #if UNITY_EDITOR
                        prefab = GetPrefabByPath();
                    #endif
                    if(prefab == null) throw new Exception("Cant load debug asset from package. " + " PackageExist: " + PackageExist);
                    
                    return prefab;
                }
                else
                {
                    if(prefab == null) throw new Exception("Debug asset is not assigned");
                    return prefab;
                }
            }
        }

        public string PackageName => settings.packageName;
        public void ImportPackage()
        {
            #if UNITY_EDITOR
            if(!settings.isLocatedInPackage)
                throw new Exception("Cant load package for asset" + debugAssetName +  ". Its marked as not package located");
            
            var package = settings.packageAsset;
            AssetDatabase.ImportPackage(package.Path(),true);
            AssetDatabase.Refresh();

            #endif
        }

        // used to remove folders from game in release build
        public void DeletePackage()
        {
            #if UNITY_EDITOR
            if(!settings.isLocatedInPackage)
                throw new Exception("Cant unload package for asset" + debugAssetName +  ". Its marked as not package located");

            bool result = AssetDatabase.DeleteAsset(settings.packageRootFolder);
            if(result == true)
                "Removed packaged from project".Log();
            else
                throw new Exception("Cant remove package from project");
            #endif
        }

        #if UNITY_EDITOR
        private GameObject GetPrefabByPath() => AssetDatabase.LoadAssetAtPath(settings.prefabPath, typeof(GameObject)) as GameObject;
        #endif

        [System.Serializable]
        public class Settings
        {
            public string packageName;
            public bool isLocatedInPackage = false;
            public string prefabPath = string.Empty;
            public UnityEngine.Object packageAsset;
            public string packageRootFolder;
        }
    }
}
