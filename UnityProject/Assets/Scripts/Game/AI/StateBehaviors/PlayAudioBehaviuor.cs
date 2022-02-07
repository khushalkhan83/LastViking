using Core.StateMachine;
using Game.Audio;
using UnityEngine;

namespace Game.AI.Behaviours
{
    // cant find usage of this script in project
    public class PlayAudioBehaviuor : BehaviourBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private AudioID _audioID;

#pragma warning restore 0649
        #endregion

        public AudioID Audio => _audioID;

        public AudioSystem AudioSystem => AudioSystem.Instance;

        public float TimeToEnd { get; private set; }

        public override void Begin()
        {
            AudioSystem.CreateAudio(_audioID);

        }

        public override void ForceEnd()
        {
        }

        public override void Refresh()
        {

        }
    }
}
