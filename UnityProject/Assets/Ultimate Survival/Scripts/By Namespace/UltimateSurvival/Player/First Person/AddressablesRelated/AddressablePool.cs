using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AddressablesRelated
{
    public class AddressablePool
    {
        #region Data
        public class Data
        {
            public AsyncOperationHandle<GameObject> assetInMemory;
            private List<GameObject> instances = new List<GameObject>();

            public Data(AsyncOperationHandle<GameObject> assetInMemory)
            {
                this.assetInMemory = assetInMemory;
            }

            public void AddInstance(GameObject instance)
            {
                instances.Add(instance);
            }

            public void RemoveInstance(GameObject instance)
            {
                instances.Remove(instance);
            }

            public bool HasInstance(GameObject instance) => instances.Contains(instance);

            public bool NoInstances() => instances.Count == 0;

            public void Refresh()
            {
                var notUsedInstances = new List<GameObject>();
                foreach (var inst in instances)
                {
                    if(inst == null) notUsedInstances.Add(inst);
                }

                instances = instances.Except(notUsedInstances).ToList();
            }
        }

        private Dictionary<AssetReference, Data> loadedAssetByAssetRef = new Dictionary<AssetReference, Data>();

        private ActionsPerAssetQueue actionsQueue = new ActionsPerAssetQueue();
        //TODO: test edge cases
        private List<AssetReference> assetQueueToRelease = new List<AssetReference>();

        #endregion

        #region public methods
        public void Refresh()
        {
            loadedAssetByAssetRef.ToList().ForEach(x => x.Value.Refresh());
        }   
        public void LoadAsset(AssetReference assetReference, Action OnAssetLoaded = null)
        {
            if(loadedAssetByAssetRef.ContainsKey(assetReference))
            {
                OnAssetLoaded?.Invoke();
                return;
            }

            bool assetAlreadyLoading = actionsQueue.ActionsAreQueuedForAset(assetReference);

            // no need to call Addressables.LoadAssetAsync if it is already called
            if(!assetAlreadyLoading)
            {
                Addressables.LoadAssetAsync<GameObject>(assetReference.RuntimeKey).Completed += (data) => HandleAssetLoaded(data, assetReference);
            }

            actionsQueue.QueueActionOnAssetLoaded(assetReference,OnAssetLoaded);
        }

        private void HandleAssetLoaded(AsyncOperationHandle<GameObject> asset, AssetReference assetReference)
        {
            loadedAssetByAssetRef.Add(assetReference, new Data(asset));

            actionsQueue.ExecuteAllActionsForAssetAndClearQueue(assetReference);
        }

        public void UnloadAsset(AssetReference assetReference)
        {
            if (loadedAssetByAssetRef.TryGetValue(assetReference, out var asset))
            {
                if(asset.NoInstances())
                {
                    loadedAssetByAssetRef.Remove(assetReference);
                    Addressables.Release(asset.assetInMemory);
                }
                else
                {
                    assetQueueToRelease.Add(assetReference);
                }
            }
        }

        public void ReleaseInstance(GameObject instance)
        {
            bool hasInstance = false;
            Data targetData = null;
            AssetReference reference = null;

            foreach (var dataByRef in loadedAssetByAssetRef)
            {
                if (dataByRef.Value.HasInstance(instance))
                {
                    hasInstance = true;
                    targetData = dataByRef.Value;
                    reference = dataByRef.Key;
                    break;
                }
            }

            if (hasInstance == false)
            {
                Debug.LogError("Cant remove instance. Its not in pool");
                return;
            }

            targetData.RemoveInstance(instance);
            Addressables.ReleaseInstance(instance);

            ReleaseAssetIfNeeded(reference,targetData);
        }

        public bool ContainsInstance(GameObject instance) => loadedAssetByAssetRef.Values.ToList().Find(x => x.HasInstance(instance)) != null;
        /// Will Load asset if it wasn`t loaded and instantiate it.
        public void InstantiateAsync(AssetReference assetReference, Transform parent = null, Action<GameObject> OnAssetInstanced = null)
        {
            bool assetLoaded = loadedAssetByAssetRef.TryGetValue(assetReference, out var assetData);

            if (assetLoaded)
            {
                Addressables.InstantiateAsync(assetReference, parent).Completed += (instanceInMemory) => UsedLoadedAssetAndInstantced(instanceInMemory, assetData, assetReference, OnAssetInstanced);
            }
            else
            {
                LoadAsset(assetReference, () =>
                {
                    bool error = !loadedAssetByAssetRef.TryGetValue(assetReference, out assetData);

                    if (error) return;
                    Addressables.InstantiateAsync(assetReference, parent).Completed += (instanceInMemory) => LoadedAssetAndInstanced(instanceInMemory, assetData, assetReference, OnAssetInstanced);
                });
            }
        }

        #endregion
        
        #region private methods
        private void UsedLoadedAssetAndInstantced(AsyncOperationHandle<GameObject> assetInMemory, Data assetData, AssetReference assetReference, Action<GameObject> onAssetInstanced)
        {
            var instance = assetInMemory.Result;
            assetData.AddInstance(instance);

            onAssetInstanced?.Invoke(instance);
        }

        private void LoadedAssetAndInstanced(AsyncOperationHandle<GameObject> instanceInMemory, Data assetData, AssetReference assetReference, Action<GameObject> onAssetInstanced)
        {
            var instance = instanceInMemory.Result;
            assetData.AddInstance(instance);

            onAssetInstanced?.Invoke(instance);
        }

        private void ReleaseAssetIfNeeded(AssetReference assetReference, Data data)
        {
            bool release = assetQueueToRelease.Contains(assetReference);
            if(!release) return;

            if (data.NoInstances()) 
            {
                assetQueueToRelease.Remove(assetReference);
                UnloadAsset(assetReference);
            }
        }

        #endregion
    }
}