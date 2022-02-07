using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UnityEngine;
using UnityEngine.Playables;

namespace Game.Interactables
{
    public class PlayableActivatable : Activatable
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private PlayableDirector playableDirector;

#pragma warning restore 0649
        #endregion

        private CinematicModel CinematicModel => ModelsSystem.Instance._cinematicModel;

        public override void OnActivate()
        {
            playableDirector.playOnAwake = true;
            playableDirector.enabled = true;
            playableDirector.Play();
        }


        // called from SignalReciver Component
        public void BlockPlayer()
        {
            CinematicModel.StartCinematic();
        }
         // called from SignalReciver Component
        public void UnBlockPlayer()
        {
            CinematicModel.EndCinematic();
        }
    }
}