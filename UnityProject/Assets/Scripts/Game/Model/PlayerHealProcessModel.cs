using Core.Storage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    public class PlayerHealProcessModel : MonoBehaviour
    {
        [Serializable]
        public class HealData
        {
            #region Data
#pragma warning disable 0649

            [SerializeField] private float _allTime;
            [SerializeField] private float _remainingTime;
            [SerializeField] private float _healValue;
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

            public float HealValue
            {
                get => _healValue;
                set => _healValue = value;
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
            public List<HealData> HealDatas;

            public void HealProcess() => ChangeData();

            public void AddHeal(HealData data)
            {
                HealDatas.Add(data);
                ChangeData();
            }

            public void RemoveHealAt(int index)
            {
                HealDatas.RemoveAt(index);
                ChangeData();
            }

            public void ClearHeals()
            {
                HealDatas.Clear();
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

        public List<HealData> HealDatas => _data.HealDatas;

        public bool IsHasHeal => _data.HealDatas.Count > 0;

        public StorageModel StorageModel => _storageModel;

        public event Action OnAddHeal;
        public event Action OnLoadData;
        public event Action OnEndHeal;

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

        public void EndHeal()
        {
            OnEndHeal?.Invoke();
        }

        public void UpdateHealValues(float deltaTime)
        {
            HealData healData;

            for (int i = HealDatas.Count - 1; i >= 0; i--)
            {
                healData = HealDatas[i];
                healData.RemainingTime -= deltaTime;

                if (healData.RemainingTime <= 0)
                {
                    _Data.RemoveHealAt(i);
                }
            }

            if (HealDatas.Count > 0)
                _Data.HealProcess();
        }

        public bool GetIsOverflow(HealData data) => data.IsOverflow;

        public float GetHealValue(HealData heal, float deltaTime)
        {
            var healPerSecond = heal.HealValue / heal.AllTime;
            var time = Mathf.Min(heal.RemainingTime, deltaTime);

            return healPerSecond * time;
        }

        public void AddHeal(float time, float heal, bool isOverflow)
        {
            var healData = new HealData()
            {
                AllTime = time,
                RemainingTime = time,
                HealValue = heal,
                IsOverflow = isOverflow
            };

            _Data.AddHeal(healData);

            OnAddHeal?.Invoke();
        }
    }
}
