using System;
using Core.Storage;
using Game.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Game.QuestSystem.Map.Extra.Environment
{
    public class QuestDependantInteractable : MonoBehaviour
    {
        [Serializable]
        public class Data: DataBase
        {
            [SerializeField] private bool used;
            public bool Used
            {
                get {return used;}
                private set {used = value; ChangeData();}
            }

            public void SetUsed(bool value) => Used = value;
        }

        #region Data
        #pragma warning disable 0649
        [SerializeField] private Data _data;
        [SerializeField] private QuestsLifecycleModel.QuestEvent _questEvent;
        [SerializeField] private UnityEvent _onUse;
        [SerializeField] private Sprite _itemIcon;
        
        #pragma warning restore 0649
        #endregion


        private QuestsLifecycleModel QuestsLifecycleModel => ModelsSystem.Instance._questsLifecycle;
        private QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        private bool EventOccured() => QuestsLifecycleModel.EventOccured(_questEvent);

        #region MonoBehaviour
        private void OnEnable() 
        {
            StorageModel.TryProcessing(_data);
            QuestsModel.OnActivateStage += OnActivateState;
        }

        private void OnDisable() 
        {
            StorageModel.Untracking(_data);
            QuestsModel.OnActivateStage -= OnActivateState;
        }
        #endregion

        private void OnActivateState()
        {
            
        }

        public Sprite GetItemIcon() => _itemIcon;

        public bool Avaliable() => !_data.Used;

        public bool CanUse()
        {
            if(EventOccured() && Avaliable()) return true;
            else return false;
        }

        public void Use()
        {
            if(!CanUse()) return;

            _data.SetUsed(true);

            _onUse?.Invoke();
        }
    }
}