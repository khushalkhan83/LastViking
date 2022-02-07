using System;
using System.Collections.Generic;
using Game.Models;
using Game.QuestSystem.Data;
using UnityEngine;


namespace Game.QuestSystem.Map.Extra.Environment
{
    public class QuestDoor : MonoBehaviour
    {
        [Serializable]
        public class QuestSettings
        {
            public QuestData quest;
            public List<int> stagesToApplyLogic;
        }

        #region Data
#pragma warning disable 0649
        [SerializeField] private QuestSettings settings = default;
        [SerializeField] private Collider collider;

#pragma warning restore 0649
        #endregion

        private QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;

        #region MonoBehaviour

        private void OnEnable()
        {
            QuestsModel.OnActivateQuest += UpdateDoor;
            QuestsModel.OnActivateStage += UpdateDoor;

            UpdateDoor();
        }

        private void OnDisable()
        {
            QuestsModel.OnActivateQuest -= UpdateDoor;
            QuestsModel.OnActivateStage -= UpdateDoor;
        }

        #endregion

        private void UpdateDoor()
        {
            bool targetQuestIsActive = QuestsModel.ActiveQuest == settings.quest;
            bool targetStageIsActive = settings.stagesToApplyLogic.Contains(QuestsModel.ActiveStage);
            
            bool disableDoor = targetQuestIsActive && targetStageIsActive;
            SetDoorInteractable(!disableDoor);
        }

        private void SetDoorInteractable(bool interactable)
        {
            collider.enabled = interactable;
        }
    }
}