using System;
using Game.Models;
using NaughtyAttributes;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Game.AI.Behaviours.Kraken
{
    public class KrakenLeg : MonoBehaviour, IKrakenConfigurable
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private ColliderDamager _colliderDamager;
        [SerializeField] private PlayableDirector _playableDirector;
        [SerializeField] private SimpleAnimation _simpleAnimation;

        [Space]
        [SerializeField] private KrakenConfig _config;

        [Space]
        [SerializeField] private UnityEvent _onRecivedDamage;
        
        #pragma warning restore 0649
        #endregion

        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;

        public Action OnKrakenSmash;
        public Action OnKrakenAttackFinished;
        public Action OnRecivedDamage;


        private void OnEnable()
        {
            _colliderDamager.OnTargetDamaged += OnDamagedHandler;
        }
        private void OnDisable()
        {
            _colliderDamager.OnTargetDamaged -= OnDamagedHandler;
        }

        public void SetDamagerActive(bool active) => _colliderDamager.SetActive(active);

        [Button] public void Attack() => _playableDirector.Play();
        [Button] public void ReadyToAttack() => _simpleAnimation.Play();
        [Button] public void PauseMovement() => _playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
        [Button] public void ResumeMovement() => _playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
        [Button] public void TestRecivedDamage() => RecivedDamage();


        // called from signal reciver
        public void KrakenSmashLeg() => OnKrakenSmash?.Invoke();
        // called from signal reciver
        public void KrakenAttackFinished() => OnKrakenAttackFinished?.Invoke();

        // called from signal reciver
        public void RecivedDamage()
        {
            OnRecivedDamage?.Invoke();
            _onRecivedDamage?.Invoke();
        }

        public void OnDamagedHandler(Target target, Collider damager)
        {
            var direction = PlayerEventHandler.transform.position - damager.bounds.center;
            PlayerEventHandler.AddImpact(direction.normalized,_config.ThrowForce);
        }

        #region IKrakenConfigurable
        public void Configurate(KrakenConfig config)
        {
            _config = config;
        }
        #endregion
    }
}
