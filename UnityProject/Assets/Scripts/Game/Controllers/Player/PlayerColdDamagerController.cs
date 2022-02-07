using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerColdDamagerController : IPlayerColdDamagerController, IController
    {
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerAudioModel PlayerAudioModel { get; private set; }
        [Inject] public PlayerColdDamagerModel PlayerColdDamagerModel { get; private set; }
        [Inject] public PlayerWarmModel PlayerWarmModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }

        protected Coroutine CColdDamageProcess { get; private set; }

        private int _coldDamageProcessCoRoutineId = -1;

        void IController.Enable()
        {
            StorageModel.TryProcessing(PlayerColdDamagerModel._Data);

            PlayerWarmModel.OnStartColding += OnPlayerStartColding;
            PlayerWarmModel.OnStopColding += OnPlayerStopColding;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            PlayerWarmModel.OnStartColding -= OnPlayerStartColding;
            PlayerWarmModel.OnStopColding -= OnPlayerStopColding;

            EndColdDamageProcess();
        }

        private void OnPlayerStartColding() => BeginColdDamageProcess();

        private void OnPlayerStopColding() => EndColdDamageProcess();

        private void BeginColdDamageProcess()
        {
            if (CoroutineModel.GetCoroutine(_coldDamageProcessCoRoutineId) == null)
            {
                _coldDamageProcessCoRoutineId = CoroutineModel.InitCoroutine(ColdDamageProcess());
            }
        }

        private void EndColdDamageProcess()
        {
            CoroutineModel.BreakeCoroutine(_coldDamageProcessCoRoutineId);
            PlayerColdDamagerModel.SetRemainingTimeHealthDecrease(PlayerColdDamagerModel.TimeHealthDecrease);
        }

        private IEnumerator ColdDamageProcess()
        {
            do
            {
                while (PlayerColdDamagerModel.RemainingTimeHealthDecrease > 0)
                {
                    yield return null;
                    PlayerColdDamagerModel.SetRemainingTimeHealthDecrease(PlayerColdDamagerModel.RemainingTimeHealthDecrease - Time.deltaTime);
                }

                AudioSystem.PlayOnce(PlayerAudioModel.AudioIDDamageRandomly);

                PlayerHealthModel.AdjustHealth(-PlayerColdDamagerModel.HealthAdjustment);

                PlayerColdDamagerModel.SetRemainingTimeHealthDecrease(PlayerColdDamagerModel.TimeHealthDecrease);
                PlayerColdDamagerModel.HealthDecrease();

            } while (true);
        }
    }
}
