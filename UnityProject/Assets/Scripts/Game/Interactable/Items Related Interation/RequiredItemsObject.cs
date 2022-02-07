using System;
using System.Collections;
using System.Collections.Generic;
using Core.Storage;
using Game.Models;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.Events;
using Extensions;
using NaughtyAttributes;

namespace Game.Interactables
{
    public class RequiredItemsObject : ItemsRelatedInteractableBase
    {
        [Serializable]
        public class Data : DataBase
        {
            public bool ItemsPlaced;          

            public void SetItemsPlaced(bool value) 
            {
                ItemsPlaced = value;
                ChangeData();
            }         
        }

        #region Data
#pragma warning disable 0649
        [HideIf("_ignorSave")]
        [SerializeField] private Data _data;
        [SerializeField] private bool _ignorSave;
        [SerializeField] private SavableItem[] requiredItems;
        [SerializeField] private GameObject _ghostObject;
        [SerializeField] private GameObject _realObject;
        [SerializeField] private UnityEvent _onItemsPlaced;

#pragma warning restore 0649
        #endregion

        public Data Data_ => _data;
        public override SavableItem[] RequiredItems => requiredItems;
        public GameObject GhostObject => _ghostObject;
        public GameObject RealObject => _realObject;

        public StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        public bool ItemsPlaced {
            get => Data_.ItemsPlaced;
            private set => Data_.SetItemsPlaced(value);
        }

        public void SetItemsPlaced(bool value) => ItemsPlaced = value;
        
        private bool processed = false;

        private void OnEnable()
        {
            if(!_ignorSave)
            {
                if(processed) return;

                bool result = StorageModel.TryProcessing(Data_);
                
                processed = result;
            }
            SetupView();
        }

        private void OnDisable()
        {
            // StorageModel.Untracking(Data_);
        }

        public void SetActiveGhostObject(bool isActive) => GhostObject.SetActive(isActive);
        public void SetActiveRealObject(bool isActive) => RealObject.SetActive(isActive);

        public override bool CanUse() => !ItemsPlaced;
        public override void Use()
        {
            ItemsPlaced = true;
            _onItemsPlaced.Invoke();
            SetupView();
        }

        private void SetupView()
        {
            GhostObject.CheckNull()?.SetActive(!ItemsPlaced);
            RealObject.CheckNull()?.SetActive(ItemsPlaced);
        }
    }
}
