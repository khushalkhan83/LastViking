using ChunkLoaders;
using Game.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public abstract class SceneInjectionMapperBase : InjectionMapperBase
    {
        private InjectionModel InjectionModel => ModelsSystem.Instance._injectionModel;
        protected override void UpdatedLinks()
        {
            InjectionModel.SetSceneDependenciesInjected();
        }
    }
}
