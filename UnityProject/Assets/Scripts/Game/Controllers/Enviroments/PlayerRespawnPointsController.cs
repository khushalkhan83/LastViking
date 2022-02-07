using System;
using System.Collections;
using Core;
using Core.Controllers;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerRespawnPointsController : IController, IPlayerRespawnPointsController
    {
        [Inject] public PlayerScenesModel PlayerScenesModel { get; private set; }
        [Inject] public SheltersModel SheltersModel { get; set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public LoseViewModel LoseViewModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public PlayerMovementModel PlayerMovementModel { get; private set; }
        [Inject] public PlayerDeathHandler PlayerDeathHandler { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public PlayerRespawnPointsModel PlayerRespawnPointsModel { get; private set; }
        [Inject(true)] public PlayerRespawnPoints PlayerRespawnPoints { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public ShelterAttackModeModel ShelterAttackModeModel { get; private set; }

        public void Start() { }
        public void Enable()
        {
            LoseViewModel.OnPlayAgain += RespawnPlayer;

            PlayerScenesModel.OnEnvironmentLoaded += OnEnvironmentLoaded;
        }
        public void Disable()
        {
            LoseViewModel.OnPlayAgain -= RespawnPlayer;
            PlayerScenesModel.OnEnvironmentLoaded -= OnEnvironmentLoaded;
        }

        private void OnEnvironmentLoaded()
        {
            if(PlayerRespawnPointsModel.RespawnPlayerOnGameStart) RespawnPlayer();
        }

        public void RespawnPlayer()
        {
            PlayerEventHandler.OnPreSaveHandler();
            Transform point = GetRespawnPoint();
            PlayerEventHandler.SetPosition(point.position);
            PlayerEventHandler.SetRotation(point.rotation);

            PlayerEventHandler.StartCoroutine(WaitForRespawn());
        }

        private IEnumerator WaitForRespawn()
        {
            yield return PlayerEventHandler.StartCoroutine(WaitForChunksLoaded());
            PlayerDeathHandler.Respawn(PlayerDeathModel.ImunitetDuration);
            PlayerRespawnPointsModel.PlayerRespawnedAndChunksAreLoaded();
        }

        private IEnumerator WaitForChunksLoaded()
        {
            SECTR_RegionLoader loader = PlayerEventHandler.GetComponentInChildren<SECTR_RegionLoader>();
            
            yield return null;
            if(loader != null)
                yield return new WaitUntil(() => loader.Loaded);
        }
        
        private Transform GetRespawnPoint()
        {
            bool isFirstRespawnInTutorial = !LoseViewModel.HasTutorialResurrected && !TutorialModel.IsComplete;
            bool playerIsInMainLocation = PlayerScenesModel.PlayerIsOnMainLocation;
            bool playerHasShalter = SheltersModel.ShelterActive != ShelterModelID.None;

            if (playerIsInMainLocation && isFirstRespawnInTutorial)
            {
                return PlayerRespawnPoints.InitPlayerPoint;
            }
            else if (playerIsInMainLocation && playerHasShalter && !ShelterAttackModeModel.AttackModeActive)
            {
                return PlayerRespawnPoints.PointShelter;
            }
            else
            {
                return PlayerRespawnPoints.GetClosestRespawnPoint();
            }
        }
    }
}
