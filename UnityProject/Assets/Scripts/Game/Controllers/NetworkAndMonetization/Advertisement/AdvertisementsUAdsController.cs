using Core;
using Core.Controllers;
using Game.Models;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Monetization;

namespace Game.Controllers
{
    public class AdvertisementsUAdsController : IAdvertisementsUAdsController, IController
    {
        public enum AdvertisementID
        {
            video,
            rewardedVideo,
            banner
        }

        [Inject] public AdvertisementsModel AdvertisementsModel { get; private set; }
        [Inject] public ApplicationCallbacksModel ApplicationCallbacksModel { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }

        private ShowAdPlacementContent BannerPlacementContent => GetShowPlacementContent(GetPlacementId(AdvertisementID.banner));
        private ShowAdPlacementContent VideoPlacementContent => GetShowPlacementContent(GetPlacementId(AdvertisementID.video));
        private ShowAdPlacementContent RewardedVideoPlacementContent => GetShowPlacementContent(GetPlacementId(AdvertisementID.rewardedVideo));

        private static void InitializeUnityAds(string gameId, bool testMode) => Monetization.Initialize(gameId, testMode);

        private bool IsLoadedUAdsInterstitial() => IsLoadedPlacement(GetPlacementId(AdvertisementID.video));
        private bool IsLoadedUAdsRewardedVideo() => IsLoadedPlacement(GetPlacementId(AdvertisementID.rewardedVideo));
        private bool IsLoadedUAdsBannerBottom() => IsLoadedPlacement(GetPlacementId(AdvertisementID.banner));

        private void ShowUAdsBannerBottom() => BannerPlacementContent?.Show();
        private void ShowUAdsVideo() => VideoPlacementContent.Show(ShowVideoCallback);
        private void ShowUAdsRewardedVideo() => RewardedVideoPlacementContent.Show(ShowRewardedVideoCallback);

        private void HideUAdsBannerBottom() { }

        private void CacheUAdsVideo() { }
        private void CasheUAdsRewardedVideo() { }

        // + from appodeal
        private void SetRewardedVideoCalbacks() { }
        private void SetVideoCallbacks() { }
        private void DisableNetwork(string network) { }
        private void DisableAllCachce() { }
        private void EnableCache(int adType) { }
        private void DisableAllPrecachce() { }
        private void EnablePrecache(int adType) { }
        // - from appodeal

        public AdType AdType => AdType.UnityAds;

        private string GetPlacementId(AdvertisementID advertisementID) => advertisementID.ToString();

        private ShowAdPlacementContent GetShowPlacementContent(string placementId) => Monetization.GetPlacementContent(placementId) as ShowAdPlacementContent;

        void ShowVideoCallback(ShowResult x)
        {
            if (x == ShowResult.Finished)
            {
                onInterstitialShown();
                onInterstitialClosed();
            }
            else if (x == ShowResult.Skipped)
            {
                onInterstitialClosed();
            }
            else if (x == ShowResult.Failed)
            {
                onInterstitialFailedToLoad();
            }
        }

        void ShowRewardedVideoCallback(ShowResult x)
        {
            if (x == ShowResult.Finished)
            {
                onRewardedVideoFinished();
            }
            else if (x == ShowResult.Skipped)
            {
                onRewardedVideoClosed(false);
            }
            else if (x == ShowResult.Failed)
            {
                onRewardedVideoFailedToLoad();
            }
        }

        private bool IsLoadedPlacement(string placementId) => Monetization.IsReady(placementId);

        void IController.Enable()
        {
            Initialize();
            AdvertisementsModel.SetIsReadyVideo(AdType, GetIsReadyUAdsVideo);
            AdvertisementsModel.SetIsReadyRewardedVideo(AdType, GetIsReadyUAdsRewardedVideo);

            AdvertisementsModel.OnShowVideoAction.AddListener(AdType, OnShowVideoHandler);
            AdvertisementsModel.OnShowBannerAction.AddListener(AdType, OnShowBannerHandler);
            AdvertisementsModel.OnHideBannerAction.AddListener(AdType, OnHideBannerHandler);
            AdvertisementsModel.OnShowRewardedVideoAction.AddListener(AdType, OnShowRewardedVideoHandler);
            AdvertisementsModel.OnBeginLoadVideoAction.AddListener(AdType, OnBeginLoadVideoHandler);
            AdvertisementsModel.OnBeginLoadRewardedVideoAction.AddListener(AdType, OnBeginLoadRewardedVideoHandler);
            AdvertisementsModel.OnEndLoadVideoAction.AddListener(AdType, OnEndLoadVideoHandler);
            AdvertisementsModel.OnEndLoadRewardedVideoAction.AddListener(AdType, OnEndLoadRewardedVideoHandler);

            ApplicationCallbacksModel.ApplicationFocus += OnApplicationFocus;
            ApplicationCallbacksModel.ApplicationPause += OnApplicationPause;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            AdvertisementsModel.OnShowVideoAction.RemoveListener(AdType, OnShowVideoHandler);
            AdvertisementsModel.OnShowBannerAction.RemoveListener(AdType, OnShowBannerHandler);
            AdvertisementsModel.OnHideBannerAction.RemoveListener(AdType, OnHideBannerHandler);
            AdvertisementsModel.OnShowRewardedVideoAction.RemoveListener(AdType, OnShowRewardedVideoHandler);
            AdvertisementsModel.OnBeginLoadVideoAction.RemoveListener(AdType, OnBeginLoadVideoHandler);
            AdvertisementsModel.OnBeginLoadRewardedVideoAction.RemoveListener(AdType, OnBeginLoadRewardedVideoHandler);
            AdvertisementsModel.OnEndLoadVideoAction.RemoveListener(AdType, OnEndLoadVideoHandler);
            AdvertisementsModel.OnEndLoadRewardedVideoAction.RemoveListener(AdType, OnEndLoadRewardedVideoHandler);

            ApplicationCallbacksModel.ApplicationFocus -= OnApplicationFocus;
            ApplicationCallbacksModel.ApplicationPause -= OnApplicationPause;
        }

        private void OnBeginLoadVideoHandler() => CacheUAdsVideo();

        private void OnBeginLoadRewardedVideoHandler() => CasheUAdsRewardedVideo();

        private void OnEndLoadVideoHandler() => CacheUAdsVideo();

        private void OnEndLoadRewardedVideoHandler() => CasheUAdsRewardedVideo();

        private void Initialize()
        {
            {//before initialization
                DisableAllCachce();
                if (AdvertisementsModel.IsBaner)
                {
                    EnableCache((int)AdvertisementID.banner);
                }

                DisableAllPrecachce();
                if (IsHasPrecacheTypes)
                {
                    EnablePrecache(-1);
                }
            }

            AdvertisementsModel.Initialize();

            InitializeUnityAds(AdvertisementsModel.UnityApplicationKey, AdvertisementsModel.IsTestingMode);

            Monetization.onPlacementContentReady += OnPlacementContentReady;
            Monetization.onPlacementContentStateChange += OnPlacementContentStateChange;

            {//After initialization
                if (AdvertisementsModel.IsInterstitial)
                {
                    SetVideoCallbacks();
                }

                if (AdvertisementsModel.IsRewardedVideo)
                {
                    SetRewardedVideoCalbacks();
                }
            }
        }

        private void OnPlacementContentStateChange(object sender, PlacementContentStateChangeEventArgs e)
        {
            if (e.newState == PlacementContentState.NotAvailable || e.newState == PlacementContentState.NoFill)
            {
                if (AdvertisementsModel.IsLoadingVideo && e.placementId == GetPlacementId(AdvertisementID.video))
                {
                    onInterstitialFailedToLoad();
                }
                if (AdvertisementsModel.IsLoadingRewardedVideo && e.placementId == GetPlacementId(AdvertisementID.rewardedVideo))
                {
                    onRewardedVideoFailedToLoad();
                }
            }
        }

        private void OnPlacementContentReady(object sender, PlacementContentReadyEventArgs e)
        {
            if (e.placementId == GetPlacementId(AdvertisementID.video))
            {
                onInterstitialLoaded(false);
            }
            if (e.placementId == GetPlacementId(AdvertisementID.rewardedVideo))
            {
                onRewardedVideoLoaded(false);
            }
        }

        private void OnApplicationFocus(bool isHasFocus) => CallBackProcessing(isHasFocus);

        private void OnApplicationPause(bool isPause) => CallBackProcessing(!isPause);

        private void CallBackProcessing(bool isInApplication)
        {
            if (isInApplication)
            {
                if (AdvertisementsModel.IsHasCallbackVideo)
                {
                    AdvertisementsModel.ResultVideo(AdvertisementsModel.CallbackVideoResult);
                }

                if (AdvertisementsModel.IsHasCallbackRewardedVideo)
                {
                    AdvertisementsModel.ResultRewardedVideo(AdvertisementsModel.CallbackRewardedVideoResult);
                }
            }
        }

        private bool GetIsReadyUAdsVideo() =>
#if UNITY_EDITOR 
            true
#else
            IsLoadedUAdsInterstitial()
#endif
            ;

        private bool GetIsReadyUAdsRewardedVideo() =>
#if UNITY_EDITOR
            true
#else
            IsLoadedUAdsRewardedVideo()
#endif
            ;

        private bool GetIsReadyUAdsBaner() =>
#if UNITY_EDITOR
            true
#else 
            IsLoadedUAdsBannerBottom()
#endif
            ;

        private void OnShowBannerHandler() => ShowUAdsBannerBottom();

        private void OnHideBannerHandler() => HideUAdsBannerBottom();

        private void OnShowVideoHandler()
        {
#if UNITY_EDITOR
            AdvertisementsModel.ResultVideo(VideoResult.Ended);
#else
            if (GetIsReadyUAdsVideo())
            {
                ShowUAdsVideo();
            }
            else
            {
                AdvertisementsModel.ResultVideo(VideoResult.Failed);
            }
#endif
        }

        private void OnShowRewardedVideoHandler()
        {
#if UNITY_EDITOR
            AdvertisementsModel.ResultRewardedVideo(RewardedVideoResult.Ended);
#else
            if (GetIsReadyUAdsRewardedVideo())
            {
                ShowUAdsRewardedVideo();
            }
            else
            {
                AdvertisementsModel.ResultRewardedVideo(RewardedVideoResult.Failed);
            }
#endif
        }

        public bool IsHasPrecacheTypes => false;

        //Interstitial

        public void onInterstitialLoaded(bool isPrecache) => AdvertisementsModel.LoadedVideo();

        public void onInterstitialFailedToLoad() => AdvertisementsModel.ResultVideo(VideoResult.Failed);

        public void onInterstitialShown() { }

        public void onInterstitialClosed()
        {
#if UNITY_ANDROID
            AdvertisementsModel.ResultVideo(VideoResult.Ended);
#else
          Result(VideoResult.Ended);
#endif
        }

        public void onInterstitialClicked() { }

        public void onInterstitialExpired() => AdvertisementsModel.ResultVideo(VideoResult.Failed);

        //RewardedVideo

        public void onRewardedVideoLoaded(bool precache) => AdvertisementsModel.LoadedRewardedVideo();

        public void onRewardedVideoFailedToLoad() => Result(RewardedVideoResult.Failed);

        public void onRewardedVideoShown() { }

        public void onRewardedVideoFinished()
        {
#if UNITY_ANDROID
            AdvertisementsModel.ResultRewardedVideo(RewardedVideoResult.Ended);
#else
          Result(RewardedVideoResult.Ended);
#endif
        }

        public void onRewardedVideoClosed(bool finished) { }

        public void onRewardedVideoExpired() => AdvertisementsModel.ResultRewardedVideo(RewardedVideoResult.Failed);

        public void Result(RewardedVideoResult result)
        {
            CoroutineModel.InitCoroutine(DelayedAction(() => AdvertisementsModel.ResultRewardedVideo(result)));
        }

        public void Result(VideoResult result)
        {
            CoroutineModel.InitCoroutine(DelayedAction(() => AdvertisementsModel.ResultVideo(result)));
        }

        public IEnumerator DelayedAction(Action action)
        {
            yield return null;
            action();
        }
    }
}
