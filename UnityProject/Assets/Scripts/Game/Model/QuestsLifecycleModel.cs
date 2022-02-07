using System;
using System.Collections.Generic;
using Game.Providers;
using Game.QuestSystem.Data;
using UnityEngine;

namespace Game.Models
{
    public class QuestsLifecycleModel : MonoBehaviour
    {
        public enum QuestEvent
        {
            WaterfallAvaliable = 10,
            ChurchKeyAqquired = 20,
            NoUpgradesLeft = 30,
            AfterScrollPickup = 40,
            AfterWolfJump = 50,
        }

        public enum SideQuestEvent
        {
            AfterPalmJump = 10,
            AfterZombieJump = 20,
            AfterWaterfallBomb = 30,
        }

        [Serializable]
        public class QuestEventData
        {
            [SerializeField] private int _questIndex;
            [SerializeField] private int _questStage;

            public int QuestIndex => _questIndex;
            public int QuestStage => _questStage;
        }

        [Serializable]
        public class SideQuestEventData
        {
            [SerializeField] private QuestData _questData;
            [SerializeField] private int _questStage;

            public QuestData QuestData => _questData;
            public int QuestStage => _questStage;
        }


        public event Action<QuestEvent> OnQuestEvent;
        public event Action<SideQuestEvent> OnSideQuestEvent;


        private QuetsEventDataProvider QuetsEventDataProvider => ModelsSystem.Instance._quetsEventDataProvider;
        private SideQuetsEventDataProvider SideQuetsEventDataProvider => ModelsSystem.Instance._sideQuetsEventDataProvider;
        private MainQuestsProvider MainQuestsProvider => ModelsSystem.Instance._mainQuestsProvider;
        private QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;

        public bool EventOccured(QuestEvent e)
        {
            bool result = false;

            var data = QuetsEventDataProvider[e];
            bool error = !MainQuestsProvider.TryGetQuestIndex(QuestsModel.ActiveQuest, out var activeQuestIndex);

            if(error)
            {
                Debug.LogError("Error. Can`t get quets index");
                return result;
            }
            
            if(activeQuestIndex == data.QuestIndex)
            {
                result = QuestsModel.ActiveStage >= data.QuestStage;
            }
            else if(activeQuestIndex > data.QuestIndex)
            {
                result = true;
            }

            return result;
        }

        public void TriggerQuestEvent(QuestEvent questEvent) => OnQuestEvent?.Invoke(questEvent);

        public void TriggerSideQuestEvent(SideQuestEvent sideQuestEvent) => OnSideQuestEvent?.Invoke(sideQuestEvent);

    }
}
