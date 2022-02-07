using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using UnityEngine;
using System;
using UltimateSurvival;
using Game.Audio;
using RoboRyanTron.SearchableEnum;

namespace Game.AI.BehaviorDesigner
{
    public class AttackRange : MonoBehaviour
    {
        public event Action<Target> OnKillTarget;

        #region Data
    #pragma warning disable 0649

        [SerializeField] BehaviorTree _behaviourTree;
        [SerializeField] private ShaftedProjectile _arrowPrefab;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private Vector3 _playerShift;

        [SearchableEnum]
        [SerializeField] private AudioID[] _audioIDs;

    #pragma warning restore 0649
        #endregion

        private PlayerHealthModel PlayerHealthModel => ModelsSystem.Instance._playerHealthModel;
        private SharedGameObject TargetVariable { get => targetVariable ?? (SharedGameObject)_behaviourTree.GetVariable("Target");}
        private AudioSystem AudioSystem => AudioSystem.Instance;


        private SharedGameObject targetVariable;

        //from animation event
        void OnAnimationDamageRange()
        {
            if ( TargetVariable.Value == null) return;

            Target target =  TargetVariable.Value.GetComponent<Target>();
            if (target == null) return;

            PlayAttackSound();

            var rotation = Quaternion.LookRotation((target.transform.position + _playerShift) - _shootPoint.position);
            ShaftedProjectile projectile = Instantiate(_arrowPrefab, _shootPoint.position, rotation);
            projectile.Launch(null, 0);
            projectile.OnKillTarget += KillTarget;
        }

        private void KillTarget(Target deadTarget)
        {
            Target target =  TargetVariable.Value.GetComponent<Target>();
            if(target != null && target == deadTarget)
            {
                _behaviourTree.SendEvent("KillTarget");
                OnKillTarget?.Invoke(target);
            }
        }

        private void PlayAttackSound()
        {
            AudioID attackSound = GetRandomSound();
            if(attackSound != AudioID.None)
            {
                AudioSystem.PlayOnce(GetRandomSound(), transform.position);
            }
        }

        private AudioID GetRandomSound()
        { 
            if(_audioIDs.Length > 0)
                return _audioIDs[UnityEngine.Random.Range(0, _audioIDs.Length)];
            else
                return AudioID.None;
        }

    }
}
