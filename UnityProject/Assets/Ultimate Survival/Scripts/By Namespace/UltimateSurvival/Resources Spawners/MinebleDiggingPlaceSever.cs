using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Storage;
using Game.Models;
using UnityEngine;

namespace UltimateSurvival
{
    public class MinebleDiggingPlaceSever : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public List<MinebleDiggingObjectData> dataList = new List<MinebleDiggingObjectData>();
        }

        [Serializable]
        public class MinebleDiggingObjectData
        {
            public float resourceValue;
            public long timeSpawnTicks;

            public MinebleDiggingObjectData(){}

            public MinebleDiggingObjectData(float resourceValue, long timeSpawnTicks)
            {
                this.resourceValue = resourceValue;
                this.timeSpawnTicks = timeSpawnTicks;
            }
        }

        [SerializeField] private Data data = default;
        [SerializeField] private MineableObjectDiggingPlace[] mineableObjectDiggingPlaces = default;


        public List<MinebleDiggingObjectData> DataList
        {
            get{ return data.dataList;}
            set{ data.dataList = value;}
        }

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;


        private void OnEnable() 
        {
            mineableObjectDiggingPlaces = GetComponentsInChildren<MineableObjectDiggingPlace>(true);
            StorageModel.TryProcessing(data);
            ApplyData();
            StorageModel.OnPreSaveChanged += OnPreSaveChanged;
        }

        private void OnDisable() 
        {
            StorageModel.OnPreSaveChanged -= OnPreSaveChanged; 
        }

        private void OnPreSaveChanged()
        {
            if(DataList.Count != mineableObjectDiggingPlaces.Length)
            {
                DataList = mineableObjectDiggingPlaces.Select(m => new MinebleDiggingObjectData(m.Amount, m.TimeSpawnTicks)).ToList();
            }
            else
            {
                for(int i = 0; i < mineableObjectDiggingPlaces.Length; i++)
                {
                    DataList[i].resourceValue = mineableObjectDiggingPlaces[i].Amount;
                    DataList[i].timeSpawnTicks = mineableObjectDiggingPlaces[i].TimeSpawnTicks;
                }
            }

            data.ChangeData();
        }

        private void ApplyData()
        {
            int i = 0;
            for(; i < DataList.Count && i < mineableObjectDiggingPlaces.Length; i++)
            {
                var minebleFractureData = DataList[i];
                mineableObjectDiggingPlaces[i].LoadDataFromSave(minebleFractureData.resourceValue, minebleFractureData.timeSpawnTicks);
            }

            for(; i < mineableObjectDiggingPlaces.Length; i ++)
            {
                mineableObjectDiggingPlaces[i].LoadDataFromSave(mineableObjectDiggingPlaces[i].InitialAmount, 0);
            }
        }
    }
}
