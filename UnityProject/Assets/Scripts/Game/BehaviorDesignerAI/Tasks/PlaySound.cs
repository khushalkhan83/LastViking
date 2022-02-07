using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.Audio;
using RoboRyanTron.SearchableEnum;
using UnityEngine;

namespace Game.AI.BehaviorDesigner
{
    public class PlaySound : Action
    {
        [SerializeField] private SharedAudioId _audioID = default;
        [SerializeField] private bool _random = default;
        [SerializeField] private SharedAudioIdList _randomAudioIDs = default;


        private AudioSystem AudioSystem => AudioSystem.Instance;


        public override void OnStart()
        {
            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            AudioID audioID = _random && _randomAudioIDs.Value.Count > 0? _randomAudioIDs.Value[Random.Range(0, _randomAudioIDs.Value.Count)] : _audioID.Value;
            if(audioID != AudioID.None)
            {
                AudioSystem.PlayOnce(audioID, transform.position);
            }
            return TaskStatus.Success;
        }
    }
}
