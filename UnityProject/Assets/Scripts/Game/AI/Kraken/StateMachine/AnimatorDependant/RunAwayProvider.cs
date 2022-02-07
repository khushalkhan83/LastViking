using Game.AI.Behaviours.Kraken.StageMachine;
using Game.Models;
using UnityEngine;

namespace Game.AI.Behaviours.Kraken.AnimatorDependant
{
    public class RunAwayProvider : MonoBehaviour, IRunAwayProvider
    {
        private FirstKrakenModel FirstKrakenModel => ModelsSystem.Instance._firstKrakenModel;
        private bool RunAway => FirstKrakenModel.RunAway;
        private float HealthToRunAway => FirstKrakenModel.HealthToRunAway;

        #region Data
#pragma warning disable 0649
        [SerializeField] private IHealth _health;

#pragma warning restore 0649
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            _health = GetComponentInChildren<IHealth>();
        }
        #endregion

        public bool IsRunAway()
        {
            if (RunAway)
            {
                bool healthToRunAway = _health.Health <= HealthToRunAway;
                return healthToRunAway;
            }
            else return false;
        }
    }

}