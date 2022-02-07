using System;
using System.Collections;
using System.Collections.Generic;
using Game.Audio;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class ToggleCampfireController : MonoBehaviour
    {
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private BlueprintObjectsModel BlueprintObjectsModel => ModelsSystem.Instance._blueprintObjectsModel;
        private AudioSystem AudioSystem => AudioSystem.Instance;

        [SerializeField] private ToggleCampFireModel _toggleCampFireModel;
        [SerializeField] private CampFireObjectView _campFireObjectView;
        [SerializeField] private Animator _santaAnimator;
        [SerializeField] private int _blueprintsCount = 5;

        private bool On => _toggleCampFireModel.On;
        private AudioObject AudioCampBurning { get; set; }

        private void OnEnable()
        {
            StorageModel.TryProcessing(_toggleCampFireModel._Data);

            _toggleCampFireModel.OnInteract += OnInteractHandler;
        }
        private void OnDisable()
        {
            _toggleCampFireModel.OnInteract -= OnInteractHandler;
        }
        private void Start() => Toggle();


        private void SpawnBlueprints()
        {
            var spawnPosition = this.transform.position;
            var randomisePosition = 2f;
            BlueprintObjectsModel.SpawnAtPosition(_blueprintsCount, spawnPosition, spawnPosition + Vector3.up, randomisePosition);
        }


        private void OnInteractHandler() => Toggle();
        private void Toggle()
        {
            _santaAnimator.SetBool("FireEnabled", On);
            _campFireObjectView.IsActiveFire(On);

            HandleAudio();

            if (On) HandleSpawn();
        }

        private void HandleSpawn()
        {
            if (_toggleCampFireModel.WasSpawned) return;

            SpawnBlueprints();

            _toggleCampFireModel.WasSpawned = true;
        }

        #region AudioRelated
            
        private void HandleAudio()
        {
            if(On)
            {
                if(AudioCampBurning == null)
                    PlayAudioBurning();
                else ContinuePlayAudioBurning();
            }
            else
                StopAudioBurning();
        }

        private void PlayAudioBurning()
        {
            var campFireAudioContainer = _toggleCampFireModel.transform;

            AudioCampBurning = AudioSystem.CreateAudio(AudioID.Burning);
            AudioCampBurning.AudioSource.transform.SetParent(campFireAudioContainer);
            AudioCampBurning.AudioSource.transform.localPosition = Vector3.zero;
            AudioCampBurning.AudioSource.Play();
        }

        private void ContinuePlayAudioBurning()
        {
            AudioCampBurning?.AudioSource.Play();
        }

        private void StopAudioBurning()
        {
            AudioCampBurning?.AudioSource.Stop();
        }
        #endregion
    }
}