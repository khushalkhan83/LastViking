using UnityEngine;
using UnityEditor;
using System;
using Extensions;

namespace CustomeEditorTools.EditorGameSettingsData
{
    [System.Serializable]
    public class BuildSettings
    {
        [SerializeField] private string debugAssetsPath = "Assets/ScriptableObjects/DebugAssets/SO_debugAssets_default.asset";
        [SerializeField] private DebugAssets _debugAssets;
        public DebugAssets DebugAssets
        {
            get
            {
                if (_debugAssets == null)
                {
#if UNITY_EDITOR
                    _debugAssets = AssetDatabase.LoadAssetAtPath(debugAssetsPath, typeof(DebugAssets)) as DebugAssets;
#endif
                }
                return _debugAssets;
            }
        }

        public void RemoveDebugReferences()
        {
            _debugAssets = null;
            Debug.Log("debug assets removed");
        }

        public void AddReferencesForDebugBuild()
        {
            DebugAssets.Log("Loaded"); // will laod Resources by calling DebugAssets property
        }

        private const string ResourcesFolderName = "Resources";
        private const string ExcludedEnding = "_Excluded_From_Build";
        private const string EcludeScriptsFolder = "WebPlayerTemplates";
        private const string EcludeScriptsFolderPath = "Assets/" + EcludeScriptsFolder;

        public void ManageDevFolders(bool excludeDevFolders)
        {
#if UNITY_EDITOR
            var folders = DebugAssets.ResourceFolderToExcludeFromBuild;

            if (folders.Count == 0)
            {
                Debug.Log("No ResourceFolderToExcludeFromBuild to manage");
                return;
            }


            // check names
            foreach (var folder in folders)
            {
                if (!folder.name.Contains(ResourcesFolderName))
                {
                    throw new Exception("Wrong folders included in debug settings: " + folder.name + " at " + folder.Path());
                }
            }
            Debug.Log("Resources folders name ok");

            // rename them to exlude from build
            foreach (var folder in folders)
            {
                var oldPath = folder.Path();

                int lastSlashIndex = oldPath.LastIndexOf('/');
                string parrentFolderPath = oldPath.Substring(0, lastSlashIndex);

                var newPath = parrentFolderPath + "/" + (excludeDevFolders ? ResourcesFolderName + ExcludedEnding : ResourcesFolderName);
                Debug.Log("oldPath: " + oldPath);
                Debug.Log("newPath: " + newPath);
                AssetDatabase.MoveAsset(oldPath, newPath);
            }
            AssetDatabase.Refresh();
            if (excludeDevFolders)
                Debug.Log("Resources excluded from build");
            else
                Debug.Log("Resources included in build");
#endif
        }

        
        /// Can import, delete one package at once only (not cycling through them not working as expected)
        public void ManagePackages(bool exclude)
        {
#if UNITY_EDITOR
            var assets = DebugAssets.Assets;
            if (assets.Count == 0)
            {
                Debug.Log("No DebugAsset to manage");
                return;
            }

            foreach (var debugAsset in assets)
            {
                if (exclude)
                    if (debugAsset.PackageImported)
                        debugAsset.DeletePackage();
                    else
                        Debug.Log(debugAsset.PackageName + " already removed");
                else
                {
                    if (debugAsset.PackageImported)
                        Debug.Log(debugAsset.PackageName + " already imported");
                    else
                        debugAsset.ImportPackage();
                }
            }
            (DebugAssets.Assets.Count + " packages managed").Log();
#endif
        }
    }
}