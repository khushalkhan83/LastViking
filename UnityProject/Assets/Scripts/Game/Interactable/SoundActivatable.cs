using Game.Audio;
using Game.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Interactables
{
    public class SoundActivatable : Activatable
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private AudioID _audio;

#pragma warning restore 0649
        #endregion

        public AudioSystem AudioSystem => AudioSystem.Instance;

        public AudioID AudioID => _audio;

        public override void OnActivate() => AudioSystem.PlayOnce(AudioID, transform.position);
    }
}
