using System;
using System.Collections.Generic;
using Core.Storage;
using UnityEngine;

namespace Game.Models
{
    public class DebugPrefabCreationModel : MonoBehaviour
    {
        [SerializeField] private Data _data;
        public Data _Data => _data;

        public List<DebugAssetConfig> Configs => _data.configs;
        
        public void SetConfig(DebugAssetConfig config) => _data.SetConfig(config);

        [Serializable]
        public class Data: DataBase
        {
            public override SaveTime TimeSave => SaveTime.Instantly;
            public List<DebugAssetConfig> configs = new List<DebugAssetConfig>();

            public void SetConfig(DebugAssetConfig config)
            {
                var match = configs.Find(x => x.debugAssetsName == config.debugAssetsName);
                if(match == null)
                {
                    Debug.LogError("Cant find match in configs");
                    return;
                }

                match.spawnOnStart = config.spawnOnStart;
                ChangeData();
            }
        }

        [Serializable]
        public class DebugAssetConfig
        {
            public DebugAssetConfig(string debugAssetsName, bool spawnOnStart)
            {
                this.debugAssetsName = debugAssetsName;
                this.spawnOnStart = spawnOnStart;
            }

            public string debugAssetsName;
            public bool spawnOnStart;
        }
    }
}
