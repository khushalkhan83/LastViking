using System;
using Game.Models;
using Game.QuestSystem.Data.QuestTriggers.Actions;
using Game.QuestSystem.Map.Quests;
using Game.QuestSystem.Map.Stages;
using Game.QuestSystem.Map.Triggers;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

namespace Game.QuestSystem.Data.QuestTriggers
{
    [Serializable]
    public class QuestTriggerData
    {
        // [InlineButton("SelectOnMap")]
        // [InlineButton("AddOnMap")]
        [HideInInspector]
        [SerializeField] private QuestTriggerType triggerType = QuestTriggerType.GeneralTrigger;
        [SerializeField] private int id;

        private bool valid;

        public QuestTriggerType TriggerType => triggerType;
        public int ID => id;

        // [ShowIf("IsEventTrigger")]
        // public GameEventData eventTrigger;

        public ActionsWraper actionsGroup;

        // [ShowIf("IsEventTrigger")]
        // [Button]
        // private void TestConditionalTrigger()
        // {
        //     QuestTriggersModel questTriggersModel = ModelsSystem.Instance._questTriggersModel;
        //     questTriggersModel.ActivateTrigger(id);
        // }
        
        // [ShowIf("IsValid")]
        // [Button]
        // private void SelectOnMap()
        // {
        //     #if UNITY_EDITOR
        //     QuestsRoot questsRoot = GameObject.FindObjectOfType<QuestsRoot>();
        //     if(questsRoot == null) return;

        //     QuestData selectedQuest = OdinMenuTree.ActiveMenuTree?.Selection?.SelectedValue as QuestData;

        //     if(selectedQuest == null) return;
            
        //     StageData selectedStage = selectedQuest.stages.Find(x => x.triggerDatas.Find(y => y.data == this) != null);

        //     if(selectedStage == null) return;

        //     Quest questManager = questsRoot.GetQuest(selectedQuest);

        //     if(questManager == null) return;

        //     QuestStage stage = questManager.GetStage(selectedStage.num);

        //     if(stage == null) return;

        //     QuestTrigger trigger = stage.GetTrigger(id);

        //     Highlight(trigger.gameObject);

        //     #endif
        // }

        // [HideIf("IsValid")]
        // [Button]
        // private void AddOnMap()
        // {
        //     #if UNITY_EDITOR

        //     QuestsRoot questsRoot = GameObject.FindObjectOfType<QuestsRoot>();
        //     if(questsRoot == null) return;

        //     QuestData selectedQuest = OdinMenuTree.ActiveMenuTree?.Selection?.SelectedValue as QuestData;

        //     if(selectedQuest == null) return;
            
        //     StageData selectedStage = selectedQuest.stages.Find(x => x.triggerDatas.Find(y => y.data == this) != null);

        //     if(selectedStage == null) return;

        //     Quest quest = questsRoot.GetQuest(selectedQuest);

        //     if(quest == null)
        //     {
        //         quest = questsRoot.AddQuest(selectedQuest);
        //     }

        //     QuestStage stage = quest.GetStage(selectedStage.num);

        //     if(stage == null)
        //     {
        //         stage = quest.AddStage(selectedStage.num);
        //         var trigger = stage.AddTrigger(ID,triggerType);
        //         SetExtraData(trigger);
        //         Highlight(trigger.gameObject);
        //     }
        //     else
        //     {
        //         QuestTrigger trigger = stage.GetTrigger(id);
        //         if(trigger == null)
        //         {
        //             trigger = stage.AddTrigger(ID,triggerType);
        //             SetExtraData(trigger);
        //             Highlight(trigger.gameObject);
        //         }
        //         else
        //         {
        //             Debug.LogError("Trigger with this id exist");
        //             return;
        //         }
        //     }

        //     Validate();

        //     #endif
        // }

        private void SetExtraData(QuestTrigger trigger)
        {
            if(false) return;
            // if(!IsEventTrigger()) return;

            // (trigger as EventTrigger)?.SetEvent(eventTrigger.gameEvent);
        }

        [Button]
        [GUIColor("ButtonColor")]
        [HideIf("IsValid")]
        private void NotValid()
        {
            
        }
        private Color ButtonColor() => Color.red;

        private void Highlight(GameObject go)
        {
            #if UNITY_EDITOR
            Selection.activeGameObject = go;
            EditorGUIUtility.PingObject(go);
            #endif
        }

        private bool IsValid => valid;



        // public bool IsEventTrigger() => triggerType == QuestTriggerType.EventTrigger;

        // public bool Validate()
        // {
        //     valid = TryGetTrigger(out QuestTrigger trigger);
        //     return valid;
        // }


        // private bool TryGetTrigger(out QuestTrigger answer)
        // {
        //     answer = null;
        //     #if UNITY_EDITOR
            
            
        //     QuestsRoot questsRoot = GameObject.FindObjectOfType<QuestsRoot>();
        //     if(questsRoot == null) return false;

        //     QuestData selectedQuest = OdinMenuTree.ActiveMenuTree?.Selection?.SelectedValue as QuestData;

        //     if(selectedQuest == null) return false;
            
        //     StageData selectedStage = selectedQuest.stages.Find(x => x.triggerDatas.Find(y => y.data == this) != null);

        //     if(selectedStage == null) return false;

        //     Quest quest = questsRoot.GetQuest(selectedQuest);

        //     if(quest == null) return false;

        //     QuestStage stage = quest.GetStage(selectedStage.num);

        //     if(stage == null) return false;

        //     QuestTrigger trigger = stage.GetTrigger(id);

        //     if(trigger == null) return false;

        //     answer = trigger;
        //     #endif
        //     return true;
        // }
    }
}