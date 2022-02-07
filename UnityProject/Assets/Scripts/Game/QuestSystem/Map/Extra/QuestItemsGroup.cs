using System;
using System.Linq;
using UnityEngine;
using Game.Models;
using Core.Storage;
using System.Collections.Generic;
using UnityEngine.Events;
using Game.Audio;
using Game.Data;

namespace Game.QuestSystem.Map.Extra
{
    [DefaultExecutionOrder(-1)] // we neeed to process data befoure recive items calls
    public class QuestItemsGroup : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [Serializable]
        public class Data: DataBase
        {
            [SerializeField] private List<bool> _items = new List<bool>();

            public List<bool> Items
            {
                get { return _items;}
            }

            public void Reset(int itemsCount)
            {
                _items = new List<bool>();
                for (int i = 0; i < itemsCount; i++)
                    _items.Add(false);

                ChangeData();
            }

            public void SetValue(int index, bool value)
            {
                _items[index] = value;
                ChangeData();
            }
        }
        [SerializeField] private Data _data;
        [SerializeField] private List<GameObject> _questItemViews;
        [SerializeField] private UnityEvent _recivedAllItems;

        private bool processedData = false;

        #pragma warning restore 0649
        #endregion
        private SpesialMessagesModel SpesialMessagesModel => ModelsSystem.Instance._spesialMessagesModel;
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private ApplicationCallbacksModel ApplicationCallbacksModel => ModelsSystem.Instance._applicationCallbacksModel;
        private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;
        private QuestNotificationsModel QuestNotificationsModel => ModelsSystem.Instance._questNotificationsModel;
        private AudioSystem AudioSystem => AudioSystem.Instance;
        private QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;
        private LocalizationModel LocalizationModel => ModelsSystem.Instance._localizationModel;

        private int QuestItemsCount => _questItemViews.Count;
        private const AudioID RecivedQuestItemSoundID = AudioID.Blueprint;

        #region MonoBehaviour

        private void OnEnable()
        {
            if(!processedData)
            {
                processedData = true;
                bool existed = StorageModel.TryProcessing(_data);
                if(!existed) _data.Reset(QuestItemsCount);
            }

            UpdateViews();
            CheckRecivedAllItems();
        }

        private void OnDisable()
        {
            // call RequireItemsGroup reset when state is changed. not when application is quiting
            if(ApplicationCallbacksModel.IsApplicationQuitting || PlayerScenesModel.SceneTransition) return;

            _data.Reset(QuestItemsCount);
        }
        #endregion

        public void ReciveQuestItem(GameObject view)
        {
            if(!_questItemViews.Contains(view)) return;

            var viewIndex = _questItemViews.IndexOf(view);
            _data.SetValue(viewIndex,true);

            AudioSystem.PlayOnce(RecivedQuestItemSoundID);
            UpdateViews();
            CheckRecivedAllItems();
        }

        private void UpdateViews()
        {
            for (int i = 0; i < QuestItemsCount; i++)
            {
                bool recived = _data.Items[i];
                _questItemViews[i].gameObject.SetActive(!recived);
            }
        }

        private void CheckRecivedAllItems()
        {
            if(IsRecivedAllItems())
            {
                QuestNotificationsModel.SetDefaultState();
                var questItemData = QuestsModel.QuestItemData;

                if(questItemData == null) return;
                RecivedItemMessageData data = new RecivedItemMessageData(questItemData.ItemIcon,
                                                                         LocalizationModel.GetString(questItemData.LocalizationKeyID),
                                                                         string.Empty);
                SpesialMessagesModel.RecivedItem(data);
                _recivedAllItems?.Invoke();
            }
            else
            {
                QuestNotificationsModel.SetTargetItemsCount(GetRecivedItemsCount(),QuestItemsCount);
            }
        }

        private int GetRecivedItemsCount() => _data.Items.Where(x => x == true).Count();
        private bool IsRecivedAllItems() => GetRecivedItemsCount() >= QuestItemsCount;
    }
}