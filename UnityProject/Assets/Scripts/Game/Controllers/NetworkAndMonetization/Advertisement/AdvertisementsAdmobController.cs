using Core;
using Core.Controllers;
using Game.Models;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class AdvertisementsAdmobController : IAdvertisementsAdmobController, IController
    {
        public enum AdvertisementID
        {
            video,
            rewardedVideo,
            banner
        }

        [Inject] public AdvertisementsModel AdvertisementsModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public ApplicationCallbacksModel ApplicationCallbacksModel { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }

        private static void InitializeAdmob(string gameId) => MobileAds.Initialize(gameId);

        private bool IsLoadedAdmobBanner() => true;
        private bool IsLoadedAdmobRewardedVideo() => AdmobRewarded != null && AdmobRewarded.IsLoaded();
        private bool IsLoadedAdmobInterstitial() => AdmobInterstitial != null && AdmobInterstitial.IsLoaded();

        private void ShowAdmobBanner() => RequestAdmobBanner();
        private void ShowAdmobVideo() => AdmobInterstitial.Show();
        private void ShowAdmobRewardedVideo() => AdmobRewarded.Show();

        private void HideAdmobBanner() => AdmobBanner?.Hide();

        private void CacheAdmobBanner() => RequestAdmobBanner();
        private void CacheAdmobVideo() => RequestAdmobInterstitial();
        private void CacheAdmobRewardedVideo() => CreateAndLoadAdmobRewarded();

        // + from appodeal
        private void SetRewardedVideoCalbacks() { }
        private void SetVideoCallbacks() { }
        private void DisableNetwork(string network) { }
        private void DisableAllCachce() { }
        private void EnableCache(int adType) { }
        private void DisableAllPrecachce() { }
        private void EnablePrecache(int adType) { }
        // - from appodeal

        public AdType AdType => AdType.Admob;

        public BannerView AdmobBanner { get; private set; }
        public InterstitialAd AdmobInterstitial { get; private set; }
        public RewardedAd AdmobRewarded { get; private set; }

        void IController.Enable()
        {
            Initialize();
            AdvertisementsModel.SetIsReadyVideo(AdType, GetIsReadyAdmobVideo);
            AdvertisementsModel.SetIsReadyRewardedVideo(AdType, GetIsReadyAdmobRewardedVideo);

            AdvertisementsModel.OnShowVideoAction.AddListener(AdType, OnShowVideoHandler);
            AdvertisementsModel.OnShowBannerAction.AddListener(AdType, OnShowBannerHandler);
            AdvertisementsModel.OnHideBannerAction.AddListener(AdType, OnHideBannerHandler);
            AdvertisementsModel.OnShowRewardedVideoAction.AddListener(AdType, OnShowRewardedVideoHandler);
            AdvertisementsModel.OnBeginLoadVideoAction.AddListener(AdType, OnBeginLoadVideoHandler);
            AdvertisementsModel.OnBeginLoadRewardedVideoAction.AddListener(AdType, OnBeginLoadRewardedVideoHandler);
            GameUpdateModel.OnUpdate += OnUpdate;

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
            GameUpdateModel.OnUpdate -= OnUpdate;

            ApplicationCallbacksModel.ApplicationFocus -= OnApplicationFocus;
            ApplicationCallbacksModel.ApplicationPause -= OnApplicationPause;
        }

        private void OnBeginLoadVideoHandler() => CacheAdmobVideo();

        private void OnBeginLoadRewardedVideoHandler() => CacheAdmobRewardedVideo();

        private AdRequest CreateAdmobAdRequest()
        {
            var builder = new AdRequest.Builder();
            if (AdvertisementsModel.IsTestingMode)
            {
                builder.AddTestDevice(AdRequest.TestDeviceSimulator)
                    .AddTestDevice(AdvertisementsModel.AndroidTestDeviceId)
                    .AddTestDevice(AdvertisementsModel.IosTestDeviceId);
            }
            return builder.Build();
        }

        private void RequestAdmobBanner()
        {
            if (AdmobBanner != null)
            {
                AdmobBanner.Destroy();
            }

            AdmobBanner = new BannerView(AdvertisementsModel.AdmobBannerId, AdSize.Banner, AdPosition.Bottom);
            AdmobBanner.LoadAd(CreateAdmobAdRequest());
        }

        private void RequestAdmobInterstitial()
        {
            if (AdmobInterstitial != null)
            {
                AdmobInterstitial.Destroy();
            }

            AdmobInterstitial = new InterstitialAd(AdvertisementsModel.AdmobInterstitialId);

            AdmobInterstitial.OnAdLoaded += AdmobInterstitial_OnAdLoaded;
            AdmobInterstitial.OnAdFailedToLoad += AdmobInterstitial_OnAdFailedToLoad;
            AdmobInterstitial.OnAdClosed += AdmobInterstitial_OnAdClosed;

            AdmobInterstitial.LoadAd(CreateAdmobAdRequest());
        }

        private void AdmobInterstitial_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            onInterstitialFailedToLoad();
        }

        private void AdmobInterstitial_OnAdClosed(object sender, EventArgs e)
        {
            onInterstitialClosed();
        }

        private void AdmobInterstitial_OnAdLoaded(object sender, EventArgs e)
        {
            onInterstitialLoaded(true);
        }

        private void CreateAndLoadAdmobRewarded()
        {
            AdmobRewarded = new RewardedAd(AdvertisementsModel.AdmobRewardedId);

            AdmobRewarded.OnAdLoaded += HandleRewardedAdLoaded;
            AdmobRewarded.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
            AdmobRewarded.OnAdFailedToShow += HandleRewardedAdFailedToShow;
            AdmobRewarded.OnUserEarnedReward += HandleUserEarnedReward;
            AdmobRewarded.OnAdClosed += HandleRewardedAdClosed;

            AdRequest request = CreateAdmobAdRequest();
            AdmobRewarded.LoadAd(request);
        }

        private void HandleRewardedAdClosed(object sender, EventArgs e)
        {
            if (!AdvertisementsModel.IsHasCallbackRewardedVideo)
            {
                onRewardedVideoClosed(false);
            }
        }

        private void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs e)
        {
            onRewardedVideoFailedToLoad();
        }

        private void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs e)
        {
            onRewardedVideoFailedToLoad();
        }

        private void HandleUserEarnedReward(object sender, Reward e)
        {
            onRewardedVideoFinished();
        }

        private void HandleRewardedAdLoaded(object sender, EventArgs e)
        {
            onRewardedVideoLoaded(true);
        }

        private void Initialize()
        {
            AdvertisementsModel.Initialize();

            MobileAds.SetiOSAppPauseOnBackground(true);

            InitializeAdmob(AdvertisementsModel.UnityApplicationKey);

            CreateAndLoadAdmobRewarded();
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

        private bool GetIsReadyAdmobVideo() =>
#if UNITY_EDITOR 
            true
#else
            IsLoadedAdmobInterstitial()
#endif
            ;

        private bool GetIsReadyAdmobRewardedVideo() =>
#if UNITY_EDITOR
            true
#else
            IsLoadedAdmobRewardedVideo()
#endif
            ;

        private bool GetIsReadyAdmobBaner() =>
#if UNITY_EDITOR
            true
#else 
            IsLoadedAdmobBanner()
#endif
            ;

        private void OnShowBannerHandler() => ShowAdmobBanner();

        private void OnHideBannerHandler() => HideAdmobBanner();

        private void OnShowVideoHandler()
        {
#if UNITY_EDITOR
            AdvertisementsModel.ResultVideo(VideoResult.Ended);
#else
            if (GetIsReadyAdmobVideo())
            {
                ShowAdmobVideo();
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
            if (GetIsReadyAdmobRewardedVideo())
            {
                ShowAdmobRewardedVideo();
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

        public void onInterstitialFailedToLoad() => AdvertisementsModel.IsVideoFailedToLoad = true;

        public void onInterstitialShown() { }

        public void onInterstitialClosed()
        {
#if UNITY_ANDROID
            AdvertisementsModel.SetCallbackVideo(VideoResult.Ended);
#else
          Result(VideoResult.Ended);
#endif
        }

        public void onInterstitialClicked() { }

        public void onInterstitialExpired() => AdvertisementsModel.ResultVideo(VideoResult.Failed);

        //RewardedVideo

        public void onRewardedVideoLoaded(bool precache) => AdvertisementsModel.LoadedRewardedVideo();

        public void onRewardedVideoFailedToLoad() => AdvertisementsModel.IsRewardedVideoFailedToLoad = true;

        public void onRewardedVideoShown() { }

        public void onRewardedVideoFinished()
        {
#if UNITY_ANDROID
            AdvertisementsModel.SetCallbackRewardedVideo(RewardedVideoResult.Ended);
#else
          Result(RewardedVideoResult.Ended);
#endif
        }

        public void onRewardedVideoClosed(bool finished)
        {
            if (finished) return;

#if UNITY_ANDROID
            AdvertisementsModel.SetCallbackRewardedVideo(RewardedVideoResult.Failed);
#else
          Result(RewardedVideoResult.Failed);
#endif
        }

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

        private void OnUpdate()
        {
            if (AdvertisementsModel.IsVideoFailedToLoad)
            {
                AdvertisementsModel.IsVideoFailedToLoad = false;
                Result(VideoResult.Failed);
            }
            if (AdvertisementsModel.IsRewardedVideoFailedToLoad)
            {
                AdvertisementsModel.IsRewardedVideoFailedToLoad = false;
                Result(RewardedVideoResult.Failed);
            }
        }
    }
}
