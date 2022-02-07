using Core.Storage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    public class DemolitionProcessModel :  MonoBehaviour, IData
    {
        [Serializable]
        public class Data : DataBase
        {
            public PlaceType PlacedOn;

            public void SetPlacedOn(PlaceType value)
            {
                PlacedOn = value;
                ChangeData();
            }
        }

        [Serializable]
        public enum PlaceType
        {
            None,
            OnGround,
            OnConstruction
        }

        public event Action OnDataInitialize;
        public event Action OnDataLoaded;

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;

#pragma warning restore 0649
        #endregion

        public PlaceType PlacedOn
        {
            get{ return _data.PlacedOn; }
            set{ _data.SetPlacedOn(value); }
        }

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        private  WorldObjectModel _worldObjectModel;
        private WorldObjectModel WorldObjectModel {
            get
            {
                if(_worldObjectModel == null) _worldObjectModel = GetComponent<WorldObjectModel>();
                return _worldObjectModel;
            }
        }

        public IEnumerable<IUnique> Uniques
        {
            get
            {
                yield return _data;
            }
        }

        private void OnEnable()
        {
            if (WorldObjectModel)
            {
                OnDataInitialize += LoadData;
            }
            else
            {
                StorageModel.TryProcessing(_data);
                OnDataLoaded?.Invoke();
            }
        }

        private void LoadData()
        {
            StorageModel.TryProcessing(_data);
            OnDataLoaded?.Invoke();
        }

        public void UUIDInitialize() => OnDataInitialize?.Invoke();
    }
}
