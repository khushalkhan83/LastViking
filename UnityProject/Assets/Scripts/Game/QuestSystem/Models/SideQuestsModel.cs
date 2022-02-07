using System;
using System.Linq;
using System.Collections.Generic;
using Game.QuestSystem.Data;
using UnityEngine;
using NaughtyAttributes;
using static Game.Models.SideQuestsSaveModel;
using ActivityLog.Data;

namespace Game.Models
{
    [CreateAssetMenu(fileName = "ML_SideQuests", menuName = "Model/SideQuests", order = 0)]
    public class SideQuestsModel : ScriptableObject, IActivityLogEnterencesModel
    {   

        #region Data
        #pragma warning disable 0649

        #region Testing

        [Header("Testing")]

        [SerializeField] private QuestData testQuest;
        [SerializeField] private int stage;
        [Button] void TestActivate() => ActivateQuest(testQuest);
        [Button] void TestCompleate() => CompleateQuest(testQuest);
        [Button] void TestActivateStage() => ActivateStage(testQuest,stage);
            
        #endregion
        [SerializeField] private List<SideQuestProgress> data = new List<SideQuestProgress>();
        
        #pragma warning restore 0649
        #endregion

        public event Action<QuestData> OnActivate;
        public event Action<QuestData> OnCompleate;
        public event Action<QuestData> OnStageCompleted;
        public event Action<QuestData> OnStageChanged;
        public event Action OnDataChanged;


        #region Dependencies
        private QuestStageDesciptionModel QuestStageDesciptionModel => ModelsSystem.Instance._questStageDesciptionModel;
            
        #endregion

        public const int FirstStageIndex = 10;

        

        public void SetProgress(List<SideQuestProgress> progress) => data = progress;
        public List<SideQuestProgress> GetProgress() => data;


        public void ActivateQuest(QuestData questData)
        {
            SideQuestProgress questProgress = data.Find(x => x.questData == questData);
            questProgress.state = SideQuestState.Active;
            OnDataChanged?.Invoke();
            OnActivate?.Invoke(questData);
            OnStageChanged?.Invoke(questData);
        }

        public void CompleateQuest(QuestData questData)
        {
            SideQuestProgress questProgress = data.Find(x => x.questData == questData);
            OnStageCompleted?.Invoke(questData);
            questProgress.state = SideQuestState.Compleated;
            OnDataChanged?.Invoke();
            OnCompleate?.Invoke(questData);
        }

        public void ActivateStage(QuestData questData, int stage)
        {
            var targetQuest = data.Find(x => x.questData == questData);
            OnStageCompleted?.Invoke(questData);
            targetQuest.stage = stage;
            OnDataChanged?.Invoke();
            OnStageChanged?.Invoke(questData);
        }

        public bool IsQuestStateNotActive(QuestData questData)
        {
            var nonActiveQuests = data.Where(x => x.state == SideQuestState.NotActive).ToList();
            if(nonActiveQuests == null) return false;
            SideQuestProgress match = nonActiveQuests.Find(x => x.questData == questData);

            bool answer = match != null;
            return answer;
        }

        public bool IsQuestCompleated(QuestData quest)
        {
            var match = data.Find(x => x.questData == quest);
            if(match == null) return false;

            bool answer = match.state == SideQuestState.Compleated;
            return answer;
        }

        public List<QuestData> GetActiveQuests() => data.Where(x => x.state == SideQuestState.Active).Select(x => x.questData).ToList();
        public int GetQuestStage(QuestData questData) => data.Find(x => x.questData == questData).stage;
        public bool StagePlayUpdateSound(QuestData questData) => GetActiveStageData(questData).playUpdateQuestSound;
        public bool StageShowPopup(QuestData questData) => GetActiveStageData(questData).showPopup;
        public string StageDescription(QuestData questData) => QuestStageDesciptionModel.GetDescription(GetActiveStageData(questData));
        public Sprite StageIcon(QuestData questData) => GetActiveStageData(questData).icon;

        private StageData GetActiveStageData(QuestData questData) => GetStageData(questData,GetQuestStage(questData));
        private StageData GetStageData(QuestData questData, int stageIndex)
        {
            SideQuestProgress questProgress = data.Find(x => x.questData == questData);
            var stageData = questProgress.questData.stages.Find(x => x.num == stageIndex);
            return stageData;
        }

        #region IActivityLogEnterencesModel
        public event Action OnActivitiesCountChanged;
        public List<ActivityLogEnterenceData> GetActivitiesEnterences()
        {
            var answer = new List<ActivityLogEnterenceData>();
            var activeQuests = GetActiveQuests();
            foreach (var questData in activeQuests)
            {
                answer.Add(new ActivityLogEnterenceData(() => StageDescription(questData),StageIcon(questData)));
            }
            return answer;
        }
        #endregion

        #region ScriptableObject

        private void OnEnable()
        {
            OnDataChanged -= DataChangedHandler;
            OnDataChanged += DataChangedHandler;
        }

        private void OnDisable()
        {
            OnDataChanged -= DataChangedHandler;
        }
        #endregion

        private int lastCount;
        private void DataChangedHandler()
        {
            var count = GetActiveQuests().Count;

            if(count != lastCount)
            {
                OnActivitiesCountChanged?.Invoke();
            }

            lastCount = count;
        }
    }
}