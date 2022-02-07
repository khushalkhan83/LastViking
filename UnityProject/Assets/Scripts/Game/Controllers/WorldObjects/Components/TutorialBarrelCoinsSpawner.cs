using System;
using Core.Storage;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class TutorialBarrelCoinsSpawner : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public bool NeedSpawnCoins;

            public void SetNeedSpawnCoins(bool needSpawnCoins)
            {
                NeedSpawnCoins = needSpawnCoins;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private int _coinsCount;
        [SerializeField] private Vector3 _dropShiftPosition;

#if UNITY_EDITOR

        [SerializeField] private Color _colorGhost;
        [SerializeField] private Mesh _mesh;

#endif

#pragma warning restore 0649
        #endregion

        public bool NeedSpawnCoins
        {
            get
            {
                return _data.NeedSpawnCoins;
            }
            protected set
            {
                _data.SetNeedSpawnCoins(value);
            }
        }

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private TutorialModel TutorialModel => ModelsSystem.Instance._tutorialModel;
        private CoinObjectsModel CoinObjectsModel => ModelsSystem.Instance._coinObjectsModel;


        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);

            if (NeedSpawnCoins && TutorialModel.Step == 0)
            {
                CreateInstance();
            }
        }

        private void OnDisable()
        {
            
        }

        private void CreateInstance()
        {
            CoinObjectsModel.SpawnAtPosition(_coinsCount, transform.position, transform.position + _dropShiftPosition, 2f, "TutorialBarrel");
            NeedSpawnCoins = false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_mesh)
            {
                Gizmos.color = _colorGhost;
                Gizmos.DrawMesh(_mesh, transform.position + _dropShiftPosition, transform.rotation, transform.localScale);
            }
        }
#endif

    }
}