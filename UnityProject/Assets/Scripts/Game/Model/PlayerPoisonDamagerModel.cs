using Core.Storage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    public class PlayerPoisonDamagerModel : MonoBehaviour
    {
        [Serializable]
        public class PoisonData
        {
            #region Data
#pragma warning disable 0649

            [SerializeField] private float _allTime;
            [SerializeField] private float _remainingTime;
            [SerializeField] private float _damage;

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

            public float Damage
            {
                get => _damage;
                set => _damage = value;
            }
        }

        [Serializable]
        public class Data : DataBase
        {
            public List<PoisonData> Poisons;

            public void PoisonProcess() => ChangeData();

            public void AddPoison(PoisonData data)
            {
                Poisons.Add(data);
                ChangeData();
            }

            public void RemovePoisonAt(int index)
            {
                Poisons.RemoveAt(index);
                ChangeData();
            }

            public void ClearPoisons()
            {
                Poisons.Clear();
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private StorageModel _storageModel;
        [SerializeField] private float _damageTimeDelay;
        [SerializeField] private int _damageCoolDown;

#pragma warning restore 0649
        #endregion

        public Data _Data => _data;

        public List<PoisonData> Poisons => _data.Poisons;

        public bool IsHasPoison => _data.Poisons.Count > 0;

        public StorageModel StorageModel => _storageModel;

        public int DamageCoolDown => _damageCoolDown;
        public float DamageTimeDelay => _damageTimeDelay;
        public void AdjustDamageTimeDelay(float value) => _damageTimeDelay += value;
        public void SetDamageTimeDelay(float value) => _damageTimeDelay = value;

        public event Action OnAddPosion;
        public event Action OnAddAntidote;
        public event Action OnLoadData;
        public event Action OnEndPoison;

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

        public void EndPoison()
        {
            OnEndPoison?.Invoke();
        }

        public float GetPoisonDamage(float deltaTime)
        {
            var result = 0f;

            PoisonData poisonData;

            for (int i = Poisons.Count - 1; i >= 0; i--)
            {
                poisonData = Poisons[i];

                poisonData.RemainingTime -= deltaTime;

                result += GetPoisonDamage(poisonData, deltaTime);

                if (poisonData.RemainingTime <= 0)
                {
                    _Data.RemovePoisonAt(i);
                }
            }

            if (Poisons.Count > 0)
                _Data.PoisonProcess();

            return result;
        }

        private float GetPoisonDamage(PoisonData poison, float deltaTime)
        {
            var damagePerSecond = poison.Damage / poison.AllTime;
            var time = Mathf.Min(poison.RemainingTime, deltaTime);

            return damagePerSecond * time;
        }

        public void AddPoison(float time, float damage)
        {
            var poisonData = new PoisonData()
            {
                AllTime = time,
                RemainingTime = time,
                Damage = damage
            };

            _Data.AddPoison(poisonData);

            OnAddPosion?.Invoke();
        }

        public void AddAntidote()
        {
            _Data.ClearPoisons();

            OnAddAntidote?.Invoke();
        }
    }
}
