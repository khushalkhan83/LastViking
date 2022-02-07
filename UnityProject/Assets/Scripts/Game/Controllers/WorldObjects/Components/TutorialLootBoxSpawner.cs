using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Storage;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class TutorialLootBoxSpawner : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public bool isLootBoxCollected;

            public void SetIsLootBoxCollected(bool value)
            {
                isLootBoxCollected = value;
                ChangeData();
            }
        }

        public static TutorialLootBoxSpawner Instance;

        public event Action OnLootBoxCollected;
        public event Action OnInit;

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        public bool Inited {get; private set;}
        public bool IsLootBoxCollected
        {
            get {return _data.isLootBoxCollected;}
            private set {_data.SetIsLootBoxCollected(value);}
        }
        public Sprite LootBoxIcon => _lootBoxIcon;

        [SerializeField] private Data _data;
        [SerializeField] private int _tutrialStep = 4;
        [SerializeField] private GameObject _lootBoxObject = default;
        [SerializeField] private Sprite _lootBoxIcon;

        #region MonoBehaviour
        private void Awake() 
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        private void OnEnable()
        {

            if(Inited) return;
            StorageModel.TryProcessing(_data);
            Inited = true;
            OnInit?.Invoke();
        }
        #endregion

        public void TrySpawnLootBox()
        {
            if (!IsLootBoxCollected)
            {
                SpawnLootBox();
            }
            else
            {
                _lootBoxObject.SetActive(false);
            }
        }

        public bool IsSpawnedLootObject(LootObject lootObject)
        {
            var loot = _lootBoxObject.GetComponentInChildren<LootObject>();
            bool isSpawnedLoot = lootObject == loot;
            return isSpawnedLoot;
        }

        private void SpawnLootBox()
        {
            _lootBoxObject.SetActive(true);
            _lootBoxObject.GetComponentInChildren<DropContainerObject>().Place(transform.position, 0.1f);
            _lootBoxObject.GetComponentInChildren<LootObject>().ItemsContainer.OnChangeCell += OnChangeCell;
        }

        private void OnChangeCell(CellModel cell)
        {
            if(_lootBoxObject.GetComponentInChildren<LootObject>().ItemsContainer.Cells.All(x => x.IsEmpty))
            {
                _lootBoxObject.GetComponentInChildren<LootObject>().ItemsContainer.OnChangeCell -= OnChangeCell;
                IsLootBoxCollected = true;
                OnLootBoxCollected?.Invoke();
            }
        }
    }
}
