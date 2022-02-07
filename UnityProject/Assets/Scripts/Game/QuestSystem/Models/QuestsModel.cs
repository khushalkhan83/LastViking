using System;
using System.Linq;
using EnemiesAttack;
using Game.Providers;
using Game.QuestSystem.Data;
using Game.QuestSystem.Data.QuestTriggers;
using Sirenix.OdinInspector;
using SOArchitecture;
using UnityEditor;
using UnityEngine;


namespace Game.Models
{
    [CreateAssetMenu(fileName = "ML_Quests", menuName = "Model/QuestsModel", order = 0)]
    public class QuestsModel : ScriptableObject
    {   
        #region Data
        #pragma warning disable 0649
        
        [InlineButton("SetQuest")]
        [SerializeField] private QuestData activeQuest;
        [InlineButton("SetStage")]
        [SerializeField] private int activeStage;
        [SerializeField] private MainQuestsProvider mainQuestsProvider;
        [SerializeField] private GameEvent gameEventCutSceneEnd;
        [SerializeField] private EnemiesAttackConfig testConfig;
        #pragma warning restore 0649
        #endregion
        
        #region Dependencies
        private QuestStageDesciptionModel QuestStageDesciptionModel => ModelsSystem.Instance._questStageDesciptionModel;
            
        #endregion

        public const int k_defaultQuestIndex = 0;
        public const int k_defaultStageIndex = 10;

        public const int k_chapterNumStep = 10;

        public QuestData ActiveQuest => activeQuest;
        public int ActiveStage => activeStage;

        public event Action OnPreChangeQuest;
        public event Action OnActivateQuest;
        public event Action OnPreActivateStage;
        public event Action OnActivateStage;

        public event Action OnPreActivateQuest;

        public QuestData GetQuest(int id)
        {
            return mainQuestsProvider[id];
        }

        public int GetActiveQuestIndex() // TODO: add validation for all quest indexes.
        {
            foreach (var index in mainQuestsProvider.QuestsIndexes)
            {
                var quest = mainQuestsProvider[index];
                if(activeQuest == quest) return index;
            }

            throw new Exception("Can`t get quest index");
        }

        public int GetChapter()
        {
            var questIndex = GetActiveQuestIndex();;
            var chapter = questIndex / k_chapterNumStep;

            return chapter;
        }

        public bool TryGetQuetsIndexByChapter(int chapter, out int answer)
        {
            answer = 0;
            foreach (var index in mainQuestsProvider.QuestsIndexes)
            {
                if(index / k_chapterNumStep == chapter)
                {
                    answer = index;
                    return true;
                }
            }
            return false;
        }

        public bool TryGetQuestStageDataIndex(QuestData quest, int stageIndex, out int stageDataIndex)
        {
            for (int i = 0; i < quest.stages.Count; i++)
            {
                bool match = quest.stages[i].num == stageIndex;
                if(match)
                {
                    stageDataIndex = i;
                    return true;
                }
            }

            stageDataIndex = 0;
            return false;
        }

        public EnemiesAttackConfig EnemiesAttackConfig => ActiveTestConfig ? testConfig : ActiveQuest.EnemiesAttackConfig;

        public bool ActiveTestConfig {get; private set;}
        //TODO: remove as debug command
        public void SetTestAttackConfig(bool active) => ActiveTestConfig = active;


        [Button]
        public void ActivateQuest(QuestData questData, int stage = k_defaultStageIndex)
        {
            OnPreChangeQuest?.Invoke();
            OnPreActivateStage?.Invoke();
            activeQuest = questData;
            activeStage = stage;
            OnPreActivateQuest?.Invoke();
            OnActivateQuest?.Invoke();
            OnActivateStage?.Invoke();
        }


        [Button]
        public void ActivateStage(int stage)
        {
            OnPreActivateStage?.Invoke();
            activeStage = stage;
            OnActivateStage?.Invoke();
        }

        public void ActivateStage_Internal(int stage)
        {
            activeStage = stage;
        }

        public void ActivateQuest_Internal(int mainQuestIndex)
        {
            var quest = GetQuest(mainQuestIndex);
            activeQuest = quest;
        }

        public void ResetCinematicCamera()
        {
            #if UNITY_EDITOR
            bool editMode = !EditorApplication.isPlaying;
            if(editMode) return;
            #endif
            gameEventCutSceneEnd?.Raise();
        }

        private StageData Stage => activeQuest.stages.Find(x => x.num == activeStage);

        public int StagesCount => activeQuest.stages.Count;

        public bool IsLastStage => activeQuest.stages.Last() == Stage;

        public int TargetShipLevel => ActiveQuest.TargetShipLevel;
        public bool ShowPopup => Stage.showPopup;
        public string StageDescription => QuestStageDesciptionModel.GetDescription(Stage);
        public Sprite StageIcon => Stage.icon;
        public bool StagePlayUpdateSound => Stage.playUpdateQuestSound;
        public QuestItemData QuestItemData => ActiveQuest?.QuestItemData;

        // TEMP
        public bool UpgradeStage => Stage.canUpgradeShelter;

        [NaughtyAttributes.Button]
        private void SetQuest() => ActivateQuest(activeQuest);
        [NaughtyAttributes.Button]
        private void SetStage() => ActivateStage(activeStage);
    }
}