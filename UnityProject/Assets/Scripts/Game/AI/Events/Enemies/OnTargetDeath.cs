using Game.AI;
using Game.Models;
using Game.StateMachine.Events;
using UnityEngine;

namespace Game.AI.Events.Enemies
{
    public class OnTargetDeath : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Hit _hit;
        [SerializeField] private EnemyID _enemyID;

#pragma warning restore 0649
        #endregion

        public Hit Hit => _hit;
        public EnemyID EnemyID => _enemyID;
        public EnemiesModel EnemiesModel => ModelsSystem.Instance._enemiesModel;

        private void OnEnable()
        {
            Hit.OnKillTarget += OnKillTarget;
        }

        private void OnDisable()
        {
            Hit.OnKillTarget -= OnKillTarget;
        }

        private void OnKillTarget(Target target) => EnemiesModel.EnemyKillTarget(EnemyID, target);
    }

}
