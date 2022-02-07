using System;
using Game.Models;
using UltimateSurvival;
using UnityEngine;
using Updaters;

namespace Encounters
{
    public interface IDistanceFromPlayerChecker
    {
        event Action OnFarAwayFromPlayer;
    }

    public class DistanceFromPlayerChecker : MonoBehaviour, IDistanceFromPlayerChecker
    {
        #region Dependencies
        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;

        private const int k_updateRate = 2;
        private const int k_firstUpdateDelay = 30;
        private const float maxDistance = 30;
        #endregion

        private IUpdater updater;

        public event Action OnFarAwayFromPlayer;

        #region MonoBehaviour

        private void OnEnable() => updater = new DelayedUpdater(k_updateRate, TryDespawn,k_firstUpdateDelay);
        private void Update() => updater.Tick();
        private void OnDisable() => this.enabled = false;

        #endregion

        private void TryDespawn()
        {
            if (IsFarAwayFromPlayer())
            {
                OnFarAwayFromPlayer?.Invoke();
            }
        }

        private bool IsFarAwayFromPlayer()
        {
            var distance = PlayerEventHandler.transform.position - transform.position;
            bool farAway = distance.sqrMagnitude > maxDistance * maxDistance;

            return farAway;
        }
    }
}