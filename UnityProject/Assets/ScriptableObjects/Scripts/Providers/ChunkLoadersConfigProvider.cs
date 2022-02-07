using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Providers;
using Game.Models;

namespace ChunkLoaders
{

    [CreateAssetMenu(fileName = "SO_Providers_configs_chunLoaders_default", menuName = "Providers/ChunkLoaders", order = 0)]
    public class ChunkLoadersConfigProvider : ProviderScriptable<EnvironmentSceneID, ChunkLoadersConfig>
    {
        [EnumNamedArray(typeof(EnvironmentSceneID))] 
        [SerializeField] private ChunkLoadersConfig[] _data;
        [SerializeField] private ChunkLoadersConfig _defaultConfig;
        public override ChunkLoadersConfig this[EnvironmentSceneID key]
        {
            get
            {
                try
                {
                    return _data[((int)(object)key - 1)];
                }
                catch (System.Exception)
                {
                    Debug.Log($"{key.ToString()} not found. Default key value is used");
                    return _defaultConfig;
                }
            }
        }
    }
}

