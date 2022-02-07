using ChunkLoaders;
using Encounters;
using Game.Models;
using Game.Providers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class MainIslandInjectionMapper : SceneInjectionMapper
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private SkeletonSpawnManager _skeletonSpawnManager;
        [SerializeField] private TreasureHuntPlacesProvider _treasureHuntPlacesProvider;
        [SerializeField] private SpecialEncounterSpawnPointProvider _specialEncounterSpawnPointProvider;
        ///CODE_GENERATION_FIELDS

#pragma warning restore 0649
        #endregion

        protected override void InitLinks()
        {
            base.InitLinks();

            Links.Add( typeof(SkeletonSpawnManager), _skeletonSpawnManager);
            Links.Add( typeof(TreasureHuntPlacesProvider), _treasureHuntPlacesProvider);
            Links.Add( typeof(SpecialEncounterSpawnPointProvider), _specialEncounterSpawnPointProvider);
        }
    }
}
