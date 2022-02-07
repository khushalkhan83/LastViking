using System.Collections;
using Game.Models;
using Game.QuestSystem.Data;
using UnityEngine;

namespace Game.QuestSystem.Map.Extra
{
    public class SideQuestActivator : MonoBehaviour
    {
        [SerializeField] private QuestData sideQuest;

        private SideQuestsModel SideQuestsModel => ModelsSystem.Instance._sideQuestsModel;
        
        public void TryActivate()
        {
            if(SideQuestsModel.IsQuestStateNotActive(sideQuest))
            {
                SideQuestsModel.ActivateQuest(sideQuest);
            }
        }
    }
}