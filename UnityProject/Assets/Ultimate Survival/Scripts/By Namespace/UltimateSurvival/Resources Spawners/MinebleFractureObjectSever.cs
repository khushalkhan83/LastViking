using System;
using System.Collections;
using System.Collections.Generic;
using Core.Storage;
using Game.Models;
using UnityEngine;
using System.Linq;

namespace UltimateSurvival
{
    public class MinebleFractureObjectSever : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public List<MinebleFractureObjectData> dataList = new List<MinebleFractureObjectData>();
        }

        [Serializable]
        public class MinebleFractureObjectData
        {
            public float resourceValue;
            public long timeSpawnTicks;

            public MinebleFractureObjectData(){}

            public MinebleFractureObjectData(float resourceValue, long timeSpawnTicks)
            {
                this.resourceValue = resourceValue;
                this.timeSpawnTicks = timeSpawnTicks;
            }
        }

        [SerializeField] private Data data = default;
        [SerializeField] private MinebleFractureObject[] minebleFractureObjects = default;


        public List<MinebleFractureObjectData> DataList
        {
            get{ return data.dataList;}
            set{ data.dataList = value;}
        }

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;


        private void OnEnable() 
        {
            minebleFractureObjects = GetComponentsInChildren<MinebleFractureObject>(true);
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
            if(DataList.Count != minebleFractureObjects.Length)
            {
                DataList = minebleFractureObjects.Select(m => new MinebleFractureObjectData(m.ResourceValue, m.TimeSpawnTicks)).ToList();
            }
            else
            {
                for(int i = 0; i < minebleFractureObjects.Length; i++)
                {
                    DataList[i].resourceValue = minebleFractureObjects[i].ResourceValue;
                    DataList[i].timeSpawnTicks = minebleFractureObjects[i].TimeSpawnTicks;
                }
            }

            data.ChangeData();
        }

        private void ApplyData()
        {
            int i = 0;
            for(; i < DataList.Count && i < minebleFractureObjects.Length; i++)
            {
                var minebleFractureData = DataList[i];
                minebleFractureObjects[i].Init(minebleFractureData.resourceValue, minebleFractureData.timeSpawnTicks);
            }

            for(; i < minebleFractureObjects.Length; i ++)
            {
                minebleFractureObjects[i].Init(minebleFractureObjects[i].InitialResourceValue, 0);
            }
        }

    }
}
