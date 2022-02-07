using System;
using Game.Models;
using Game.Objectives;
using SOArchitecture;
using UltimateSurvival;
using UnityEngine;

namespace Game.QuestSystem.Map.Triggers
{
    // [RequireComponent(typeof(GameEventListener))]
    public class EventTrigger : QuestTriggerOld
    {
        // [SerializeField] private  GameEventListener listener;

        public new void ActivateTrigger()
        {
            if(UseTimeline)
                PlayTimeline();
            else
                base.ActivateTrigger();
        } 

        public void TimeLineEnded()
        {
            base.ActivateTrigger();
        }




        // Editor only
        public void SetEvent(GameEvent gameEvent)
        {
            // listener.SetEvent(gameEvent);
        }
    }
}