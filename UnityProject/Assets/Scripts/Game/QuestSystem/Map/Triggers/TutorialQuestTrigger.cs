using Game.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Game.QuestSystem.Map.Triggers
{
    public class TutorialQuestTrigger : QuestTriggerBase
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private UnityEvent onActivateTrigger;
        
        #pragma warning restore 0649
        #endregion

        public override void ActivateTrigger()
        {
            onActivateTrigger?.Invoke();
        }
    }
}