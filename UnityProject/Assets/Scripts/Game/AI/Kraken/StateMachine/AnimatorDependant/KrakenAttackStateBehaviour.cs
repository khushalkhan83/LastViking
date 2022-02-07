using Extensions;
using Game.Misc;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.AI.Behaviours.Kraken.StageMachine.AnimatorDependant
{
    public class KrakenAttackStateBehaviour : MonoBehaviour, IKrakenConfigurable
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private KrakenConfig _config;

        [Space]
        [SerializeField] private string _canAttack;
        [SerializeField] private Animator _animator;
        [SerializeField] private KrakenLeg[] _legs;
        [SerializeField] private float _calibrationAngle;
        [SerializeField] private float _errorAngle; // used to make WarningSmash
        [SerializeField] private GenericShake _genericShake;
        [SerializeField] private KrakenLeg _lastActiveLeg;

        [SerializeField] private EnemyHealthModel _krakenHealth;
        [SerializeField] private LookAtPlayer _lookAtPlayer;

#pragma warning restore 0649
        #endregion

        private Transform _target => ModelsSystem.Instance._playerEventHandler.transform;
        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;

        private IHealth KrakenHealth => _krakenHealth;


        #region Based on Settings

        private float HoldLegTime => _config.HoldLegOnGroundTime;
        private float DamageAmuntToResumeLegMovement => _config.DamageToCancelHoldLegOnGround;
        private float ShakeScale => _config.ShakeScale;
        private bool UseWarningSmash => _config.UseWarningSmash;
        private int WarningSmashesCount => _config.WarningSmashesCount;

        #endregion

        private float _healthBefoureAttack;
        private float _damageRecived;

        private float _holdLegTimeLeft;
        private bool _atackBehaviourStartedAndActive;

        private bool _movementIsStoped;
        private bool _enoughDamageToResumeLegMovement;
        private int _smashesCount;

        private bool _stateWasActive;
        private bool _init = true;

        #region MonoBehaviour

        private void OnEnable()
        {
            GameUpdateModel.OnUpdate += OnUpdate;

            KrakenHealth.OnChangeHealth += OnChangeHealthHandler;

            for (int i = 0; i < _legs.Length; i++)
            {
                var leg = _legs[i];
                leg.OnKrakenSmash += OnKrakenSmash;
                leg.OnKrakenAttackFinished += OnKrakenAttackFinished;
            }
        }
        private void OnDisable()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;

            for (int i = 0; i < _legs.Length; i++)
            {
                var leg = _legs[i];
                leg.OnKrakenSmash -= OnKrakenSmash;
                leg.OnKrakenAttackFinished -= OnKrakenAttackFinished;
                KrakenHealth.OnChangeHealth -= OnChangeHealthHandler;
            }
        }

        #endregion

        #region UpdateLoop

        private void OnUpdate()
        {
            bool stateIsActive = _animator.GetBool(_canAttack);

            if (stateIsActive != _stateWasActive || _init)
            {
                OnStateActiveChanged(stateIsActive);
                _init = false;
            }

            _stateWasActive = stateIsActive;

            if (!stateIsActive) return;

            if (_atackBehaviourStartedAndActive) Refresh();
            else BeginAttack();
        }

        private void OnStateActiveChanged(bool active)
        {
            _lookAtPlayer.enabled = active;
        }


        private void Refresh()
        {
            if (_movementIsStoped)
            {
                _holdLegTimeLeft -= Time.deltaTime;
                bool timeToResumeMovement = _holdLegTimeLeft <= 0;

                if (timeToResumeMovement || _enoughDamageToResumeLegMovement)
                {
                    ResumeLegMovement();
                }
            }
        }

        #endregion

        #region States

        // stage 1 (start)
        private void BeginAttack()
        {
            // if(attackedOnce) return;
            // attackedOnce = true;

            var leg = _legs.RandomElement();
            RotateLeg(leg.gameObject);
            leg.SetDamagerActive(true);
            leg.Attack();
            _holdLegTimeLeft = HoldLegTime;

            _lastActiveLeg = leg;

            _atackBehaviourStartedAndActive = true;
            ResetRecivedDamage();
        }

        // stage 2
        private void OnKrakenSmash()
        {
            _genericShake.Shake(ShakeScale);
            _lastActiveLeg.PauseMovement();
            _movementIsStoped = true;
            _smashesCount++;
        }

        // if _enoughDamageToResumeLegMovement then stage 3 can be selected without waiting 
        private void OnChangeHealthHandler()
        {
            _damageRecived = _healthBefoureAttack - KrakenHealth.Health;
            _enoughDamageToResumeLegMovement = _damageRecived >= DamageAmuntToResumeLegMovement;
        }

        // stage 3
        private void ResumeLegMovement()
        {
            if (_lastActiveLeg == null) return;

            _lastActiveLeg.ResumeMovement();
            _movementIsStoped = false;
        }

        // stage 4 (end)
        private void OnKrakenAttackFinished()
        {
            ResetAttackLeg();
            _atackBehaviourStartedAndActive = false;
        }

        #endregion

        #region Other methods

        private void ResetAttackLeg()
        {
            if (_lastActiveLeg != null)
            {
                _lastActiveLeg.ReadyToAttack();
                _lastActiveLeg.SetDamagerActive(false);

                _lastActiveLeg = null;
            }
        }

        private void ResetRecivedDamage()
        {
            _healthBefoureAttack = KrakenHealth.Health;
            _damageRecived = 0;
            _enoughDamageToResumeLegMovement = false;
        }

        private void RotateLeg(GameObject leg)
        {
            leg.transform.LookAt(_target.transform, Vector3.up);
            // if (_calibrationAngle == 0) return;

            var rotation = leg.transform.rotation.eulerAngles;
            var angleY = rotation.y + _calibrationAngle + (IsWarningSmash() ? _errorAngle : 0);
            leg.transform.rotation = Quaternion.Euler(rotation.x, angleY, rotation.z);
        }

        // at first we want to smash not on player, but close to him
        private bool IsWarningSmash()
        {
            if (!UseWarningSmash) return false;

            var warning = _smashesCount < WarningSmashesCount;
            return warning;
        }

        public void Configurate(KrakenConfig config)
        {
            _config = config;
        }
        #endregion

    }
}
