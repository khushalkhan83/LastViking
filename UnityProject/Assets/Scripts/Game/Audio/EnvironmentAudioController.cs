using Game.Models;
using UnityEngine;
using Extensions;

namespace Game.Audio
{
    public class EnvironmentAudioController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private EnvironmentOnceAudioThread _environmentOnceAudioThread;
        [SerializeField] private ColliderTriggerModel _colliderTriggerModel;
        [SerializeField] private AudioID daySound; 
        [SerializeField] private AudioID nightSound; 
        [SerializeField] private LayerMask _playerLayer;

#pragma warning restore 0649
        #endregion

        private EnvironmentOnceAudioThread EnvironmentOnceAudioThread
        {
            get
            {
                if(_environmentOnceAudioThread == null) _environmentOnceAudioThread = GetComponent<EnvironmentOnceAudioThread>();
                return _environmentOnceAudioThread;
            }
        }
        private GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;

        private EnvironmentTimeModel EnvironmentTimeModel => ModelsSystem.Instance._environmentTimeModel;
        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;

        private bool IsPlayer(Collider collider) => collider.gameObject.InsideLayerMask(_playerLayer);

        #region MonoBehaviour

        private void OnEnable()
        {
            _colliderTriggerModel.OnEnteredTrigger += OnEnteredTrigger;
            _colliderTriggerModel.OnExitedTrigger += OnExitedTrigger;
        }

        private void OnDisable()
        {
            _colliderTriggerModel.OnEnteredTrigger -= OnEnteredTrigger;
            _colliderTriggerModel.OnExitedTrigger -= OnExitedTrigger;
            EnvironmentTimeModel.OnDayTimeChanged -= OnDayTimeChangedHandler;
        }
            
        #endregion

        private void OnEnteredTrigger(Collider other)
        {
            if(!IsPlayer(other)) return;

            PlayCurrentAudio();
            EnvironmentTimeModel.OnDayTimeChanged += OnDayTimeChangedHandler;
        }

        private void OnExitedTrigger(Collider other)
        {
            if(!IsPlayer(other)) return;
            
            EnvironmentOnceAudioThread.StopAudio();
            EnvironmentTimeModel.OnDayTimeChanged -= OnDayTimeChangedHandler;
        }

        private void OnDayTimeChangedHandler()
        {
            PlayCurrentAudio();
        }

        private void PlayCurrentAudio()
        {
            var audioId = EnvironmentTimeModel.IsDayTime ? daySound : nightSound;
            EnvironmentOnceAudioThread.PlayOnce(audioId);
        }
    }
}
