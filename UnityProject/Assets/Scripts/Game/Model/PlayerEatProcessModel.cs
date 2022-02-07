using Core.Storage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    public class PlayerEatProcessModel : MonoBehaviour
    {
        [Serializable]
        public class EatData
        {
            #region Data
#pragma warning disable 0649

            [SerializeField] private float _allTime;
            [SerializeField] private float _remainingTime;
            [SerializeField] private float _eatValue;
            [SerializeField] private bool _isOverflow;

#pragma warning restore 0649
            #endregion

            public float AllTime
            {
                get => _allTime;
                set => _allTime = value;
            }

            public float RemainingTime
            {
                get => _remainingTime;
                set => _remainingTime = value;
            }

            public float EatValue
            {
                get => _eatValue;
                set => _eatValue = value;
            }

            public bool IsOverflow
            {
                get => _isOverflow;
                set => _isOverflow = value;
            }
        }

        [Serializable]
        public class Data : DataBase
        {
            public List<EatData> EatDatas;

            public void EatProcess() => ChangeData();

            public void AddEat(EatData data)
            {
                EatDatas.Add(data);
                ChangeData();
            }

            public void RemoveEatAt(int index)
            {
                EatDatas.RemoveAt(index);
                ChangeData();
            }

            public void ClearEats()
            {
                EatDatas.Clear();
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private StorageModel _storageModel;

#pragma warning restore 0649
        #endregion

        public Data _Data => _data;

        public List<EatData> EatDatas => _data.EatDatas;

        public bool IsHasEat => _data.EatDatas.Count > 0;

        public StorageModel StorageModel => _storageModel;

        public event Action OnAddEat;
        public event Action OnLoadData;
        public event Action OnEndEat;

        public bool IsInitializeData { get; private set; }

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
            InitializeData();
        }

        public void InitializeData()
        {
            IsInitializeData = true;
            OnLoadData?.Invoke();
        }

        public void EndEat()
        {
            OnEndEat?.Invoke();
        }

        public void UpdateEatValues(float deltaTime)
        {
            EatData eatData;

            for (int i = EatDatas.Count - 1; i >= 0; i--)
            {
                eatData = EatDatas[i];
                eatData.RemainingTime -= deltaTime;

                if (eatData.RemainingTime <= 0)
                {
                    _Data.RemoveEatAt(i);
                }
            }

            if (EatDatas.Count > 0)
                _Data.EatProcess();
        }

        public bool GetIsOverflow(EatData data) => data.IsOverflow;

        public float GetEatValue(EatData eat, float deltaTime)
        {
            var eatPerSecond = eat.EatValue / eat.AllTime;
            var time = Mathf.Min(eat.RemainingTime, deltaTime);

            return eatPerSecond * time;
        }

        public void AddEat(float time, float eat, bool isOverflow)
        {
            var eatData = new EatData()
            {
                AllTime = time,
                RemainingTime = time,
                EatValue = eat,
                IsOverflow = isOverflow
            };

            _Data.AddEat(eatData);

            OnAddEat?.Invoke();
        }
    }
}
