using System;
using Game.QuestSystem.Data;
using Game.QuestSystem.Map.Triggers;
using UnityEngine;


namespace Game.Models
{
    [CreateAssetMenu(fileName = "ML_QuestTriggers", menuName = "Model/QuestTriggers", order = 0)]
    public class QuestTriggersModel : ScriptableObject
    {
        public event Action<QuestTrigger> OnActivateTriggerNew;

        public void ActivateTrigger(QuestTrigger questTrigger)
        {
            OnActivateTriggerNew?.Invoke(questTrigger);
        }
    }
}