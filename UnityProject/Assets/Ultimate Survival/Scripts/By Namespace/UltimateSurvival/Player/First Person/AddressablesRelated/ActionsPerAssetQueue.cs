using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AddressablesRelated
{
    public class ActionsPerAssetQueue
    {
        public class ActionsPerAsset
        {

            private AssetReference assetReference;
            private List<Action> actions = new List<Action>();

            public AssetReference AssetRef => assetReference;

            public ActionsPerAsset(AssetReference assetReference, Action action)
            {
                this.assetReference = assetReference;
                actions.Add(action);
            }

            public void AddAction(Action action) => actions.Add(action);

            public void RemoveAction(Action action) => actions.Remove(action);

            public void ExecuteAllActionsAndRemoveThem()
            {
                foreach (var action in actions)
                {
                    action?.Invoke();
                }
                actions.Clear();
            }
        }

        private List<ActionsPerAsset> assetsInLoadingProcess = new List<ActionsPerAsset>();

        public void QueueActionOnAssetLoaded(AssetReference assetReference, Action action)
        {
            var al = assetsInLoadingProcess.Find(x => x.AssetRef == assetReference);
            if (al != null)
            {
                al.AddAction(action);
            }
            else
            {
                assetsInLoadingProcess.Add(new ActionsPerAsset(assetReference, action));
            }
        }

        public void ExecuteAllActionsForAssetAndClearQueue(AssetReference assetReference)
        {
            // Handle callbacks for this asset ref
            var loadedAssetRefAction = assetsInLoadingProcess.Find(x => x.AssetRef == assetReference);
            if (loadedAssetRefAction != null)
            {
                loadedAssetRefAction.ExecuteAllActionsAndRemoveThem();
                assetsInLoadingProcess.Remove(loadedAssetRefAction);
            }
            else
            {
                Debug.LogError("cant find actions for this asset ref: " + assetReference.ToString());
            }
        }

        // // Not used 
        // public void UnregisterLoadedCallback(AssetReference assetReference, Action action)
        // {
        //     var al = assetsInLoadingProcess.Find(x => x.AssetRef == assetReference);
        //     if (al != null)
        //     {
        //         al.RemoveAction(action);
        //     }
        //     else
        //     {
        //         Debug.LogError("Cant unregister action, no asset is loading");
        //     }
        // }

        public bool ActionsAreQueuedForAset(AssetReference assetReference)
        {
            var result = assetsInLoadingProcess.Find(x => x.AssetRef == assetReference) != null;
            return result;
        }
    }
}