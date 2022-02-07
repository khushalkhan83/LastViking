using Core.Storage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    public enum WorldObjectCreationType
    {
        None = 0,
        World = 1,
        Spawner = 2,
    }

    public class WorldObjectModel : MonoBehaviour, IData
    {
        [Serializable]
        public class WorldObjectData : DataBase
        {
            public Vector3 Position;
            public Vector3 Scale;
            public Quaternion Rotation;

            public void SetPosition(Vector3 pos)
            {
                Position = pos;
                ChangeData();
            }

            public void SetRotation(Quaternion rot)
            {
                Rotation = rot;
                ChangeData();
            }

            public void SetScale(Vector3 scale)
            {
                Scale = scale;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private WorldObjectData _data;
        [SerializeField] private WorldObjectID _worldObjectID;

#pragma warning restore 0649
        #endregion

        public WorldObjectData Data => _data;
        public WorldObjectID WorldObjectID => _worldObjectID;

        public Vector3 Position
        {
            get => Data.Position;
            private set => Data.SetPosition(value);
        }

        public Quaternion Rotation
        {
            get => Data.Rotation;
            private set => Data.SetRotation(value);
        }

        public Vector3 Scale
        {
            get => Data.Scale;
            private set => Data.SetScale(value);
        }

        public WorldObjectCreationType WorldObjectCreationType { get; private set; }

        public uint ID { get; set; }
        public EnvironmentSceneID EnvironmentSceneID {get; set;}

        public IEnumerable<IUnique> Uniques
        {
            get
            {
                yield return _data;
            }
        }

        public event Action OnDataInitialize;
        public event Action OnPreDelete;
        public event Action OnDelete;

        public void Delete() 
        {
            OnPreDelete?.Invoke();
            OnDelete?.Invoke();
        }

        public void UUIDInitialize() => OnDataInitialize?.Invoke();

        public void SetPosition(Vector3 position) => Position = position;
        public void SetRotation(Quaternion rotation) => Rotation = rotation;
        public void SetScale(Vector3 scale) => Scale = scale;
        public void SetCreationType(WorldObjectCreationType worldObjectSpawnSystemID) => WorldObjectCreationType = worldObjectSpawnSystemID;
    }
}
