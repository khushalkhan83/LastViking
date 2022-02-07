using System;
using NaughtyAttributes;
using SOArchitecture;
using UnityEngine;
using UnityEngine.Playables;

namespace Game.QuestSystem.Map.Triggers
{
    public abstract class QuestTriggerOld : QuestTrigger
    {
        [SerializeField] private bool useTimeline;
        [ShowIf("useTimeline")]
        [SerializeField] private PlayableDirector playableDirector;
        protected bool UseTimeline => useTimeline;
        protected void PlayTimeline() => playableDirector.Play();
    }
}