using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;
using Game.Views;
using System;
using System.Collections;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class LoseController : ILoseController, IController
    {
        [Inject] public PlayerHealthBarViewModel PlayerHealthBarViewModel { get; private set; }
        [Inject] public PlayerWaterBarViewModel PlayerWaterBarViewModel { get; private set; }
        [Inject] public PlayerFoodBarViewModel PlayerFoodBarViewModel { get; private set; }
        [Inject] public PlayerDeathHandler PlayerDeathHandler { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public WorldObjectsModel WorldObjectsModel { get; private set; }
        [Inject] public PlayerWaterModel PlayerWaterModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public WorldCameraModel WorldCameraModel { get; private set; }
        [Inject] public PlayerFoodModel PlayerFoodModel { get; private set; }
        [Inject] public PrefabsProvider PrefabsProvider { get; private set; }
        [Inject] public LoseViewModel LoseViewModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public PlayerCameras PlayerCameras { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public TombsModel TombsModel { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }
        [Inject] public PlayerScenesModel PlayerScenesModel { get; private set; }

        public DeathFaderView DeathFaderView { get; private set; }
        public BlackView BlackView { get; private set; }

        public LoseView LoseView { get; private set; }
        public DeathPlayerView DeathPlayerView { get; private set; }
        public TutorialDeathPlayerView TutorialDeathPlayerView { get; private set; }

        public WorldObjectModel TombObject { get; private set; }
        public Camera CameraTomb { get; private set; }

        private int _tutorialDeathProcessCoroutineId = -1;
        private int _preDeathProcessCoroutineId = -1;
        private int _deathProcessCoroutineId = -1;
        private int _fadeCoroutineId = -1;

        void IController.Enable()
        {
        }

        void IController.Start()
        {
            if(!PlayerScenesModel.SceneLoading)
            {
                Initialize();
            }
            else
            {
                PlayerScenesModel.OnEnvironmentLoaded += OnEnvironmentLoaded;
            }
        }

        private void Initialize()
        {
            if (PlayerHealthModel.IsDead)
            {
                if (TutorialModel.IsComplete || LoseViewModel.HasTutorialResurrected)
                {
                    if (LoseViewModel.ShouldCreateTomb)
                    {
                        LoseViewModel.ResetShouldCreateTomb();
                        OnDeathPlayerHandler();
                    }
                    else
                    {
                        LoseView = ViewsSystem.Show<LoseView>(ViewConfigID.Lose);

                        PlayerDeathModel.OnPreRevival += OnPreRevivalPlayerHandler;
                        ShowCameraTomb();
                    }
                }
                else
                {
                    TutorialDeathPlayerView = ViewsSystem.Show<TutorialDeathPlayerView>(ViewConfigID.TutorialDeathPlayer);

                    PlayerDeathModel.OnPreRevival += OnTutorialRevivalHandler;
                }

                GameTimeModel.ScaleStop();
                PlayerDeathHandler.DisableObjects();
            }
            else
            {
                UpdateSubscribesOnDeath();
            }

            if (!(TutorialModel.IsComplete || LoseViewModel.HasTutorialResurrected))
            {
                TutorialModel.OnComplete += OnTutorialComplete;
            }
        }

        private void OnEnvironmentLoaded()
        {
            Initialize();
            PlayerScenesModel.OnEnvironmentLoaded -= OnEnvironmentLoaded;
        }


        void IController.Disable()
        {
            PlayerHealthModel.OnDeath -= OnPreDeathPlayerHandler;
            PlayerHealthModel.OnDeath -= OnTutorialDeathPlayerHandler;
            PlayerScenesModel.OnEnvironmentLoaded -= OnEnvironmentLoaded;
            CoroutineModel.BreakeCoroutine(_tutorialDeathProcessCoroutineId);
            CoroutineModel.BreakeCoroutine(_preDeathProcessCoroutineId);
            CoroutineModel.BreakeCoroutine(_deathProcessCoroutineId);
            CoroutineModel.BreakeCoroutine(_fadeCoroutineId);
        }

        private void OnTutorialComplete()
        {
            TutorialModel.OnComplete -= OnTutorialComplete;
            UpdateSubscribesOnDeath();
        }

        private void UpdateSubscribesOnDeath()
        {
            PlayerHealthModel.OnDeath -= OnTutorialDeathPlayerHandler;
            PlayerHealthModel.OnDeath -= OnPreDeathPlayerHandler;

            if (TutorialModel.IsComplete || LoseViewModel.HasTutorialResurrected)
            {
                PlayerHealthModel.OnDeath += OnPreDeathPlayerHandler;
            }
            else
            {
                PlayerHealthModel.OnDeath += OnTutorialDeathPlayerHandler;
            }
        }

        private void OnTutorialDeathPlayerHandler()
        {
            PlayerDeathModel.OnPreRevival += OnTutorialRevivalHandler;
            PlayerHealthModel.OnDeath -= OnTutorialDeathPlayerHandler;

            PlayerDeathHandler.TutorialDeath();
            GameTimeModel.ScaleStop();

            _tutorialDeathProcessCoroutineId = CoroutineModel.InitCoroutine(TutorialDeathProcess());
        }

        private void OnTutorialRevivalHandler()
        {
            PlayerDeathModel.OnPreRevival -= OnTutorialRevivalHandler;
            UpdateSubscribesOnDeath();

            GameTimeModel.ScaleReset();
        }

        private void OnEndResurrectTimeHandler()
        {
            LoseViewModel.OnEndResurrectTime -= OnEndResurrectTimeHandler;
            PlayerDeathModel.OnPreRevival -= OnPreRevivalHandler;
            LoseViewModel.ResetShouldCreateTomb();
            OnDeathPlayerHandler();
        }

        private void OnPreDeathPlayerHandler()
        {
            PlayerDeathModel.OnPreRevival += OnPreRevivalHandler;
            PlayerHealthModel.OnDeath -= OnPreDeathPlayerHandler;

            PlayerDeathHandler.PreDeath();
            GameTimeModel.ScaleStop();

            _preDeathProcessCoroutineId = CoroutineModel.InitCoroutine(PreDeathProcess());
            LoseViewModel.SetShouldCreateTomb();
            LoseViewModel.OnEndResurrectTime += OnEndResurrectTimeHandler;
        }

        private void OnDeathPlayerHandler()
        {
            PlayerDeathModel.OnPreRevival += OnPreRevivalPlayerHandler;

            PlayerDeathHandler.Death();

            PlayerFoodModel.SetAddonLevel((ushort)(PlayerFoodModel.AddonLevel + 1));
            PlayerWaterModel.SetAddonLevel((ushort)(PlayerWaterModel.AddonLevel + 1));
            PlayerHealthModel.SetAddonLevel((ushort)(PlayerHealthModel.AddonLevel + 1));
            PlayerFoodBarViewModel.SetAddonLevel((ushort)(PlayerFoodBarViewModel.AddonLevel + 1));
            PlayerWaterBarViewModel.SetAddonLevel((ushort)(PlayerWaterBarViewModel.AddonLevel + 1));
            PlayerHealthBarViewModel.SetAddonLevel((ushort)(PlayerHealthBarViewModel.AddonLevel + 1));

            TombsModel.CreateTomb();
            InitTombObject();
            LoseViewModel.SetPivot(TombObject.transform);
            TombObject.gameObject.SetActive(false);
            _deathProcessCoroutineId = CoroutineModel.InitCoroutine(DeathProcess());
        }

        private void InitTombObject()
        {
            var tombs = WorldObjectsModel.SaveableObjectModels[WorldObjectID.Tomb];
            TombObject = tombs[tombs.Count - 1]; //HACK last tomb in list is last created tomb
        }

        private void OnPreRevivalHandler()
        {
            PlayerDeathModel.OnPreRevival -= OnPreRevivalHandler;
            LoseViewModel.OnEndResurrectTime -= OnEndResurrectTimeHandler;
            PlayerHealthModel.OnDeath += OnPreDeathPlayerHandler;

            GameTimeModel.ScaleReset();
        }

        private void OnPreRevivalPlayerHandler()
        {
            PlayerDeathModel.OnPreRevival -= OnPreRevivalPlayerHandler;

            PlayerHealthModel.OnDeath += OnPreDeathPlayerHandler;

            GameTimeModel.ScaleReset();

            WorldCameraModel.SetCamera(WorldCameraModel.DefaultWorldCamera);
            HideTomb();
        }

        public Color Zero => new Color(0, 0, 0, 0);

        private IEnumerator TutorialDeathProcess()
        {
            yield return null;

            ViewsSystem.HideAll();
            TutorialDeathPlayerView = ViewsSystem.Show<TutorialDeathPlayerView>(ViewConfigID.TutorialDeathPlayer);
        }

        private IEnumerator PreDeathProcess()
        {
            yield return null;

            ViewsSystem.HideAll();
            DeathPlayerView = ViewsSystem.Show<DeathPlayerView>(ViewConfigID.DeathPlayer);
        }

        private IEnumerator DeathProcess()
        {
            yield return null;

            TombObject.gameObject.SetActive(true);
            ShowCameraTomb();

            yield return new WaitForSecondsRealtime(1f);

            LoseView = ViewsSystem.Show<LoseView>(ViewConfigID.Lose);
        }

        private void ShowCameraTomb()
        {
            PlayerCameras.HideAllCameras();
            CreateCameraTomb();
        }

        private void HideTomb()
        {
            PlayerCameras.ShowAllCameras();

            if (LoseView != null)
            {
                ViewsSystem.Hide(LoseView);
                LoseView = null;
            }

            DestroyCameraTomb();
        }

        public IEnumerator Fade(Action<Color> color, Color from, Color to, float time)
        {
            var timeCurrent = 0f;
            while (timeCurrent < time)
            {
                color(Color.Lerp(from, to, Mathf.Clamp01(timeCurrent / time)));

                yield return null;
                timeCurrent += Time.unscaledDeltaTime;
            }
        }

        private void CreateCameraTomb()
        {
            var cameraRef = PrefabsProvider[PrefabID.TombCamera];
            var cameraTombObject = GameObject.Instantiate(cameraRef, WorldObjectsModel.transform);
            var cameraTombTransform = cameraTombObject.transform;
            var position = LoseViewModel.PivotPosition
                + LoseViewModel.PivotForward * LoseViewModel.CameraTombOffset.z
                + LoseViewModel.PivotUp * LoseViewModel.CameraTombOffset.y
                + LoseViewModel.PivotRigth * LoseViewModel.CameraTombOffset.x;

            cameraTombTransform.position = position;
            cameraTombTransform.LookAt(LoseViewModel.PivotPosition + LoseViewModel.CameraPivotOffset);

            CameraTomb = cameraTombObject.GetComponent<Camera>();
            WorldCameraModel.SetCamera(CameraTomb);
        }

        private void DestroyCameraTomb()
        {
            if (CameraTomb != null)
            {
                GameObject.Destroy(CameraTomb.gameObject);
                CameraTomb = null;
            }
        }
    }
}
