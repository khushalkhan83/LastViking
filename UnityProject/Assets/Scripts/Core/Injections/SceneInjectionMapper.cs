using ChunkLoaders;
using Game.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class SceneInjectionMapper : SceneInjectionMapperBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private PlayerRespawnPoints _playerRespawnPoints;
        [SerializeField] private ChunkLoadersConfigProvider _chunkLoadersConfigProvider;
        [SerializeField] private SceneTransitionsModel _sceneTransitionsModel;
        ///CODE_GENERATION_FIELDS

#pragma warning restore 0649
        #endregion

        protected override void InitLinks()
        {
            Links = new Dictionary<Type, object>()
            {
                { typeof(PlayerRespawnPoints), _playerRespawnPoints},
                { typeof(ChunkLoadersConfigProvider), _chunkLoadersConfigProvider},
                { typeof(SceneTransitionsModel), _sceneTransitionsModel},
                ///CODE_GENERATION_LINKS
            };    
        }
    }
}
