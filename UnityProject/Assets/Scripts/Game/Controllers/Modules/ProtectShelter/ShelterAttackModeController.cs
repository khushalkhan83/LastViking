using System;
using Core;
using Core.Controllers;
using Game.Models;
using UnityEngine;
using Extensions;
using Game.Audio;
using Game.Views;
using System.Collections;
using Game.Providers;
using System.Linq;
using EnemiesAttack;

namespace Game.Controllers
{
    public class ShelterAttackModeController : IShelterAttackModeController, IController
    {
        [Inject] public ShelterAttackModeModel ShelterAttackModeModel {get; private set;}
        [Inject] public ShelterUpgradeModel ShelterUpgradeModel {get; private set;}
        [Inject] public AudioSystem AudioSystem {get; private set;}
        [Inject] public NotificationContainerViewModel NotificationContainerViewModel {get; private set;}
        [Inject] public ViewsSystem ViewsSystem {get; private set;}
        [Inject] public CoroutineModel CoroutineModel {get; private set;}
        [Inject] public EnemiesSpawnModel EnemiesSpawnModel {get; private set;}
        [Inject] public StorageModel StorageModel {get; private set;}
        [Inject] public PlayerDeathModel PlayerDeathModel {get; private set;}
        [Inject] public LocalizationModel LocalizationModel {get; private set;}

        private ShelterModelsProvider ShelterModelsProvider => ModelsSystem.Instance._shelterModelsProvider;
        private ShelterModel ShelterModel => ShelterModelsProvider[ShelterModelID.Ship];


        private EnemiesAttackConfig config;
        private int corutineIndex = -1;
        private ObjectiveAttackProcessView attackProcessView;
        private bool attackModeStarted;

        void IController.Enable() 
        {
            return;
            StorageModel.TryProcessing(ShelterAttackModeModel._Data);

            ShelterAttackModeModel.OnAttackModeStart += OnAttackModeStart;
            ShelterAttackModeModel.OnAttackModeAvaliableChanged += OnAttackModeAvaliableChanged;
            ShelterAttackModeModel.OnAttackModeFinish += OnAttackModeFinish;
            ShelterAttackModeModel.OnAttackModeCancel += OnAttackModeCancel;
            ShelterAttackModeModel.OnWaveComplete += OnWaveComplete;
            EnemiesSpawnModel.OnEnemyDestroyed += OnEnemyDestroyed;
            EnemiesSpawnModel.OnWaveStart += OnWaveStart;
            ShelterModel.OnDeath += OnShelterDestroyed;
            PlayerDeathModel.OnPreRevival += OnPreRevival;
            PlayerDeathModel.OnRevivalPrelim += OnRevivalPrelim;
        }

        void IController.Start() 
        {
            
        }

        void IController.Disable() 
        {
            ShelterAttackModeModel.OnAttackModeStart -= OnAttackModeStart;
            ShelterAttackModeModel.OnAttackModeAvaliableChanged -= OnAttackModeAvaliableChanged;
            ShelterAttackModeModel.OnAttackModeFinish -= OnAttackModeFinish;
            ShelterAttackModeModel.OnAttackModeCancel -= OnAttackModeCancel;
            ShelterAttackModeModel.OnWaveComplete -= OnWaveComplete;
            EnemiesSpawnModel.OnEnemyDestroyed -= OnEnemyDestroyed;
            EnemiesSpawnModel.OnWaveStart -= OnWaveStart;
            ShelterModel.OnDeath -= OnShelterDestroyed;
            PlayerDeathModel.OnPreRevival -= OnPreRevival;
            PlayerDeathModel.OnRevivalPrelim -= OnRevivalPrelim;
            CoroutineModel.BreakeCoroutine(corutineIndex);
        }

        private void OnPreRevival() => UpdateViews();
        private void OnRevivalPrelim() => UpdateViews();

        private void UpdateViews()
        {
            if(!attackModeStarted) return;

            AttackModeInProgressUI();
        }


        private void OnEnemyDestroyed(DeathController enemy)
        {
            ShelterAttackModeModel.EnemiesLeft--;
        }

        private void OnWaveStart()
        {
            AudioSystem.PlayOnce(AudioID.WaveStart);
        }

        private void OnAttackModeStart()
        {
            StartAttack(false);
        }

        private void StartAttack(bool continueAttack)
        {
            if(attackModeStarted) return;
            attackModeStarted = true;

            ShelterUpgradeModel.SetShowUpgrade(true);

            config = ShelterAttackModeModel.EnemiesAttackConfig;
           
            if(!continueAttack)
            {
                ShelterAttackModeModel.WaveIndex = 0;
            }

            EnemiesSpawnModel.StartSpawnSession(config);

            var enemiesTotal = config.EnemiesTotal;
            ShelterAttackModeModel.EnemiesTotal = enemiesTotal;
            ShelterAttackModeModel.EnemiesLeft = config.GetEnemiesLeft(ShelterAttackModeModel.WaveIndex);

            AttackModeStartUI();
        }

        private void OnAttackModeFinish() => DisableAttackMode();
        private void OnAttackModeCancel() => DisableAttackMode();

        private void DisableAttackMode()
        {
            if(!attackModeStarted) return;
            
            attackModeStarted = false;

            ShelterUpgradeModel.SetShowUpgrade(false);

            AttackModeFinishedUI();
        }

        private void OnShelterDestroyed()
        {
            if(!attackModeStarted) return;
            attackModeStarted = false;

            ShelterUpgradeModel.SetShowUpgrade(false);
            ShelterAttackModeModel.AttackModeFailed();

            AttackModeFaildedUI();
        }

        private void OnAttackModeAvaliableChanged(bool stateActive)
        {
            if(stateActive)
            {
                if(ShelterAttackModeModel.AttackModeActive)
                {
                    StartAttack(true);
                }
            }
            else
            {
                if(ShelterAttackModeModel.AttackModeActive)
                {
                    ShelterAttackModeModel.CancelAttackMode();
                }
            }
        }

        private void OnWaveComplete()
        {
            int wavesCount = config.Waves.Count();
            bool noWavesLeft = ShelterAttackModeModel.WaveIndex >= wavesCount;

            if(noWavesLeft) return;

            OnWaveCompleteUI();
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
            NotificationContainerViewModel.Show(PriorityID.Wave, ViewConfigID.SpawnWaveInfo, new ObjectiveInfoViewControllerData(ShelterAttackModeModel.AttackFailedIcon, messageText, 3));
        }

        private void OnWaveCompleteUI()
        {
            ViewsSystem.TryHideView(attackProcessView);

            float timeToDisplay = config.PauseBetweenWaves;
            string messageText = LocalizationModel.GetString(LocalizationKeyID.InGameNotif_ShipAttackNewWave) 
                + " (" + (ShelterAttackModeModel.WaveIndex + 1) + "/" + ShelterAttackModeModel.EnemiesAttackConfig.Waves.Count() + ")";
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