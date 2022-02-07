using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class PlayerDeathModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredFloat ImunitetRemaining;

            public void SetImunitetRemaining(float imunitetRemaining)
            {
                ImunitetRemaining = imunitetRemaining;
                ChangeData();
            }
        }

        [Serializable]
        public class DeathData : DataBase, IImmortal
        {
            public ObscuredInt DeathCount;

            public void SetDeathCount(int deathCount)
            {
                DeathCount = deathCount;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private DeathData _deathData;

        [SerializeField] private ObscuredFloat _imunitetDuration;

#pragma warning restore 0649
        #endregion

        public float ImunitetDuration => _imunitetDuration;

        public float ImmunitetRemaining
        {
            get => _data.ImunitetRemaining;
            protected set => _data.SetImunitetRemaining(value);
        }

        public bool IsImmunable => ImmunitetRemaining > 0;

        public int DeathCount
        {
            get => _deathData.DeathCount;
            protected set => _deathData.SetDeathCount(value);
        }
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        public event Action OnPreRevival; 
        public event Action OnRevival;
        public event Action OnRevivalPrelim;
        public event Action OnBeginImunitet;
        public event Action OnEndImunitet;

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
            StorageModel.TryProcessing(_deathData);
        }

        public void Revival()
        {
            OnPreRevival?.Invoke();
            OnRevival?.Invoke();
        }

        public void PrelimRevival()
        {
            OnPreRevival?.Invoke();
            OnRevivalPrelim?.Invoke();
        }

        public void BeginImunitet()
        {
            ImmunitetRemaining = ImunitetDuration;
            OnBeginImunitet?.Invoke();
        }

        public void UpdateDeathCounter()
        {
            ++_deathData.DeathCount;
        }

        public void ImmunitetProcess(float deltaTime)
        {
            ImmunitetRemaining -= deltaTime;

            if (ImmunitetRemaining < 0)
            {
                ImmunitetRemaining = 0;
                OnEndImunitet?.Invoke();
            }
        }
    }
}
