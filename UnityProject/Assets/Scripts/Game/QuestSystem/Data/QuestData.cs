using System.Collections.Generic;
using EnemiesAttack;
using Game.Models;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.QuestSystem.Data
{
    [CreateAssetMenu(fileName = "SO_Quest_new", menuName = "Quests/QuestData", order = 0)]
    public class QuestData : ScriptableObject
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private QuestData nextQuest;
        [SerializeField] private int targetShipLevel;
        // [SerializeField] private bool skeletonAttackOnUpgrade;
        [SerializeField] private EnemiesAttackConfig enemiesAttackConfig;
        [SerializeField] private QuestItemData questItemData;
        
        #pragma warning restore 0649
        #endregion
        
        private static QuestsModel questsModel;
        private static QuestsModel QuestsModel {get => questsModel ?? (questsModel = FindObjectOfType<ModelsSystem>()._questsModel);}

        public QuestData NextQuest => nextQuest;
        public EnemiesAttackConfig EnemiesAttackConfig => enemiesAttackConfig;
        public int TargetShipLevel => targetShipLevel;
        // public bool SkeletonAttackOnUpgrade => skeletonAttackOnUpgrade;
        public QuestItemData QuestItemData => questItemData;

        [TableList]
        public List<StageData> stages = new List<StageData>();

        [Button]
        [GUIColor("ButtonColor")]
        private void ActivateQuest()
        {
            QuestsModel.ActivateQuest(this);
        }
        [Button]
        private bool Validate()
        {
            bool answer = true;
            foreach (var stage in stages)
            {
                bool valid = stage.Validate();
                if(!valid) answer = false;
            }
            return answer;
        }


        private Color ButtonColor()
        {
            bool active = QuestsModel.ActiveQuest == this;

            return active ? Color.green : Color.yellow;
        }
    }
}