using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Game.Audio;
using UnityEngine;


namespace Game.AI.BehaviorDesigner
{
    [System.Serializable]
    public class SharedAudioId : SharedVariable<AudioID>
    {
        public static implicit operator SharedAudioId(AudioID value) { return new SharedAudioId { Value = value }; }
    }

    [System.Serializable]
    public class SharedAudioIdList : SharedVariable<List<AudioID>>
    {
        public static implicit operator SharedAudioIdList(List<AudioID> value) { return new SharedAudioIdList { Value = value }; }
    }
}