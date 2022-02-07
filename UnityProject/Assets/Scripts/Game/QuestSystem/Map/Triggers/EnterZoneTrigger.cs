using System;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.QuestSystem.Map.Triggers
{
    public class EnterZoneTrigger : QuestTriggerOld
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.name != "Player") return;
            
            if(UseTimeline)
            {
                PlayTimeline();
            }
            else
            {
                ActivateTrigger();
            }
        }
    }
}