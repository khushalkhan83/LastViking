using Game.Models;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Game.Controllers
{
    public class WorldObjectController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private WorldObjectModel _worldObjectModel;

#pragma warning restore 0649
        #endregion

        private WorldObjectModel WorldObjectModel => _worldObjectModel;
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private WorldObjectsModel WorldObjectsModel => ModelsSystem.Instance._worldObjectsModel;
        private AnaliticsModel AnaliticsModel => ModelsSystem.Instance._analiticsModel;

        #region MonoBehaviour
        private void OnEnable()
        {
            StorageModel.OnPreSaveAll += OnPreSaveHandler;
            WorldObjectModel.OnDataInitialize += OnUUIDInitializeHandler;
            WorldObjectModel.OnDelete += OnDeleteHandler;
        }

        private void OnDisable()
        {
            StorageModel.OnPreSaveAll -= OnPreSaveHandler;
            WorldObjectModel.OnDataInitialize -= OnUUIDInitializeHandler;
            WorldObjectModel.OnDelete -= OnDeleteHandler;
        }
        #endregion

        private void OnUUIDInitializeHandler()
        {
            WorldObjectModel.OnDataInitialize -= OnUUIDInitializeHandler;

            switch (WorldObjectModel.WorldObjectCreationType)
            {
                case WorldObjectCreationType.World:
                    HandleUUIDInitialize_WorldCreationType();
                    break;
                case WorldObjectCreationType.Spawner:
                    HandleUUIDInitialize_SpawnerCreationType();
                    break;
                default:
                    HandleUUIDInitialize_NoneCreationType();
                    break;
            }
        }

        private void HandleUUIDInitialize_WorldCreationType()
        {
            if (StorageModel.TryProcessing(WorldObjectModel.Data))
            {
                transform.position = WorldObjectModel.Position;
                transform.rotation = WorldObjectModel.Rotation;
                transform.localScale = WorldObjectModel.Scale;
            }
            else
            {
                OnPreSaveHandler();
            }
            if (CheckPositionZero(transform.position))
            {
                AnaliticsModel.SendSpawnObjectInZero(WorldObjectModel.WorldObjectID);
                LogExeption();
            }
        }

        private void LogExeption()
        {
            Debug.LogException(new System.Exception($"Null coorditane spawn {WorldObjectModel.WorldObjectID}"));

            try
            {
                if (!EditorGameSettings.Instance.ShowNullCoordinatesSpawnPopup) return;
#if UNITY_EDITOR
                bool ok = EditorUtility.DisplayDialog("Внимание",
                "Произошел спавн в null координатах",
                "ОК");
#endif
            }
            catch (System.Exception)
            {

            }
        }

        private void HandleUUIDInitialize_SpawnerCreationType()
        {
            
        }

        private void HandleUUIDInitialize_NoneCreationType()
        {
            Debug.LogError("None Spawn System");
        }

        private bool CheckPositionZero(Vector3 pos) => (int)pos.x == 0 && (int)pos.y == 0 && (int)pos.z == 0;

        private void OnPreSaveHandler()
        {
            if(WorldObjectModel.WorldObjectCreationType != WorldObjectCreationType.World) return;

            WorldObjectModel.SetPosition(transform.position);
            WorldObjectModel.SetRotation(transform.rotation);
            WorldObjectModel.SetScale(transform.localScale);
        }

        private void OnDeleteHandler()
        {
            switch (WorldObjectModel.WorldObjectCreationType)
            {
                case WorldObjectCreationType.World:
                    DeleteWorldObjectSpawnSystem();
                    break;
                case WorldObjectCreationType.Spawner:
                    DeleteSpawnableWorldObjectSpawnSystem();
                    break;
                default:
                    DeleteNoneSystem();
                    break;
            }
        }

        private void DeleteNoneSystem()
        {
            ClearData();
            WorldObjectsModel.RemoveSpawnable(WorldObjectModel);
        }

        private void DeleteSpawnableWorldObjectSpawnSystem()
        {
            ClearData();
            WorldObjectsModel.RemoveSpawnable(WorldObjectModel);
        }

        private void DeleteWorldObjectSpawnSystem()
        {
            ClearData();
            WorldObjectsModel.RemoveSavable(WorldObjectModel);
        }

        private void ClearData()
        {
            var datas = WorldObjectModel.GetComponentsInChildren<IData>(true);
            foreach (var data in datas)
            {
                foreach (var unique in data.Uniques)
                {
                    StorageModel.Clear(unique);
                    StorageModel.Untracking(unique);
                }
            }
        }
    }
}
