using System;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Misc
{
    public class LookAtPlayer : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private Transform _target;
        
        #pragma warning restore 0649
        #endregion

        private Vector3 _targetLookPosition;

        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;
        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;

        #region MonoBehaviour

        private void OnEnable()
        {
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        private void OnDisable()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        #endregion

        private void OnUpdate()
        {
            Rotate();
        }

        private void Rotate()
        {
            _targetLookPosition = new Vector3(PlayerEventHandler.transform.position.x,_target.position.y, PlayerEventHandler.transform.position.z);
            _target.LookAt(_targetLookPosition);
        }
    }
}
