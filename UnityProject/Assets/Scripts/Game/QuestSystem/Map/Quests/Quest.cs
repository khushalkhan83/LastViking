using System;
using System.Collections.Generic;
using Game.Models;
using Game.QuestSystem.Data;
using Game.QuestSystem.Map.Stages;
using UnityEngine;
using Extensions;
using CustomeEditorTools;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using NaughtyAttributes;

namespace Game.QuestSystem.Map.Quests
{
    // [ExecuteAlways]
    public class Quest : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649

        [SerializeField] private QuestData quest;
        [SerializeField] private List<QuestStage> stages = new List<QuestStage>();
        
        #pragma warning restore 0649
        #endregion

        // different cintaxis because of Execute Always
        #region Dependencies
        private QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;
        private SideQuestsModel SideQuestsModel => ModelsSystem.Instance._sideQuestsModel;
        private TutorialModel TutorialModel => ModelsSystem.Instance?._tutorialModel;
        private CinematicModel CinematicModel => ModelsSystem.Instance?._cinematicModel;
        
        private QuestsRoot questsRoot;
        private QuestsRoot QuestsRoot => questsRoot == null ? (questsRoot = GetComponentInParent<QuestsRoot>()) : questsRoot;
            
        #endregion

        private int ActiveStageIndex => IsMainQuest ? QuestsModel.ActiveStage : SideQuestsModel.FirstStageIndex;

        public QuestData Data => quest;
        public bool IsMainQuest => QuestsRoot.IsMainQuest(this);
        public int StagesCount => stages.Count;

        #region MonoBehaviour
        private void OnEnable()
        {
            if(this.IsOpenedInPrefabMode()) return;

            if(!this.IsEditMode() && !TutorialModel.IsComplete)
            {
                TutorialModel.OnComplete += MainLogic;
            }
            else
                MainLogic();
        }

        private void OnDisable()
        {
            if(this.IsOpenedInPrefabMode()) return;

            QuestsModel.OnPreActivateQuest -= DeactivateAllStages;
            QuestsModel.OnActivateStage -= ActivateOnlyActiveStage;

            SideQuestsModel.OnActivate -= OnSideQuestActivate;
            SideQuestsModel.OnCompleate -= OnSideQuestCompleate;
            SideQuestsModel.OnStageChanged -= OnStageChanged;

            DeactivateAllStages();
        }

        #endregion

        private void MainLogic()
        {
            if(IsMainQuest)
            {
                QuestsModel.OnPreActivateQuest += DeactivateAllStages;
                QuestsModel.OnActivateStage += ActivateOnlyActiveStage;
                ActivateOnlyActiveStage();
            }
            else
            {
                SideQuestsModel.OnActivate += OnSideQuestActivate;
                SideQuestsModel.OnCompleate += OnSideQuestCompleate;
                SideQuestsModel.OnStageChanged += OnStageChanged;

                var match = SideQuestsModel.GetActiveQuests().Find(x => x == Data);
                if(match == null) return;

                int stage = SideQuestsModel.GetQuestStage(match);
                ActivateStage(GetStage(stage));
            }

        }

        private void OnSideQuestActivate(QuestData data)
        {
            if(!IsThisQuest(data)) return;

            DeactivateAllStages();
        }

        private void OnSideQuestCompleate(QuestData data)
        {
            if(!IsThisQuest(data)) return;

            DeactivateAllStages();
        }

        private void OnStageChanged(QuestData data)
        {
            if(!IsThisQuest(data)) return;

            var stage = SideQuestsModel.GetQuestStage(data);

            ActivateStage(GetStage(stage));
        }

        private void ActivateOnlyActiveStage() => ActivateStage(GetStage(ActiveStageIndex));
        
        private void ActivateStage(QuestStage targetStage)
        {
            CinematicModel?.ResetCameraInPlayMode();

            foreach (var stage in stages)
            {
                bool activate = stage == targetStage;
                stage.gameObject.SetActive(activate);
                

                #if UNITY_EDITOR
                if(activate && EditorGameSettings.Instance.autoSelectQuestStageInEditor)
                {
                    var actorsRoot = GameObjectsUtil.GetObjectsByFilter(stage.gameObject, new List<string> {"Actors"}).FirstOrDefault();

                    bool hasActors = actorsRoot != null &&  actorsRoot.transform.childCount > 0;

                    if(hasActors)
                        EditorGUIUtility.PingObject(actorsRoot);
                    else
                        EditorGUIUtility.PingObject(stage);
                }
                #endif
            }
        }


        private void DeactivateAllStages()
        {
            stages.ForEach(x => x.gameObject.SetActive(false));
        }

        public QuestStage GetStage(int stageNum)
        {
            bool error = !QuestsModel.TryGetQuestStageDataIndex(quest,stageNum, out int stageDataIndex);

            if(error)
            {
                // TODO: handle this scenario
                Debug.LogError($"Cant find stage {stageNum} for {quest.name}");
            }

            return stages[stageDataIndex];
        }

        public bool TryGetNextStageIndex(QuestStage stage, out int? nextStageIndex)
        {
            nextStageIndex = null;
            if(!stages.Contains(stage)) return false;


            int nextStageComponentIndex = stages.IndexOf(stage) + 1;

            bool outOfRange = stages.IndexOutOfRange(nextStageComponentIndex);

            if(outOfRange)
            {
                return true;
            }
            else
            {
                nextStageIndex = quest.stages[nextStageComponentIndex].num;
            }

            return true;
        }

        public bool IsPreLastStage(QuestStage stage)
        {
            if(!stages.Contains(stage)) return false;
            int stageIndex = stages.IndexOf(stage);
            return stageIndex == stages.Count - 2;
        }

        private bool IsThisQuest(QuestData data) => data == Data;

        [Button]
        private void ResetData()
        {
            #if UNITY_EDITOR
            if(!this.IsOpenedInPrefabMode())
            {
                EditorUtility.DisplayDialog("Важная информация", "ResetData работает только в режиме префаба", "Ok");
                return;
            }

            stages.Clear();
            var newStages = GetComponentsInChildren<QuestStage>(includeInactive:true);
            stages.AddRange(newStages);

            EditorUtility.SetDirty(this);
            #endif
        }
    }
}