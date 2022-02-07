using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Controllers
{
    public class WorldObjectsController : IWorldObjectsController, IController
    {
        [Inject] public WorldObjectCreator WorldObjectCreator {get; private set;}
        [Inject] public WorldObjectsModel WorldObjectsModel {get; private set;}
        [Inject] public WorldObjectsProvider WorldObjectsProvider {get; private set;}
        [Inject] public StorageModel StorageModel {get; private set;}
        [Inject] public PlayerScenesModel PlayerScenesModel {get; private set;}
        [Inject] public SceneNamesProvider SceneNamesProvider {get; private set;}

        private bool initialization = true;

        void IController.Enable() 
        {
            StorageModel.TryProcessing(WorldObjectsModel.RegularSaveData);
            StorageModel.TryProcessing(WorldObjectsModel.ConstructionsSaveData);

            UnPackRegularObjects();
            UnPackConstructionObjects();

            PlayerScenesModel.OnPreEnvironmentLoaded += OnPreEnvironmentLoaded;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            StorageModel.OnPreSaveChanged -= OnPreSaveHandler;
            PlayerScenesModel.OnPreEnvironmentLoaded -= OnPreEnvironmentLoaded;
        }
 
        private void OnPreEnvironmentLoaded()
        {
            StorageModel.OnPreSaveChanged -= OnPreSaveHandler;
            StorageModel.OnPreSaveChanged += OnPreSaveHandler;

            Initialization();
        }

        private void OnPreSaveHandler()
        {
            if(initialization) return;

            WorldObjectsModel.UpdateRuntimeData();
            PackRegularObjects();
            PackConstructionObjects();
        }

        private void Initialization()
        {
            initialization = true;

            WorldObjectsModel.InitializedEnvironment = PlayerScenesModel.ActiveEnvironmentSceneID;
            WorldObjectsModel.SaveableObjectModels.Clear();

            var envSceneName = SceneNamesProvider[PlayerScenesModel.ActiveEnvironmentSceneID];
            var scene = SceneManager.GetSceneByName(envSceneName);
            Transform holder = CreateWorldObjectsHolder(scene);

            WorldObjectCreator.SetDefaultObjectsRoot(holder);

            InitilaizeFromData(WorldObjectsModel.RegularRuntimeData.WorldObjects, WorldObjectsModel.NotCreatedSavableWordlObjectIds, "Error with initialization of worldobjects: ", holder);
            InitilaizeFromData(WorldObjectsModel.ConstructionsRuntimeData.WorldObjects, WorldObjectsModel.NotCreatedSavableConstructionWordlObjectIds, "Error with initialization of worldobjects(construction): ", holder);

            initialization = false;
            WorldObjectsModel.__InitializeConstruction();
        }

        private Transform CreateWorldObjectsHolder(Scene scene)
        {
            var holder = new GameObject("WorldObjects");
            SceneManager.MoveGameObjectToScene(holder, scene);
            holder.transform.SetAsFirstSibling();
            return holder.transform;
        }

        private void InitilaizeFromData(UIntWorldObjectSavableDataNewDictinary objectsFromSave, UIntWorldObjectSavableDataNewDictinary notCreatedObjects, string errorMessage, Transform holder)
        {   
            WorldObjectModel @ref;

            notCreatedObjects.Clear();

            foreach (var item in objectsFromSave)
            {
                var worldObjectId = item.Value.WorldObjectID;
                var envScene = item.Value.Environment;
                var id = item.Key;

                @ref = WorldObjectsProvider[worldObjectId];
                
                if(envScene == WorldObjectsModel.InitializedEnvironment)
                {
                    try
                    {
                        WorldObjectCreator.Create(id, @ref, holder);
                    }
                    catch (System.Exception ex)
                    {
                        string extraData = id + (@ref == null ? "null @ref" : @ref.ID.ToString());
                        (errorMessage + extraData).Error();
                        ex.Message.Error();
                        ex.StackTrace.Error();
                        RegisterObjectAsNotCreated();
                    }
                }
                else
                {
                    RegisterObjectAsNotCreated();
                }

                void RegisterObjectAsNotCreated() => notCreatedObjects.Add(id,new WorldObjectSavableDataNew(worldObjectId,envScene));
            }
        }

        private void UnPackRegularObjects() => UnPackSaveData(WorldObjectsModel.RegularSaveData,WorldObjectsModel.RegularRuntimeData);
        private void UnPackConstructionObjects() => UnPackSaveData(WorldObjectsModel.ConstructionsSaveData,WorldObjectsModel.ConstructionsRuntimeData);

        private void PackRegularObjects() => PackSaveData(WorldObjectsModel.RegularRuntimeData,WorldObjectsModel.RegularSaveData);
        private void PackConstructionObjects() => PackSaveData(WorldObjectsModel.ConstructionsRuntimeData,WorldObjectsModel.ConstructionsSaveData);

        private void UnPackSaveData(WorldObjectsSavableData saveData, WorldObjectsRuntimeData runtimeData)
        {
            runtimeData.WorldObjects.Clear();
            
            foreach (var d in saveData.WorldObjects)
            {
                for (int i = 0; i < d.Ids.Count; i++)
                {
                    if(i >= d.Environments.Count)
                    {
                        for(int j = d.Environments.Count; j <= i; j++)
                        {
                            d.Environments.Add(PlayerScenesModel.DefaultSceneID);
                        }
                    }

                    var id = d.Ids[i];
                    var Environment = d.Environments[i];
                    var temp = new WorldObjectSavableDataNew(d.WorldObjectID, Environment);
                    runtimeData.WorldObjects.Add(id,temp);
                }
            }
        }

        private void PackSaveData(WorldObjectsRuntimeData runtimeData, WorldObjectsSavableData saveData)
        {
            List<WorldObjectSavableData> packedDatas = new List<WorldObjectSavableData>();

            foreach (var d in runtimeData.WorldObjects)
            {
                uint id = d.Key;
                var env = d.Value.Environment;
                var worldObjectId = d.Value.WorldObjectID;

                var match = packedDatas.Find(x => x.WorldObjectID == worldObjectId);
                if(match == null)
                {
                    var temp = new WorldObjectSavableData();

                    temp.WorldObjectID = worldObjectId;
                    temp.Ids.Add(id);
                    temp.Environments.Add(env);
                    
                    packedDatas.Add(temp);
                }
                else
                {
                    match.Ids.Add(id);
                    match.Environments.Add(env);
                }
            }

            saveData.WorldObjects = packedDatas.ToArray();
            saveData.ChangeData();
        }
    }
}