using Core;
using Core.Storage;
using Extensions;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    [Serializable]
    public class WorldObjectSavableData
    {
        public WorldObjectID WorldObjectID;
        public List<uint> Ids;
        public List<EnvironmentSceneID> Environments;

        public WorldObjectSavableData()
        {
            Ids = new List<uint>();
            Environments = new List<EnvironmentSceneID>();
        }
    }

    #region New
    [Serializable]
    public class WorldObjectSavableDataNew
    {
        [SerializeField] private WorldObjectID _worldObjectID;
        [SerializeField] private EnvironmentSceneID _environment;
        public WorldObjectID WorldObjectID => _worldObjectID;
        public EnvironmentSceneID Environment => _environment;

        public WorldObjectSavableDataNew(WorldObjectID worldObjectID, EnvironmentSceneID environment)
        {
            _worldObjectID = worldObjectID;
            _environment = environment;
        }
    }

    [Serializable]
    public class WorldObjectsRuntimeData
    {
        [SerializeField] private UIntWorldObjectSavableDataNewDictinary _worldObjects;
        public UIntWorldObjectSavableDataNewDictinary WorldObjects {get => _worldObjects; private set => _worldObjects = value;}

        public void SetData(UIntWorldObjectSavableDataNewDictinary dic)
        {
            WorldObjects = dic;
        }
    }
        
    #endregion

    [Serializable]
    public class WorldObjectsSavableData : DataBase
    {
        public WorldObjectSavableData[] WorldObjects;
        public uint Index;

        public void SetWorldObjects(WorldObjectSavableData[] objects)
        {
            WorldObjects = objects;
            ChangeData();
        }
    }
    public class WorldObjectsModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private WorldObjectsSavableData _data;
        [SerializeField] private WorldObjectsSavableData _constructionData;

        [ReadOnly] [SerializeField] private WorldObjectsRuntimeData _regularObjectsData;
        
        [ReadOnly] [SerializeField] private WorldObjectsRuntimeData _constructionObjectsData;

#pragma warning restore 0649
        #endregion

        public WorldObjectsSavableData RegularSaveData => _data;
        public WorldObjectsSavableData ConstructionsSaveData => _constructionData;
        public WorldObjectsRuntimeData RegularRuntimeData => _regularObjectsData;
        public WorldObjectsRuntimeData ConstructionsRuntimeData => _constructionObjectsData;

        public uint Index
        {
            get => _data.Index;
            set
            {
                _data.Index = value;
                _data.ChangeData();
            }
        }

        public uint ConstructionIndex
        {
            get => _constructionData.Index;
            set
            {
                _constructionData.Index = value;
                _constructionData.ChangeData();
            }
        }
        
        public Dictionary<WorldObjectID, List<WorldObjectModel>> SaveableObjectModels { get; } = new Dictionary<WorldObjectID, List<WorldObjectModel>>();
        
        public UIntWorldObjectSavableDataNewDictinary NotCreatedSavableWordlObjectIds { get; set; } = new UIntWorldObjectSavableDataNewDictinary();
        public UIntWorldObjectSavableDataNewDictinary NotCreatedSavableConstructionWordlObjectIds { get; set; } = new UIntWorldObjectSavableDataNewDictinary();

        private UniqueAction<WorldObjectID, WorldObjectModel> OnAddAction { get; } = new UniqueAction<WorldObjectID, WorldObjectModel>();
        private UniqueAction<WorldObjectID, WorldObjectModel> OnRemoveAction { get; } = new UniqueAction<WorldObjectID, WorldObjectModel>();

        public IUniqueEvent<WorldObjectID, WorldObjectModel> OnAdd => OnAddAction;
        public IUniqueEvent<WorldObjectID, WorldObjectModel> OnRemove => OnRemoveAction;

        public event Action<WorldObjectModel> OnSpawned;
        public event Action<WorldObjectModel> OnRemoveSpawned;

        public event Action __OnConstructionInitialized;
        public bool __IsConstructionInitialized { get; private set; }

        public Dictionary<WorldObjectID, List<WorldObjectModel>> CreatedWorldObjectsModels { get; } = new Dictionary<WorldObjectID, List<WorldObjectModel>>();

        public EnvironmentSceneID InitializedEnvironment {get; set;}

        public void AddSpawned(WorldObjectModel model)
        {
            if (model == null)
            {
                throw new Exception();
            }

            if (CreatedWorldObjectsModels.ContainsKey(model.WorldObjectID))
            {
                CreatedWorldObjectsModels[model.WorldObjectID].Add(model);
            }
            else
            {
                CreatedWorldObjectsModels[model.WorldObjectID] = new List<WorldObjectModel>() { model };
            }

            OnSpawned?.Invoke(model);
        }

        public void RemoveSpawnable(WorldObjectModel model)
        {
            if(CreatedWorldObjectsModels.TryGetValue(model.WorldObjectID, out List<WorldObjectModel> models))
            {
                models.Remove(model);
                OnRemoveSpawned?.Invoke(model);
            }
        }

        public void AddSavable(WorldObjectModel model)
        {
            if (model == null)
            {
                throw new Exception();
            }

            if (SaveableObjectModels.ContainsKey(model.WorldObjectID))
            {
                SaveableObjectModels[model.WorldObjectID].Add(model);
            }
            else
            {
                SaveableObjectModels[model.WorldObjectID] = new List<WorldObjectModel>() { model };
            }

            OnAddAction.Invoke(model.WorldObjectID, model);
        }

        public void RemoveSavable(WorldObjectModel model)  
        {
            if(SaveableObjectModels.TryGetValue(model.WorldObjectID, out List<WorldObjectModel> models))
            {
                models.Remove(model);
                OnRemoveAction.Invoke(model.WorldObjectID, model);
            }
        }

        public uint GetGenerateId() => (++Index);
        public uint GetGenerateIdConstruction() => (++ConstructionIndex);

        public void UpdateRuntimeData()
        {
            UpdateRuntimeData(false);
            UpdateRuntimeData(true);
        }

        private void UpdateRuntimeData(bool constructions)
        {
            var activeEnvironment = InitializedEnvironment;

            var createdObjectsDict = new UIntWorldObjectSavableDataNewDictinary();
            foreach (var keyValuePair in SaveableObjectModels)
            {
                if(IsConstructionItem(keyValuePair.Key) != constructions) continue;

                foreach (var model in keyValuePair.Value)
                {
                    createdObjectsDict.Add(model.ID,new WorldObjectSavableDataNew(keyValuePair.Key, activeEnvironment));
                }
            }

            var notCreatedObjects = constructions ? NotCreatedSavableConstructionWordlObjectIds : NotCreatedSavableWordlObjectIds;

            var newSave = createdObjectsDict.MergeLeft(notCreatedObjects);

            if(constructions)
                ConstructionsRuntimeData.SetData(newSave);
            else
                RegularRuntimeData.SetData(newSave);
        }

        public bool IsConstructionItem(WorldObjectID worldObjectID) => worldObjectID.ToString().Contains("construction");

        public void __InitializeConstruction()
        {
            __IsConstructionInitialized = true;
            __OnConstructionInitialized?.Invoke();
        }
    }
}
