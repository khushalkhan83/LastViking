using Core;
using Core.Controllers;
using EnemiesAttack;
using Extensions;
using Game.Audio;
using Game.Models;
using Game.Views;
using Game.VillageBuilding;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Game.Controllers
{
    public class VillageAttackController : IVillageAttackController, IController
    {
        [Inject] public VillageAttackModel VillageAttackModel {get; private set;}
        [Inject] public EnemiesSpawnModel EnemiesSpawnModel {get; private set;}
        [Inject] public VillageBuildingModel VillageBuildingModel {get; private set;}
        [Inject] public LocalizationModel LocalizationModel {get; private set;}
        [Inject] public NotificationContainerViewModel NotificationContainerViewModel {get; private set;}
        [Inject] public ViewsSystem ViewsSystem {get; private set;}
        [Inject] public CoroutineModel CoroutineModel {get; private set;}
        [Inject] public PlayerScenesModel PlayerScenesModel { get; private set; }
        [Inject] public AudioSystem AudioSystem {get; private set;}
        [Inject] public PlayerDeathModel PlayerDeathModel {get; private set;}


        private ObjectiveAttackProcessView attackProcessView;
        private bool attackModeStarted;
        private EnemiesAttackConfig config;
        private int corutineIndex = -1;

        private HouseBuilding Townhall => VillageBuildingModel.Townhall;
            
        void IController.Enable() 
        {
            PlayerScenesModel.OnEnvironmentLoaded += OnEnvironmentLoaded;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            PlayerScenesModel.OnEnvironmentLoaded -= OnEnvironmentLoaded;
            VillageAttackModel.OnStartTownHallUpgrade -= StartTownHallUpgrade;
            VillageAttackModel.OnWaveComplete -= OnWaveComplete;
            EnemiesSpawnModel.OnAllEnemiesDestroyed -= OnAllEnemiesDestroyed;
            EnemiesSpawnModel.OnEnemyDestroyed -= OnEnemyDestroyed;
            PlayerDeathModel.OnPreRevival -= OnPreRevival;
            PlayerDeathModel.OnRevivalPrelim -= OnRevivalPrelim;
            Townhall.GetComponentInChildren<BuildingHealthModel>().OnDeath -= OnTownHallDestroy;
            Townhall.OnCompleteBuildingProcess -= OnCompleteBuildingProcess;
            CoroutineModel.BreakeCoroutine(corutineIndex);
        }

        private void OnEnvironmentLoaded()
        {
            PlayerScenesModel.OnEnvironmentLoaded -= OnEnvironmentLoaded;
            Init();
        }

        private void Init()
        {
            VillageAttackModel.OnStartTownHallUpgrade += StartTownHallUpgrade;
            VillageAttackModel.OnWaveComplete += OnWaveComplete;
            EnemiesSpawnModel.OnAllEnemiesDestroyed += OnAllEnemiesDestroyed;
            EnemiesSpawnModel.OnEnemyDestroyed += OnEnemyDestroyed;
            PlayerDeathModel.OnPreRevival += OnPreRevival;
            PlayerDeathModel.OnRevivalPrelim += OnRevivalPrelim;
            if(VillageAttackModel.AttackModeActive)
            {
                ResumeTownHallUpgrade();
            }
            else if(!VillageAttackModel.AttackModeActive && Townhall.IsBuildingProcess)
            {
                Townhall.BreakBuildingProcess();
            }
        }

        private void StartTownHallUpgrade()
        {
            if(VillageAttackModel.HasEnemiesAttackConfigForLevel(Townhall.Level))
            {
                if(attackModeStarted) return;
                attackModeStarted = true;

                config = VillageAttackModel.GetEnemiesAttackConfig(Townhall.Level);
                VillageAttackModel.AttackModeActive = true;
                VillageAttackModel.WaveIndex = 0;
                var healthModel = Townhall.GetComponentInChildren<BuildingHealthModel>();
                healthModel.SetHealth(healthModel.HealthMax);
                Townhall.StartBuildingProcess(false);
                EnemiesSpawnModel.StartSpawnSession(config);
                VillageAttackModel.EnemiesTotal = config.EnemiesTotal;
                VillageAttackModel.EnemiesLeft = config.GetEnemiesLeft(VillageAttackModel.WaveIndex);
                AttackModeStartUI();
                healthModel.OnDeath += OnTownHallDestroy;
                Townhall.OnCompleteBuildingProcess += OnCompleteBuildingProcess;
            }
            else
            {
                Townhall.StartBuildingProcess();
            }
        }

        private void ResumeTownHallUpgrade()
        {
            if(VillageAttackModel.HasEnemiesAttackConfigForLevel(Townhall.Level) && Townhall.IsBuildingProcess)
            {
                if(attackModeStarted) return;
                attackModeStarted = true;

                config = VillageAttackModel.GetEnemiesAttackConfig(Townhall.Level);
                EnemiesSpawnModel.StartSpawnSession(config);
                VillageAttackModel.EnemiesTotal = config.EnemiesTotal;
                VillageAttackModel.EnemiesLeft = config.GetEnemiesLeft(VillageAttackModel.WaveIndex);
                Townhall.GetComponentInChildren<BuildingHealthModel>().OnDeath += OnTownHallDestroy;
                Townhall.OnCompleteBuildingProcess += OnCompleteBuildingProcess;
                AttackModeStartUI();
            }
            else
            {
                VillageAttackModel.AttackModeActive = false;
                Townhall.BreakBuildingProcess();
            }
        }

        private void OnEnemyDestroyed(DeathController enemy)
        {
            VillageAttackModel.EnemiesLeft--;
        }

        private void OnAllEnemiesDestroyed()
        {
            AttackFinish();
        }

        private void OnTownHallDestroy()
        {
            AttackFailure();
        }

        private void AttackFinish()
        {
            attackModeStarted = false;
            VillageAttackModel.AttackModeActive = false;
            AttackModeFinishedUI();
            Townhall.OnCompleteBuildingProcess -= OnCompleteBuildingProcess;
            Townhall.CompleteBuildingProcess();
            Townhall.GetComponentInChildren<BuildingHealthModel>().OnDeath -= OnTownHallDestroy;

            VillageAttackModel.TownHallUpgradeCompleated();
        }

        private void AttackFailure()
        {
            attackModeStarted = false;
            VillageAttackModel.AttackModeActive = false;
            AttackModeFaildedUI();
            Townhall.OnCompleteBuildingProcess -= OnCompleteBuildingProcess;
            Townhall.BreakBuildingProcess();
            EnemiesSpawnModel.StopSpawnSession();
            Townhall.GetComponentInChildren<BuildingHealthModel>().OnDeath -= OnTownHallDestroy;
            VillageAttackModel.TownHallUpgradeFailed();
        }

        private void OnPreRevival() => UpdateViews();
        private void OnRevivalPrelim() => UpdateViews();

        private void UpdateViews()
        {
            if(!attackModeStarted) return;

            AttackModeInProgressUI();
        }

        private void OnWaveComplete()
        {
            int wavesCount = config.Waves.Count();
            bool noWavesLeft = VillageAttackModel.WaveIndex >= wavesCount;

            if(noWavesLeft) return;

            OnWaveCompleteUI();
        }

        private void OnCompleteBuildingProcess(HouseBuilding houseBuilding)
        {
            if(VillageAttackModel.AttackModeActive)
            {
                EnemiesSpawnModel.StopSpawnSession();
                AttackFinish();
            }
        }

        private void AttackModeStartUI()
        {
            float timeToDisplay = 3;
            string messageText = LocalizationModel.GetString(LocalizationKeyID.InGameNotif_ShipAttackStart);
            NotificationContainerViewModel.Show(PriorityID.Wave, ViewConfigID.AttackSessionCompleted, new AttackSessionCompletedControllerData(messageText, timeToDisplay));
            
            AttackModeInProgressUI(timeToDisplay);
        }

        private void AttackModeInProgressUI(float doAfterSeconds = 0)
        {
            corutineIndex = CoroutineModel.InitCoroutine(DoAfterSeconds(doAfterSeconds, () => {
                attackProcessView = ViewsSystem.Show<ObjectiveAttackProcessView>(ViewConfigID.ShelterAttackModeConfig);}
                ));
        }

        private void AttackModeFinishedUI()
        {
            ViewsSystem.TryHideView(attackProcessView);
            string messageText = LocalizationModel.GetString(LocalizationKeyID.InGameNotif_ShelterAttackCompleted);
            NotificationContainerViewModel.Show(PriorityID.Wave, ViewConfigID.AttackSessionCompleted, new AttackSessionCompletedControllerData(messageText, 3));
        }

        private void AttackModeFaildedUI()
        {
            ViewsSystem.TryHideView(attackProcessView);
            string messageText = LocalizationModel.GetString(LocalizationKeyID.InGameNotif_ShipAttackFailed);
            NotificationContainerViewModel.Show(PriorityID.Wave, ViewConfigID.SpawnWaveInfo, new ObjectiveInfoViewControllerData(VillageAttackModel.AttackFailedIcon, messageText, 3));
        }

        private void OnWaveCompleteUI()
        {
            ViewsSystem.TryHideView(attackProcessView);

            float timeToDisplay = config.PauseBetweenWaves;
            string messageText = LocalizationModel.GetString(LocalizationKeyID.InGameNotif_ShipAttackNewWave) 
                + " (" + (VillageAttackModel.WaveIndex + 1) + "/" +config.Waves.Count() + ")";
            NotificationContainerViewModel.Show(PriorityID.Wave, ViewConfigID.AttackSessionCompleted, new AttackSessionCompletedControllerData(messageText, timeToDisplay));

            AttackModeInProgressUI(timeToDisplay);

            AudioSystem.PlayOnce(AudioID.WaveDefeate);
        }

        IEnumerator DoAfterSeconds(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
            yield break;
        }

    }
}
