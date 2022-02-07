using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Purchases;
using Game.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Controllers
{
    public class LoseViewController : ViewControllerBase<LoseView>
    {
        [Inject] public PlayerBleedingDamagerModel PlayerBleedingDamagerModel { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public LoseViewModel LoseViewModel { get; private set; }
        [Inject] public LeaderboardViewModel LeaderboardViewModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerFoodModel PlayerFoodModel { get; private set; }
        [Inject] public PlayerWaterModel PlayerWaterModel { get; private set; }
        [Inject] public PlayerStaminaModel PlayerStaminaModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public PlayerDeathHandler PlayerDeathHandler { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public PlayerProfileModel PlayerProfileModel { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public SheltersModel SheltersModel { get; private set; }
        [Inject] public PlayerRespawnPoints PlayerRespawnPoints { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public GameSparksModel GameSparksModel { get; private set; }
        [Inject] public InventoryViewModel InventoryViewModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public AdvertisementsModel AdvertisementsModel { get; private set; }
        [Inject] public PlayerRunModel PlayerRunModel { get; private set; }
        [Inject] public StatisticsModel StatisticsModel { get; private set; }
        [Inject] public TouchpadModel TouchpadModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public JoystickModel JoystickModel { get; private set; }
        [Inject] public PurchaseComplitedModel PurchaseComplitedModel { get; private set; }

        protected Animation _selectPlayer;
        protected Animation _selectStatistics;

        protected List<PlayerData> PlayersData { get; } = new List<PlayerData>();

        protected override void Show()
        {
            AudioSystem.PlayOnce(AudioID.DeathView);
            LocalizationModel.OnChangeLanguage += SetLocalization;
            View.OnRestart += OnRestartHandler;
            //View.OnSelectPlayer += OnSelectPlayerHandler;
            //View.OnSelectStatistics += OnSelectStatisticsHandler;

            LoseViewModel.OnPlayAgain += OnPlayAgainModelHandler;

            //View.LeaderboardView.OnGameSparksRegister += OnButtonSubmit;
            //StartCoroutine(StartDefault());
            //View.ShowObjects();

            View.SetSpriteRespawnPoint(RespawnIcon);

            AdvertisementsModel.ShowBanner();
            //WaitForInitLeaderboard();

            SetLocalization();

            NetworkModel.OnInternetConnectionStateChange += OnInternetConnectionStateChangeHandler;
            NetworkModel.UpdateInternetConnectionStatus();

            if (NetworkModel.IsHasConnection)
            {
                PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.AfterDethVideoInter).Prepere();
            }

            //StartCoroutine(StartShowAnimation());
            //StartCoroutine(SetStatisticsPopupValues());
        }
        
        private IEnumerator StartDefault()
        {
            View.Animator.SetTrigger("Default");
            yield return null;
        }

        private IEnumerator StartShowAnimation()
        {
            yield return new WaitForSecondsRealtime(1f);

            View.Animator.SetTrigger("LineSlide");
            yield return null;
        }

        protected override void Hide()
        {
            NetworkModel.OnInternetConnectionStateChange -= OnInternetConnectionStateChangeHandler;

            LocalizationModel.OnChangeLanguage -= SetLocalization;

            View.OnRestart -= OnRestartHandler;
            View.OnSelectPlayer -= OnSelectPlayerHandler;
            View.OnSelectStatistics -= OnSelectStatisticsHandler;

            LoseViewModel.OnPlayAgain -= OnPlayAgainModelHandler;

            View.LeaderboardView.OnGameSparksRegister -= OnButtonSubmit;
            StopAllCoroutines();

            AdvertisementsModel.HideBanner();
            ResetLeaderboard();
        }

        private void OnInternetConnectionStateChangeHandler()
        {
            if (NetworkModel.IsHasConnection)
            {
                PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.AfterDethVideoInter).Prepere();
            }
        }

        private Sprite RespawnIcon => SheltersModel.ShelterActive != ShelterModelID.None
                ? View.ShelterSprite
                : View.PalmSprite;

        private void OnPlayAgainModelHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            LoseViewModel.OnPlayAgain -= OnPlayAgainModelHandler;

            RestartGame();
            PurchasesModel.Purchase(PurchaseID.AfterDethVideoInter, r => { });
        }

        private void OnRestartHandler()
        {
            View.OnRestart -= OnRestartHandler;
            RefreshHotBar();
            LoseViewModel.PlayAgain();
        }

        private void RefreshHotBar()
        {
            HotBarModel.Equp(0);
        }

        private void OnWatchEndHandler(PurchaseResult purchaseResult) => LoseViewModel.PlayAgain();

        private void RestartGame()
        {
            Refill();

            InventoryModel.ItemsContainer.RemoveAllItems();

            HotBarModel.ItemsContainer.RemoveAllItems();
            HotBarModel.SetDefaultItem();

            PlayerDeathModel.Revival();
            PlayerDeathModel.BeginImunitet();
        }

        private Transform GetRespawnPoint() => SheltersModel.ShelterActive != ShelterModelID.None
            ? PlayerRespawnPoints.PointShelter
            : GetRandomRespawnPoint();

        private Transform GetRandomRespawnPoint() => PlayerRespawnPoints.Points[Random.Range(0, PlayerRespawnPoints.Points.Length)];

        private void Refill()
        {
            PlayerHealthModel.RefillHealth();
            PlayerFoodModel.RefillFood();
            PlayerWaterModel.RefillWater();
            PlayerStaminaModel.RefillStamina();
            InventoryViewModel.Reset();
            InventoryModel.Reset();
            JoystickModel.SetDefaultAxes();
            TouchpadModel.SetDefaultAxes();

            PlayerRunModel.RunStop();
            PlayerRunModel.RunTogglePassive();
            PlayerEventHandler.Jump.ForceStop();
            PlayerEventHandler.Aim.ForceStop();
            PlayerBleedingDamagerModel.SetBleeding(false);
        }

        private IEnumerator SetStatisticsPopupValues()
        {
            yield return new WaitForSecondsRealtime(1f);
            ShowCounterProcess();
            ShowTimeProcess();
            ShowTimeSessionProcess();
            ShowGatheredPorcess();
            ShowCraftedProcess();
            ShowAnimalsProcess();
            ShowZombiesProcess();
            ShowSkeletonsProcess();
            ShowBarrelsProcess();
            yield return null;
        }

        private void ShowTimeProcess() => StartCoroutine(ShowProcess(LeaderboardViewModel.InitAccumulatingDelay + 0.2f, (int) GameTimeModel.Days, View.SetTotalDaysObjectScale, View.SetTotalDaysOnIsland));
        private void ShowCounterProcess() => StartCoroutine(ShowProcess(LeaderboardViewModel.InitAccumulatingDelay + 0.2f, (int)GameTimeModel.GetDays(GameTimeModel.Ticks - StatisticsModel.StartAliveTimeTicks), View.SetCounterObjectScale, View.SetCounter));
        private void ShowTimeSessionProcess() => StartCoroutine(ShowProcess(LeaderboardViewModel.InitAccumulatingDelay + 1.4f, (int)GameTimeModel.GetDays(GameTimeModel.Ticks - StatisticsModel.StartAliveTimeTicks), View.SetSessionDaysObjectScale, View.SetDaysBySession));
        private void ShowGatheredPorcess() => StartCoroutine(ShowProcess(LeaderboardViewModel.InitAccumulatingDelay + 1.6f, (int)StatisticsModel.GatheredResources, View.SetGatheredObjectScale, View.SetGatharedResourcesCount));
        private void ShowCraftedProcess() => StartCoroutine(ShowProcess(LeaderboardViewModel.InitAccumulatingDelay + 1.8f, (int)StatisticsModel.CraftedItems, View.SetCraftedObjectScale, View.SetCraftedItemsCount));
        private void ShowAnimalsProcess() => StartCoroutine(ShowProcess(LeaderboardViewModel.InitAccumulatingDelay + 2.2f, (int)StatisticsModel.KilledAnimal, View.SetKilledAnimalObjectScale, View.SetKilledAnimalCount));
        private void ShowZombiesProcess() => StartCoroutine(ShowProcess(LeaderboardViewModel.InitAccumulatingDelay + 2.4f, (int)StatisticsModel.KilledZombie, View.SetKilledZombiesObjectScale, View.SetKilledZombieCount));
        private void ShowSkeletonsProcess() => StartCoroutine(ShowProcess(LeaderboardViewModel.InitAccumulatingDelay + 2.6f, (int)StatisticsModel.KilledSkeleton, View.SetKilledSkeletonsObjectScale, View.SetKilledSkeletonsCount));
        private void ShowBarrelsProcess() => StartCoroutine(ShowProcess(LeaderboardViewModel.InitAccumulatingDelay + 2.9f, (int)StatisticsModel.BarrelsDestroyed, View.SetBrokenObjectScale, View.SetBrokenBarrelsCount));

        private IEnumerator ShowProcess(float waitTime, int score, Action<float> scaleProcessCallback, Action<int> numberProcessCallback)
        {
            yield return new WaitForSecondsRealtime(waitTime);

            yield return ScaleElement(scaleProcessCallback, 1f, LeaderboardViewModel.TargetElementsScale);
            yield return AccumulateNumber(numberProcessCallback, 0, score);
            AudioSystem.PlayOnce(AudioID.Count);
            yield return ScaleElement(scaleProcessCallback, LeaderboardViewModel.TargetElementsScale, 1f);
        }

        private void OnSelectPlayerHandler()
        {
            View.Animator.SetTrigger("SelectPlayerAnimation");
            AudioSystem.PlayOnce(AudioID.Switch);
        }

        private void OnSelectStatisticsHandler()
        {
            View.Animator.SetTrigger("SelectStatisticsAnimation");
            AudioSystem.PlayOnce(AudioID.Switch);
        }

        private void SetLocalization()
        {
            View.SetTextPlayAgainButton(LocalizationModel.GetString(LocalizationKeyID.LoseMenu_PayAgainBtn));
            View.SetTextTitle(LocalizationModel.GetString(LocalizationKeyID.LoseMenu_Title));

            SetLocalizationLeaderboard();
        }

        private void SetLocalizationLeaderboard()
        {
            View.LeaderboardView.SetTextPlayerScore(LocalizationModel.GetString(LocalizationKeyID.AccountPopup_Score));
            View.LeaderboardView.SetTextSubmitButton(LocalizationModel.GetString(LocalizationKeyID.CharacterPopup_SubmitBtn));
        }

        private void AutoLogin(System.Action onEnd)
        {
            /* GAMESPARK STUFF */
            //if (GameSparksModel.IsHasUserLogined)
            //{
            //    new AuthenticationRequest()
            //        .SetUserName(GameSparksModel.UserName)
            //        .SetPassword(GameSparksModel.UserPass)
            //        .Send
            //        (
            //            response =>
            //            {
            //                if (!response.HasErrors)
            //                {
            //                    onEnd?.Invoke();
            //                }
            //            }
            //        );
            //}
            //else if (GameSparksModel.IsHasUserDeviceLogined)
            //{
            //    new DeviceAuthenticationRequest()
            //        .SetDisplayName(GameSparksModel.UserName)
            //        .Send
            //        (
            //            response =>
            //            {
            //                if (!response.HasErrors)
            //                {
            //                    onEnd?.Invoke();
            //                }
            //            }
            //        );
            //}
        }

        private void WaitForInitLeaderboard()
        {
            LeaderboardViewModel.HasUpdatedAfterLogin = false;

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                View.ShowLeaderboardView(true);
                StartCoroutine(ShowNoInternetLeaderboardProcess());
            }
            else
            {
                View.ShowLeaderboardView(true);
                View.LeaderboardView.ShowSubmitButton(false);

                InitLeaderboard();
            }
        }

        private void ShowDefaultLocalPlayerView()
        {
            View.LeaderboardView.SetPlayerName(PlayerProfileModel.PlayerName);
            View.LeaderboardView.SetPlayerScore(0);
        }

        private void InitLeaderboard()
        {
            if (LeaderboardViewModel.HasUpdatedAfterLogin)
            {
                return;
            }

            AutoLogin
            (
                () =>
                {
                                                            /* GAMESPARK STUFF */
                    if (GameSparksModel.IsHasUserLogined/* && GS.Authenticated*/)
                    {
                        LoadHighScore();
                    }
                    else
                    {
                        LoadPreviewHighScore();
                    }
                }
            );
        }

        private void LoadPreviewHighScore() => LoadFirstScores(() => StartCoroutine(ShowPreviewLeaderboardProcess()));

        private void LoadHighScore() => GetPlayerCurrentData
        (
            () => LoadFirstScores
            (
                () =>
                {
                    if (LoseViewModel.PlayerRank > LeaderboardViewModel.NumFirstPartPlayers)
                    {
                        LoadSecondScores(() => StartCoroutine(ShowLeaderboardProcess()));
                    }
                    else
                    {
                        StartCoroutine(ShowLeaderboardProcess());
                    }
                }
            )
        );

        private void LoadHighScoreAndUpdateView() => PushScores
        (
            PlayerProfileModel.PlayerScore,
            () => GetPlayerCurrentData(
                () => LoadFirstScores(
                    () =>
                    {
                        if (LoseViewModel.PlayerRank > LeaderboardViewModel.NumFirstPartPlayers)
                        {
                            LoadSecondScores(UpdatePreviewViewsFinal);
                        }
                        else
                        {
                            UpdatePreviewViewsFinal();
                        }
                    }
                )
            )
        );

        private void PushScores(int score, Action onEnd)
        {
            /* GAMESPARK STUFF */
            //            new LogEventRequest()
            //.SetEventKey(GameSparksModel.ScorerKey)
            //.SetEventAttribute(GameSparksModel.ScorerAttribute, score)
            //.Send
            //(
            //response =>
            //{
            //if (!response.HasErrors)
            //{
            //onEnd?.Invoke();
            //}
            //}
            //);
        }

        private void GetPlayerCurrentData(Action onEnd)
        {
            /* GAMESPARK STUFF */
            //            new AroundMeLeaderboardRequest()
            //.SetLeaderboardShortCode(GameSparksModel.LeaderboardId)
            //.SetEntryCount(1)
            //.Send
            //(
            //response =>
            //{
            //if (!response.HasErrors)
            //{
            //foreach (var data in response.Data)
            //{
            //var rank = (int)data.Rank;
            //var name = data.UserName;
            //var id = data.UserId;
            //var score = data.JSONData[GameSparksModel.ScorerAttribute].ToString();

            //if (IsLocalPlayer(id))
            //{
            //LoseViewModel.PlayerRank = rank;
            //LoseViewModel.OldScore = int.Parse(score);
            //}
            //}
            //onEnd?.Invoke();
            //}
            //}
            //);
        }

        private void LoadFirstScores(Action onEnd)
        {
            /* GAMESPARK STUFF */
            //            new LeaderboardDataRequest()
            //.SetLeaderboardShortCode(GameSparksModel.LeaderboardId)
            //.SetEntryCount(LeaderboardViewModel.NumFirstPartPlayers)
            //.Send
            //(
            //response =>
            //{
            //if (!response.HasErrors)
            //{
            //foreach (var data in response.Data)
            //{
            //var rank = (int)data.Rank;
            //var name = data.UserName;
            //var id = data.UserId;
            //var score = data.JSONData[GameSparksModel.ScorerAttribute].ToString();
            //var player = GetPlayerData(name, int.Parse(score), rank);

            //if (IsLocalPlayer(id))
            //{
            //player.IsLocal = true;
            //}

            //PlayersData.Add(player);
            //}

            //onEnd?.Invoke();
            //}
            //}
            //);
        }

        private void LoadSecondScores(Action onEnd)
        {
            /* GAMESPARK STUFF */
            //            new AroundMeLeaderboardRequest()
            //.SetLeaderboardShortCode(GameSparksModel.LeaderboardId)
            //.SetEntryCount(LeaderboardViewModel.NumSecondPartPlayers / 2)
            //.Send
            //(
            //response2 =>
            //{
            //if (!response2.HasErrors)
            //{
            //for (int i = 1; i < response2.Data.Count(); i++)
            //{
            //var data = response2.Data.ElementAt(i);
            //var rank = (int)data.Rank;
            //var name = data.UserName;
            //var id = data.UserId;
            //var score = data.JSONData[GameSparksModel.ScorerAttribute].ToString();
            //var player = GetPlayerData(name, int.Parse(score), rank);

            //if (IsLocalPlayer(id))
            //{
            //player.IsLocal = true;
            //}

            //PlayersData.Add(player);
            //}
            //onEnd?.Invoke();
            //}
            //}
            //);
        }

                                                    /* GAMESPARK STUFF */
        private bool IsLocalPlayer(string id) => id == "";// GS.GSPlatform.UserId;

        private int GetLocalIndex() => PlayersData.FindIndex(p => p.IsLocal);

        private PlayerData GetLocalData() => PlayersData.Find(p => p.IsLocal);

        private void ShowLeaderboardProcessFirst()
        {
            LoseViewModel.OldIndex = GetLocalIndex();

            View.LeaderboardView.SetPlayerName(PlayerProfileModel.PlayerName);
            View.LeaderboardView.SetPlayerScore(0);

            UpdatePlayerViews(PlayersData);
        }

        private IEnumerator ShowNoInternetLeaderboardProcess()
        {
            ShowLeaderboardProcessFirst();
            if (PlayerProfileModel.PlayerScore <= 3000)
            {
                LoseViewModel.Delay = 0.8f;
                LoseViewModel.Speed = 4;
            }
            else
            {
                LoseViewModel.Delay = 1f;
                LoseViewModel.Speed = 5;
            }
            yield return null;
            yield return new WaitForSecondsRealtime(LeaderboardViewModel.InitAccumulatingDelay + 2f);

            yield return ScaleElement(View.LeaderboardView.SetScoreObjectScale, 1f, LeaderboardViewModel.TargetElementsScale);
            yield return AccumulateNumber(View.LeaderboardView.SetPlayerScore, 0, PlayerProfileModel.PlayerScore);
            yield return ScaleElement(View.LeaderboardView.SetScoreObjectScale, LeaderboardViewModel.TargetElementsScale, 1f);
            AudioSystem.PlayOnce(AudioID.CountFinal);
            yield return new WaitForSecondsRealtime(LoseViewModel.Delay);
            AudioSystem.PlayOnce(AudioID.Switch);
        }

        private IEnumerator ShowPreviewLeaderboardProcess()
        {
            ShowLeaderboardProcessFirst();
            UpdatePreviewPlayerView();

            if (PlayerProfileModel.PlayerScore > 3000)
            {
                LoseViewModel.Delay = 1f;
                LoseViewModel.Speed = 5;
            }
            else
            {
                LoseViewModel.Delay = 0.8f;
                LoseViewModel.Speed = 4;
            }

            yield return null;
            yield return new WaitForSecondsRealtime(LeaderboardViewModel.InitAccumulatingDelay + 2f);

            yield return ScaleElement(View.LeaderboardView.SetScoreObjectScale, 1f, LeaderboardViewModel.TargetElementsScale);

            yield return AccumulateNumber(View.LeaderboardView.SetPlayerScore, 0, PlayerProfileModel.PlayerScore);
            yield return ScaleElement(View.LeaderboardView.SetScoreObjectScale, LeaderboardViewModel.TargetElementsScale, 1f);
            AudioSystem.PlayOnce(AudioID.CountFinal);
            yield return new WaitForSecondsRealtime(LoseViewModel.Delay);
            AudioSystem.PlayOnce(AudioID.Switch);

            yield return new WaitForSecondsRealtime(LeaderboardViewModel.AccumulatingDelay);

            var localView = View.LeaderboardView.DataViews[LeaderboardViewModel.NumFirstPartPlayers];
            localView.SetPlayerScore(PlayerProfileModel.PlayerScore);

            UpdateUserView(false);
        }

        private IEnumerator ShowLeaderboardProcess()
        {
            ShowLeaderboardProcessFirst();

            if (PlayerProfileModel.PlayerScore > 3000)
            {
                LoseViewModel.Delay = 1f;
                LoseViewModel.Speed = 5;
            }
            else
            {
                LoseViewModel.Delay = 0.8f;
                LoseViewModel.Speed = 4;
            }

            var localData = GetLocalData();
            var localIndex = GetLocalIndex();

            yield return null;
            yield return new WaitForSecondsRealtime(LeaderboardViewModel.InitAccumulatingDelay + 2f);

            yield return ScaleElement(View.LeaderboardView.SetScoreObjectScale, 1f, LeaderboardViewModel.TargetElementsScale);
            yield return AccumulateNumber(View.LeaderboardView.SetPlayerScore, 0, PlayerProfileModel.PlayerScore);
            yield return ScaleElement(View.LeaderboardView.SetScoreObjectScale, LeaderboardViewModel.TargetElementsScale, 1f);

            AudioSystem.PlayOnce(AudioID.CountFinal);

            yield return new WaitForSecondsRealtime(LoseViewModel.Delay);

            AudioSystem.PlayOnce(AudioID.Switch);

            ResetLeaderboardData();

            PushScores
            (
                PlayerProfileModel.PlayerScore,
                () => LoadFirstScores
                (
                    () =>
                    {
                        var playerData = GetLocalData();
                        var rank = playerData.Score > 0 ? playerData.Rank : localData.Rank;
                        if (rank > LeaderboardViewModel.NumFirstPartPlayers)
                        {
                            LoadSecondScores(() => StartCoroutine(ShowLeaderboardProcessSecond()));
                        }
                        else
                        {
                            StartCoroutine(ShowLeaderboardProcessSecond());
                        }
                    }
                )
            );
        }

        private IEnumerator ShowLeaderboardProcessSecond()
        {
            if (PlayerProfileModel.PlayerScore >= 3000)
            {
                LoseViewModel.Delay = 1f;
                LoseViewModel.Speed = 5;
            }
            else
            {
                LoseViewModel.Delay = 0.8f;
                LoseViewModel.Speed = 4;
            }

            var localData = GetLocalData();

            yield return new WaitForSecondsRealtime(LeaderboardViewModel.AccumulatingTime + 2f);

            if (LoseViewModel.OldScore < localData.Score)
            {
                var localView = View.LeaderboardView.DataViews[LoseViewModel.OldIndex];
                AudioSystem.PlayOnce(AudioID.CountFinal);
                yield return new WaitForSecondsRealtime(LoseViewModel.Delay);
                AudioSystem.PlayOnce(AudioID.Switch);
                localView.SetPlayerScore(localData.Score);
            }

            yield return new WaitForSecondsRealtime(LeaderboardViewModel.AccumulatingTime + 2f);

            if (LoseViewModel.PlayerRank > localData.Rank)
            {
                var localView = View.LeaderboardView.DataViews[LoseViewModel.OldIndex];
                AudioSystem.PlayOnce(AudioID.CountFinal);
                yield return new WaitForSecondsRealtime(LoseViewModel.Delay);
                AudioSystem.PlayOnce(AudioID.Switch);
                localView.SetPlace(localData.Rank);

            }

            ResetLeaderboardView();

            UpdatePlayerViews(PlayersData);
        }

        private void UpdatePlayerViews(List<PlayerData> playersData)
        {
            PlayerDataView view;
            for (int i = 0; i < playersData.Count(); i++)
            {
                view = View.LeaderboardView.DataViews[i];

                view.SetPlace(playersData[i].Rank);
                view.SetActiveCrown(i < 3);

                switch (i)
                {
                    case 0:
                        view.SetCrownIcon(View.LeaderboardView.GoldCrown);
                        break;
                    case 1:
                        view.SetCrownIcon(View.LeaderboardView.SilverCrown);
                        break;
                    case 2:
                        view.SetCrownIcon(View.LeaderboardView.BronzeCrown);
                        break;
                }

                view.SetPlayerData(playersData[i]);

                if (playersData[i].IsLocal)
                {
                    view.SetActiveHighlight(true);
                }

                view.gameObject.SetActive(true);
            }
        }

        private void UpdatePreviewPlayerView()
        {
            var view = View.LeaderboardView.DataViews[LeaderboardViewModel.NumFirstPartPlayers];
            var player = GetPlayerProfileData();

            player.Score = 0;

            view.SetPlaceString("??");
            view.SetActiveCrown(false);

            view.SetPlayerData(player);
            view.SetActiveHighlight(true);
            view.gameObject.SetActive(true);
        }

        private void UpdatePreviewViewsFinal()
        {
            View.LeaderboardView.SetPlayerName(PlayerProfileModel.PlayerName);
            UpdatePlayerViews(PlayersData);
        }

        private void ResetLeaderboard()
        {
            ResetLeaderboardView();
            ResetLeaderboardData();
        }
        private void ResetLeaderboardView()
        {
            foreach (var d in View.LeaderboardView.DataViews)
            {
                d.SetActiveHighlight(false);
                d.gameObject.SetActive(false);
            }
        }
        private void ResetLeaderboardData() => PlayersData.Clear();

        private PlayerData GetRandomPlayerData() => new PlayerData()
        {
            Name = "player" + Random.Range(0, 100),
            Score = Random.Range(5, 1000),
        };

        private PlayerData GetPlayerData(string name, int score, int rank) => new PlayerData()
        {
            Name = name,
            Score = score,
            Rank = rank
        };

        private void UpdateUserView(bool authed) => View.LeaderboardView.ShowSubmitButton(!authed);

        private void OnButtonSubmit()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            View.AccountView = ViewsSystem.Show<GameSparksAccountView>(ViewConfigID.GameSparksAccount);
            OnAccountViewOpen();
        }

        private void OnAccountViewOpen()
        {
            GameSparksModel.OnUserLogined += OnAuthSuccess;
            View.AccountView.OnClose += OnAccountViewClosed;
        }

        private void OnAccountViewClose()
        {
            GameSparksModel.OnUserLogined -= OnAuthSuccess;
            View.AccountView.OnClose -= OnAccountViewClosed;
        }

        private void OnAuthSuccess()
        {
            UpdateUserView(true);
            View.LeaderboardView.SetPlayerName(PlayerProfileModel.PlayerName);

            if (LeaderboardViewModel.HasUpdatedAfterLogin)
            {
                return;
            }

            ResetLeaderboard();
            LoseViewModel.PlayerRank = 0;
            LoseViewModel.OldScore = 0;
            LoseViewModel.OldRank = 0;
            LoseViewModel.OldIndex = 0;
            LoadHighScoreAndUpdateView();
            LeaderboardViewModel.HasUpdatedAfterLogin = true;
            OnAccountViewClosed();
        }

        private void OnAccountViewClosed() => OnAccountViewClose();

        private PlayerData GetPlayerProfileData() => new PlayerData
        {
            Name = PlayerProfileModel.PlayerName,
            Icon = PlayerProfileModel.PlayerAvatar,
            Score = PlayerProfileModel.PlayerScore,
            IsLocal = true
        };

        private IEnumerator AccumulateNumber(Action<int> number, float initNumber, int targetNumber, bool canDowngrade = false)
        {
            if (!canDowngrade && initNumber > targetNumber)
            {
                number?.Invoke(targetNumber);
                yield break;
            }

            number?.Invoke((int)initNumber);

            const float EPSION = 0.5f; //??

            while (Mathf.Abs(targetNumber - initNumber) > EPSION)
            {
                initNumber = Mathf.Lerp(initNumber, targetNumber, LoseViewModel.Speed * Time.unscaledDeltaTime);//
                number((int)initNumber + 1);
                yield return null;
            }

            number?.Invoke(targetNumber);
        }

        private IEnumerator ScaleElement(Action<float> scale, float initScale, float targetScale)
        {
            scale?.Invoke(initScale);

            float speed = 5f;

            while (!Mathf.Approximately(initScale, targetScale))
            {
                initScale = Mathf.MoveTowards(initScale, targetScale, speed * Time.unscaledDeltaTime);
                scale?.Invoke(initScale);
                yield return null;
            }
            scale?.Invoke(targetScale);
        }
    }
}
