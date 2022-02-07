using System;
using Core.Storage;
using UnityEngine;
using SOArchitecture;

namespace Game.Models
{
    public class ShelterUpgradeModel : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649

        [Serializable]
        public class Data: DataBase
        {
            
        }
        [SerializeField] private Data _data;
        [SerializeField] private GameEvent _gameEventPreCompleateUpgrade;
        #pragma warning restore 0649
        #endregion

        public bool CanBeUpgraded => OnGetCanBeUpgraded.Invoke();
        public bool EnemiesAttackWillStartOnUpgrade => OnGetEnemiesAttackWillStartOnUpgrade.Invoke();
        public bool ShowUpgrade {get; private set;}
        public bool NeedQuestItem => OnGetNeedQuestItem.Invoke();
        public bool UpgradedInThisChapter => OnGetUpgradedInThisChapter.Invoke();
        public bool IsConstructed => OnGetIsConstructed.Invoke();
        
        public event Func<bool> OnGetCanBeUpgraded;
        public event Func<bool> OnGetEnemiesAttackWillStartOnUpgrade;
        public event Func<bool> OnGetNeedQuestItem;
        public event Func<bool> OnGetUpgradedInThisChapter;
        public event Func<bool> OnGetIsConstructed;

        public event Action OnInteractWithShelterUpgradeTable;
        public event Action<bool> OnGetShowUpgradeChanged;
        public event Action OnStartUpgrade;
        public event Action OnPreCompleateUpgrade;
        public event Action OnCompleteUpgrade;


        public void InteractWithShelterUpgradeTable()
        {
            OnInteractWithShelterUpgradeTable.Invoke();
        }

        public void SetShowUpgrade(bool showUpgrade)
        {
            ShowUpgrade = showUpgrade;
            OnGetShowUpgradeChanged?.Invoke(showUpgrade);
        }

        public void StartUpgrade()
        {
            OnStartUpgrade?.Invoke();
        }
        public void PreCompleateUpgrade()
        {
            OnPreCompleateUpgrade?.Invoke();
            _gameEventPreCompleateUpgrade?.Raise();
        }

        public void CompleteUpgrad()
        {
            OnCompleteUpgrade?.Invoke();
        }

        #region MonoBehaviour

        private void OnEnable()
        {
            ModelsSystem.Instance._storageModel.TryProcessing(_data);
        }

        #endregion
    }
}
