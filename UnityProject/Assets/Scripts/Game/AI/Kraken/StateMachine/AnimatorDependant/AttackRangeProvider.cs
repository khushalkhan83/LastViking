using Game.AI.Behaviours.Kraken.StageMachine;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.AI.Behaviours.Kraken.AnimatorDependant
{
    public class AttackRangeProvider : MonoBehaviour, IAttackRangeProvider
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private float _range = 5;
        [SerializeField] private bool _isDrawGizmos = true;
        [SerializeField] private Color _gizmosColor = Color.green;

#pragma warning restore 0649
        #endregion


        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
        private float _aproximetDistance;

        bool IAttackRangeProvider.PlayerIsInrange()
        {
            _aproximetDistance = (PlayerEventHandler.transform.position - transform.position).sqrMagnitude;
            bool playerIsInRange = _aproximetDistance < _range * _range;
            return playerIsInRange;
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            if (_isDrawGizmos)
            {
                Gizmos.color = _gizmosColor;
                Gizmos.DrawSphere(transform.position, _range);
                Gizmos.DrawWireSphere(transform.position, _range);
            }
        }
#endif
    }
}

