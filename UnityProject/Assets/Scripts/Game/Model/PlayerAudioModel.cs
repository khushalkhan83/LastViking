using Game.Audio;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    public class PlayerAudioModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private AudioID[] _audioID;

#pragma warning restore 0649
        #endregion

        public IEnumerable<AudioID> AudioID => _audioID;

        public AudioID AudioIDDamageRandomly => _audioID[Random.Range(0, _audioID.Length)];
    }
}
