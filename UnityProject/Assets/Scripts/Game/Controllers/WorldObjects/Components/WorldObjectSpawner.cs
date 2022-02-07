using Core.Storage;
using Game.Models;
using Game.Providers;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Game.Controllers
{
    public class WorldObjectSpawner : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private string _id;
        [SerializeField] private WorldObjectID _worldObjectID;
        [SerializeField] private Transform _container;

#if UNITY_EDITOR

        [SerializeField] private Mesh _mesh;
#endif

#pragma warning restore 0649
        #endregion

        public string Id => _id;
        public Transform Container => _container;
        public WorldObjectID WorldObjectID => _worldObjectID;
        public WorldObjectCreator WorldObjectCreator => ModelsSystem.Instance._worldObjectCreator;
        public GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;

        public WorldObjectModel Instance { get; private set; }

        private void Start()
        {
            CreateInstance();
        }

        private void CreateInstance()
        {
            var tfm = (Container != null) ? Container : transform;
            Instance = WorldObjectCreator.CreateAsSpawnable(WorldObjectID, tfm.position, tfm.rotation, tfm.localScale, DataProcessing, transform); // [transform - change]
            Instance.transform.parent = Container;
        }

        private void DataProcessing(IEnumerable<IUnique> uniques)
        {
            var add = '.' + Id;
            foreach (var item in uniques)
            {
                item.UUID += add;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_mesh)
            {
                Gizmos.color = new Color(1, 1, 1, .5f);
                Gizmos.DrawMesh(_mesh, transform.position, transform.rotation, transform.localScale);
            }
        }
#endif

    }
}
