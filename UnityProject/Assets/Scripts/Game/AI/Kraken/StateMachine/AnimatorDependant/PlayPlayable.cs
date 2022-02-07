using System.Collections.Generic;
using Game.Models;
using SOArchitecture;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Game.AI.Behaviours.Kraken.StageMachine.AnimatorDependant
{
    public class PlayPlayable : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private PlayableDirector _playable;
        [SerializeField] private Animator _animator;
        [SerializeField] private string _targetBoolParamName;
        [SerializeField] private bool _targetValue = true;

        [SerializeField] private List<PlayableDirector> _playablesToDisable;
        [SerializeField] private UnityEvent _onBefoureBeginPlay;
        [SerializeField] private bool _behaviourActivated;

        [SerializeField] private GameEvent _gameEventPlayableStrted;

#pragma warning restore 0649
        #endregion

        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;

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
            if (_behaviourActivated) return;
            if (_animator.GetBool(_targetBoolParamName) != _targetValue) return;
            _behaviourActivated = true;

            Begin();
        }

        public void Begin()
        {
            _onBefoureBeginPlay?.Invoke();
            _playable.Play();
            _playablesToDisable.ForEach(x => x.enabled = false);
            _gameEventPlayableStrted?.Raise();
        }
    }
}