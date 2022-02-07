using Core.Storage;
using Game.Models;
using Game.Providers;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectCreator : MonoBehaviour
{
    #region Data
#pragma warning disable 0649

    [SerializeField] private WorldObjectsModel _worldObjectsModel;
    [SerializeField] private WorldObjectsProvider _worldObjectsProvider;

    private Transform _transform;
#pragma warning restore 0649
    #endregion
    public Transform Transform 
    {
        get
        {
            if(_transform == null) _transform = transform;
            return _transform;
        }
        private set => _transform = value;
    }

    private WorldObjectsModel WorldObjectsModel => _worldObjectsModel;
    private WorldObjectsProvider WorldObjectsProvider => _worldObjectsProvider;

    public void SetDefaultObjectsRoot(Transform t)
    {
        Transform = t;
    }

    public WorldObjectModel Create(WorldObjectID worldObjectID, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        var @ref = WorldObjectsProvider[worldObjectID];
        return Create(@ref, position, rotation, @ref.transform.localScale, parent);
    }

    private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;

    public WorldObjectModel CreateAsSpawnable(WorldObjectID worldObjectID, Vector3 position, Quaternion rotation, Vector3 localScale, DataProcessing dataProcessing, Transform parent = null)
    {
        var instance = Instantiate(WorldObjectsProvider[worldObjectID]);
        instance.transform.position = position;
        instance.transform.rotation= rotation;
        instance.transform.localScale = localScale;
        instance.transform.parent = parent ?? Transform;
        instance.SetCreationType(WorldObjectCreationType.Spawner);
        instance.EnvironmentSceneID = PlayerScenesModel.ActiveEnvironmentSceneID;

        Initialization(instance, dataProcessing);
        WorldObjectsModel.AddSpawned(instance);

        return instance;
    }

    public WorldObjectModel Create(WorldObjectModel @ref, Vector3 position, Quaternion rotation, Vector3 localScale, Transform parent = null)
    {
        var instance = Instantiate(@ref, position, rotation, parent ?? Transform);
        instance.transform.localScale = localScale;
        instance.SetCreationType(WorldObjectCreationType.World);
        instance.EnvironmentSceneID = PlayerScenesModel.ActiveEnvironmentSceneID;

        Initialization(instance);

        WorldObjectsModel.AddSavable(instance);
        return instance;
    }

    public WorldObjectModel Create(uint id, WorldObjectModel @ref, Transform parent)
    {
        var instance = Instantiate(@ref, parent);
        instance.SetCreationType(WorldObjectCreationType.World);
        instance.EnvironmentSceneID = PlayerScenesModel.ActiveEnvironmentSceneID;

        Initialization(instance, id);
        WorldObjectsModel.AddSavable(instance);

        return instance;
    }

    private void Initialization(WorldObjectModel instance)
    {
        var id = WorldObjectsModel.IsConstructionItem(instance.WorldObjectID)? 
            WorldObjectsModel.GetGenerateIdConstruction() : WorldObjectsModel.GetGenerateId();
        Initialization(instance, id);
    }

    private void Initialization(WorldObjectModel instance, uint id)
    {
        instance.ID = id;
        var inits = instance.GetComponentsInChildren<IData>(true);

        foreach (var init in inits)
        {
            DataProcessing(init.Uniques, id);
            init.UUIDInitialize();
        }
    }

    private void Initialization(WorldObjectModel instance, DataProcessing dataProcessing)
    {
        var inits = instance.GetComponentsInChildren<IData>(true);

        foreach (var init in inits)
        {
            dataProcessing(init.Uniques);
            init.UUIDInitialize();
        }
    }

    private void DataProcessing(IEnumerable<IUnique> uniques, uint id)
    {
        foreach (var item in uniques)
        {
            item.UUID = id + '.' + item.UUID;
            item.UUIDPrefix = id;
        }
    }
}
