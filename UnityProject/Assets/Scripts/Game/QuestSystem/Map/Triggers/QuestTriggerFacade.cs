using Game.Models;
using Game.QuestSystem.Map.Triggers.TimelineSkipCheck;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;

namespace Game.QuestSystem.Map.Triggers
{
    public class QuestTriggerFacade : MonoBehaviour
    {
        [SerializeField] private QuestTriggerBase questTrigger;
        [SerializeField] private bool useTimeline;
        [ShowIf("useTimeline")]
        [SerializeField] private bool useCinematic = true;
        [ShowIf("useTimeline")]
        [SerializeField] TimelineSkipCheckBase[] skipTimelineChecks;
        [SerializeField] private PlayableDirector playableDirector;

        private CinematicModel CinematicModel => ModelsSystem.Instance._cinematicModel;

        #region MonoBehaviour
        private void OnEnable()
        {
            questTriggerActivated = false;
        }

        private void OnDisable()
        {

        }
            
        #endregion

        private bool questTriggerActivated = false;
        private bool timeLineActivated = false;

        public void ActivateTrigger()
        {
            if(questTriggerActivated) return;

            if(useTimeline && !SkipTimeline())
            {
                ActivateTimeline();
            }
            else
            {
                ActivateQuestTrigger();
            }
        }

        private void ActivateQuestTrigger()
        {
            questTriggerActivated = true;
            questTrigger.ActivateTrigger();
        }

        private void ActivateTimeline()
        {
            playableDirector.Play();
            timeLineActivated = true;

            if(useCinematic)
                CinematicModel.StartCinematic();
        }

        // called by UnityEvent
        public void OnTimelineEnded()
        {
            if(!timeLineActivated) return;

            timeLineActivated = false;

            if(useCinematic)
                CinematicModel.EndCinematic();

            questTrigger.ActivateTrigger();
        }

        private bool SkipTimeline()
        {
            foreach (var timelineSkipCheck in skipTimelineChecks)
            {
                if(timelineSkipCheck.Skip() == true) return true;
            }
            return false;
        }
    }
}