using ChunkLoaders;
using Core;
using Game.Controllers;
using Game.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Interactables
{
    public class SetChunkLoaderConfigActivatable : Activatable
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private ChunkLoadersConfigProvider _configProvider;

        #endregion

        private Dictionary<Type, object> Links { get; set; }

        private void Awake()
        {
            Links = new Dictionary<Type, object>()
            {
                { typeof(ChunkLoadersConfigProvider), _configProvider},
            };
        }

        private ChunksLoadersModel ChunksLoadersModel => ModelsSystem.Instance._chunksLoadersModel;
        private InjectionSystem InjectionSystem => ModelsSystem.Instance._injectionSystem;

        public override void OnActivate()
        {
            InjectionSystem.UpdateLinks(Links);
            ChunksLoadersModel.ChangeChunkLoaderConfig();
        }
    }
}
