using Core;
using Core.Controllers;
using Game.Models;
using ChunkLoaders;

namespace Game.Controllers
{
    public class ChunksLoadersController : IChunksLoadersController, IController
    {
        [Inject(true)] public ChunkLoadersConfigProvider ChunkLoadersConfigProvider {get; private set;}
        //[Inject] public ChunkLoadersProvider ChunkLoadersProvider {get; private set;}
        [Inject] public PlayerScenesModel PlayerScenesModel {get; private set;}
        [Inject] public UnloadUnusedAssetsModel UnloadUnusedAssetsModel {get; private set;}
        [Inject] public ChunksLoadersModel ChunksLoadersModel {get; private set;}

        void IController.Start() {}
        void IController.Enable() 
        {
            UnloadUnusedAssetsModel.OnUnusedSceneAssetsUnloaded += SetChunkLoaderSize;
            ChunksLoadersModel.OnChunkLoadersConfigChanged += SetChunkLoaderSize;
        }

        void IController.Disable() 
        {
            UnloadUnusedAssetsModel.OnUnusedSceneAssetsUnloaded -= SetChunkLoaderSize;
            ChunksLoadersModel.OnChunkLoadersConfigChanged -= SetChunkLoaderSize;
        }

        private void SetChunkLoaderSize()
        {
            // var loader = ChunkLoadersProvider.RegionLoader;
            // var envID = PlayerScenesModel.ActiveEnvironmentSceneID;
            // var configVariable = ChunkLoadersConfigProvider[envID];

            // loader.LoadSize = configVariable.LoadSize;
        }
    }
}
