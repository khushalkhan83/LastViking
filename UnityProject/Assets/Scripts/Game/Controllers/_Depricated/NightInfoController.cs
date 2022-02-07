using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class NightInfoController : INightInfoController, IController
    {
        [Inject] public StartNightInfoModel StartNightInfoModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public AttackDelayStatusViewModel AttackDelayStatusViewModel { get; private set; }
        [Inject] public NotificationContainerViewModel NotificationContainerViewModel { get; private set; }
        [Inject] public SkeletonSpawnManager SkeletonSpawnManager { get; private set; }

        void IController.Enable()
        {
            SkeletonSpawnManager.OnSessionStarted += OnSessionStarted;
            SkeletonSpawnManager.OnSessionCleared += OnSessionCleared;
        }

        void IController.Start()
        {
            if (SkeletonSpawnManager.IsEnableSpawn)
            {
                GameUpdateModel.OnUpdate += OnUpdate;
                SkeletonSpawnManager.OnDisableSpawn += OnDisableSpawn;
            }
            else
            {
                SkeletonSpawnManager.OnEnableSpawn += OnEnableSpawn;
            }
        }

        void IController.Disable()
        {
            SkeletonSpawnManager.OnSessionStarted -= OnSessionStarted;
            SkeletonSpawnManager.OnSessionCleared -= OnSessionCleared;
            GameUpdateModel.OnUpdate -= OnUpdate;
            SkeletonSpawnManager.OnDisableSpawn -= OnDisableSpawn;
            SkeletonSpawnManager.OnEnableSpawn -= OnEnableSpawn;
        }

        private void OnEnableSpawn()
        {
            SkeletonSpawnManager.OnEnableSpawn -= OnEnableSpawn;
            GameUpdateModel.OnUpdate += OnUpdate;
            SkeletonSpawnManager.OnDisableSpawn += OnDisableSpawn;
        }

        private void OnDisableSpawn()
        {
            SkeletonSpawnManager.OnDisableSpawn -= OnDisableSpawn;
            GameUpdateModel.OnUpdate -= OnUpdate;
            SkeletonSpawnManager.OnEnableSpawn += OnEnableSpawn;

            OnSessionFailed(); // [change]
        }

        public bool GetIsInAttackPredelay(float spawnTime) => (int)GameTimeModel.EnviroTimeOfDay >= spawnTime - StartNightInfoModel.ShowAttackMessagePredelay && GameTimeModel.EnviroTimeOfDay < spawnTime;

        SkeletonSpawnManager.SessionSettings sessionNext;
        SkeletonSpawnManager.WaveSettings waveFirst;
        private bool _canShow = true;
        private void OnUpdate()
        {
            if
            (
                _canShow
                && SkeletonSpawnManager.TryGetSession(out sessionNext)
                && sessionNext.TryGetWave(0, out waveFirst)
                && GetIsInAttackPredelay(waveFirst.SpawnTime)
            )
            {
                NotificationContainerViewModel.Show(PriorityID.Wave, ViewConfigID.ObjectiveTimeDelay, new ObjectiveTimeDelayViewControllerData(LocalizationModel.GetString(LocalizationKeyID.InGameNotif_ShelterAttackSoon), waveFirst.SpawnTime, 5));
                AttackDelayStatusViewModel.StartWaitAttack();
                AttackDelayStatusViewModel.TargetTime = waveFirst.SpawnTime;
                _canShow = false;
            }
        }

        private void OnSessionStarted()
        {
            StartNightInfoModel.StartNight();
            var data = StartNightInfoModel.GetWaveInfoData(WaveInfoId.SessionStarted);
            NotificationContainerViewModel.Show(PriorityID.Wave, ViewConfigID.SpawnWaveInfo, new ObjectiveInfoViewControllerData(data.Icon, LocalizationModel.GetString(data.DescriptionKey), 3));
            AttackDelayStatusViewModel.EndWaitAttack();
        }

        private void OnSessionCleared()
        {
            StartNightInfoModel.EndNight();
            var data = StartNightInfoModel.GetWaveInfoData(WaveInfoId.SessionCleared);
            NotificationContainerViewModel.Show(PriorityID.Wave, ViewConfigID.AttackSessionCompleted, new AttackSessionCompletedControllerData(LocalizationModel.GetString(data.DescriptionKey), 3));

            _canShow = true;
        }

        private void OnSessionFailed()
        {
            var data = StartNightInfoModel.GetWaveInfoData(WaveInfoId.SessionFailed);
            var msg = "You failed Undead's attack";
            NotificationContainerViewModel.Show(PriorityID.Wave, ViewConfigID.SpawnWaveInfo, new ObjectiveInfoViewControllerData(data.Icon, msg, 3));
        }
    }
}
