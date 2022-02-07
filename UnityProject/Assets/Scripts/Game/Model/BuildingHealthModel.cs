using Core.Storage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    public class BuildingHealthModel : MonoBehaviour, IHealth, IData
    {
        [Serializable]
        public class Data : DataBase
        {
            public float Health;

            public void SetHealth(float health)
            {
                Health = health;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private float _healthMax;
        [SerializeField] private bool _deleteDataAdterDeath = true;

#pragma warning restore 0649
        #endregion

        public DataBase _Data => _data;

        #region Dependencies
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private TutorialBuildingModel TutorialBuildingModel => ModelsSystem.Instance._tutorialBuildingModel;
        private WorldObjectsModel WorldObjectsModel => ModelsSystem.Instance._worldObjectsModel;
        private WorldObjectModel _worldObjectModel;
        private WorldObjectModel WorldObjectModel
        {
            get
            {
                if (_worldObjectModel == null) _worldObjectModel = GetComponentInParent<WorldObjectModel>();
                return _worldObjectModel;
            }
        }
            
        #endregion

        private bool IsDamageBlocked()
        {
            // bool damageBlocked = TutorialBuildingModel.TutorialStage && WorldObjectsModel.IsConstructionItem(WorldObjectModel.WorldObjectID);
            bool damageBlocked = false;
            return damageBlocked;
        }

        #region IData
            
        public IEnumerable<IUnique> Uniques
        {
            get
            {
                yield return _data;
            }
        }
        public event Action OnDataInitialize;
        public void UUIDInitialize() => OnDataInitialize?.Invoke();

        #endregion

        #region IHealth
        public event Action OnChangeHealth;
        public event Action OnDeath;

        public float Health
        {
            get => _data.Health;
            protected set => _data.SetHealth(value);
        }
        public float HealthMax => _healthMax;
        public bool IsDead => Health <= 0;

        public void AdjustHealth(float adjustment)
        {
            if(IsDamageBlocked()) return;

            Health += adjustment;
            OnChangeHealth?.Invoke();

            if (IsDead)
            {
                Death();
            }
        }

        public void SetHealth(float health)
        {
            if(IsDamageBlocked()) return;

            Health = health;
            OnChangeHealth?.Invoke();

            if (IsDead)
            {
                Death();
            }
        }

        public void SetHealthMax(float healthMax)
        {
            _healthMax = healthMax;
            OnChangeHealth?.Invoke();
        }

        #endregion

        #region MonoBehaviour

        private bool dataProcessed;
        private void OnEnable()
        {
            if (WorldObjectModel)
            {
                WorldObjectModel.OnDataInitialize += LoadData;
            }
            else
            {
                if(dataProcessed) return;
                
                StorageModel.TryProcessing(_data);
                dataProcessed = true;
            }
        }

        #endregion

        private void LoadData()
        {
            StorageModel.TryProcessing(_data);
            WorldObjectModel.OnDataInitialize -= LoadData;
        }

        private void Death()
        {
            if(IsDamageBlocked()) return;

            OnDeath?.Invoke();

            if(_deleteDataAdterDeath)
            {
                StorageModel.Clear(_data);
                StorageModel.Untracking(_data);
            }
        }
    }
}
