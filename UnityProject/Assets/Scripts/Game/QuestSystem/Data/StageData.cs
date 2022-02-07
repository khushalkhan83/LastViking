using System;
using System.Collections.Generic;
using Game.Models;
using Game.Objectives;
using Game.QuestSystem.Data.QuestTriggers;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game.QuestSystem.Data
{
    [Serializable]
    public class StageData
    {
        [TableColumnWidth(30,resizable: false)]
        public int num;
        [TableColumnWidth(120,resizable: false)]
        public bool showPopup = true;
        [TableColumnWidth(50,resizable: false)]
        public bool useLocID = false;
        [GUIColor("DescriptionColor")]
        [TableColumnWidth(150,resizable: false)]
        [TextArea]
        public string description;
        [GUIColor("DescriptionKeyColor")]
        [TableColumnWidth(150,resizable: false)]
        public LocalizationKeyID descriptionKey;
        [TableColumnWidth(50,resizable: false)]
        [PreviewField(Alignment = ObjectFieldAlignment.Left)]
        public Sprite icon;

        [TableColumnWidth(100,resizable: false)]
        public bool canUpgradeShelter;
        public bool playUpdateQuestSound;
        [HideInInspector]
        public List<QuestTriggerWrapper> triggerDatas = new List<QuestTriggerWrapper>(){new QuestTriggerWrapper()};

        private static QuestsModel questsModel;
        private static QuestsModel QuestsModel {get => questsModel ?? (questsModel = GameObject.FindObjectOfType<ModelsSystem>()._questsModel);}

        [VerticalGroup("Buttons")]
        [Button]
        private void ActivateStage()
        {
            bool stageIsInSelectedQuest = QuestsModel.ActiveQuest.stages.Contains(this);
            if(stageIsInSelectedQuest)
                QuestsModel.ActivateStage(num);
            else
            {
                #if UNITY_EDITOR
                EditorUtility.DisplayDialog("Внимание", "Чтобы активировать стадию, для начала нужно активировать квест к которому она относиться.", "Ok");
                #endif   
            }
        }

        public bool Validate()
        {
            bool answer = true;
            foreach (var triggerData in triggerDatas)
            {
                bool valid = triggerData.Validate();
                if(!valid) answer = false;
            }

            return answer;
        }

        private Color DescriptionColor()
        {
            return useLocID ? Color.yellow : Color.white;
        }

        private Color DescriptionKeyColor()
        {
            return !useLocID ? Color.yellow : Color.white;
        }
    }
}