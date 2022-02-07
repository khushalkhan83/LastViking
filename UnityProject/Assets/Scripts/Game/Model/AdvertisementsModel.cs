using CodeStage.AntiCheat.ObscuredTypes;
using Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    [Flags]
    public enum AdType
    {
        None = 0,
        UnityAds = 1,
        Admob = 2
    }

    public enum VideoResult
    {
        Ended,
        Failed
    }

    public enum RewardedVideoResult
    {
        Ended,
        Failed
    }

    public class AdvertisementsModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [Header("Testing")]
        [SerializeField] private ObscuredBool _isTestingMode;
        [SerializeField] private ObscuredString _androidTestDeviceId;
        [SerializeField] private ObscuredString _iosTestDeviceId;

        [Header("Ads types")]
        [SerializeField] private ObscuredBool _isInterstitial;
        [SerializeField] private ObscuredBool _isPrechacheInterstitial;
        [SerializeField] private ObscuredBool _isRewardedVideo;
        [SerializeField] private ObscuredBool _isBaner;
        [SerializeField] private ObscuredBool _isPrecacheBaner;

        [Header("Admob")]
        [SerializeField] private ObscuredString _admobApplicationKeyAndroid;
        [SerializeField] private ObscuredString _admobApplicationKeyIOs;
        [Space]
        [SerializeField] private ObscuredString _admobBannerIdAndroid;
        [SerializeField] private ObscuredString _admobBannerIdIos;
        [Space]
        [SerializeField] private ObscuredString _admobInterstitialIdAndroid;
        [SerializeField] private ObscuredString _admobInterstitialIdIos;
        [Space]
        [SerializeField] private ObscuredString _admobRewardedIdAndroid;
        [SerializeField] private ObscuredString _admobRewardedIdIos;

        [Header("Unity Ads")]
        [SerializeField] private ObscuredString _unityApplicationKeyAndroid;
        [SerializeField] private ObscuredString _unityApplicationKeyIOs;

        [Header("Mediation")]
        [ObscuredID(typeof(AdType)), SerializeField] private ObscuredInt _preferredAd;
        [SerializeField] private ObscuredBool _setPreferredAdsOnSuccess;


#pragma warning restore 0649
        #endregion

        public readonly string AdmobTestApplicationKeyAndroid = "ca-app-pub-3940256099942544~3347511713";
        public readonly string AdmobTestApplicationKeyIOs = "ca-app-pub-3940256099942544~1458002511";
                               
        public readonly string AdmobTestBannerIdAndroid = "ca-app-pub-3940256099942544/6300978111";
        public readonly string AdmobTestBannerIdIos = "ca-app-pub-3940256099942544/2934735716";
                               
        public readonly string AdmobTestInterstitialIdAndroid = "ca-app-pub-3940256099942544/1033173712";
        public readonly string AdmobTestInterstitialIdIos = "ca-app-pub-3940256099942544/4411468910";
                               
        public readonly string AdmobTestRewardedIdAndroid = "ca-app-pub-3940256099942544/5224354917";
        public readonly string AdmobTestRewardedIdIos = "ca-app-pub-3940256099942544/1712485313";

        public bool IsTestingMode => _isTestingMode;
        public string AndroidTestDeviceId => _androidTestDeviceId;
        public string IosTestDeviceId => _iosTestDeviceId;

        public bool IsInterstitial => _isInterstitial;
        public bool IsPrechacheInterstitial => _isPrechacheInterstitial;
        public bool IsRewardedVideo => _isRewardedVideo;
        public bool IsBaner => _isBaner;
        public bool IsPrecacheBaner => _isPrecacheBaner;
        
        public string UnityApplicationKey =>
#if UNITY_ANDROID
            _unityApplicationKeyAndroid;
#elif UNITY_IOS
            _unityApplicationKeyIOs;
#else
        "";
#endif
        
        private string AdmobReleaseApplicationKey =>
#if UNITY_ANDROID
            _admobApplicationKeyAndroid;
#elif UNITY_IOS
            _admobApplicationKeyIOs;
#else
        "";
#endif

        private string AdmobTestApplicationKey =>
#if UNITY_ANDROID
            AdmobTestApplicationKeyAndroid;
#elif UNITY_IOS
            AdmobTestApplicationKeyIOs;
#else
        "";
#endif

        public string AdmobApplicationKey => IsTestingMode ? AdmobTestApplicationKey : AdmobReleaseApplicationKey;

        private string AdmobReleaseBannerId =>
#if UNITY_ANDROID
            _admobBannerIdAndroid;
#elif UNITY_IOS
            _admobBannerIdIos;
#else
        "";
#endif

        private string AdmobTestBannerId =>
#if UNITY_ANDROID
            AdmobTestBannerIdAndroid;
#elif UNITY_IOS
            AdmobTestBannerIdIos;
#else
        "";
#endif

        public string AdmobBannerId => IsTestingMode ? AdmobTestBannerId : AdmobReleaseBannerId;

        private string AdmobReleaseInterstitialId =>
#if UNITY_ANDROID
            _admobInterstitialIdAndroid;
#elif UNITY_IOS
            _admobInterstitialIdIos;
#else
        "";
#endif

        private string AdmobTestInterstitialId =>
#if UNITY_ANDROID
            AdmobTestInterstitialIdAndroid;
#elif UNITY_IOS
            AdmobTestInterstitialIdIos;
#else
        "";
#endif

        public string AdmobInterstitialId => IsTestingMode ? AdmobTestInterstitialId : AdmobReleaseInterstitialId;

        private string AdmobReleaseRewardedId =>
#if UNITY_ANDROID
            _admobRewardedIdAndroid;
#elif UNITY_IOS
            _admobRewardedIdIos;
#else
        "";
#endif

        private string AdmobTestRewardedId =>
#if UNITY_ANDROID
            AdmobTestRewardedIdAndroid;
#elif UNITY_IOS
            AdmobTestRewardedIdIos;
#else
        "";
#endif

        public string AdmobRewardedId => IsTestingMode ? AdmobTestRewardedId : AdmobReleaseRewardedId;

        public AdType PreferredAd => (AdType) (int) _preferredAd;
        public bool SetPreferredAdsOnSuccess => _setPreferredAdsOnSuccess;

        public bool IsHasCallbackRewardedVideo { get; private set; }
        public RewardedVideoResult CallbackRewardedVideoResult { get; private set; }

        public bool IsHasCallbackVideo { get; private set; }
        public VideoResult CallbackVideoResult { get; private set; }
        
        public bool IsInitialized { get; private set; }

        public bool IsLoadingVideo { get; private set; }
        public bool IsLoadingRewardedVideo { get; private set; }
        public bool IsVideoFailedToLoad { get; set; }
        public bool IsRewardedVideoFailedToLoad { get; set; }

        public event Action<VideoResult> OnResultVideo;
        public event Action<RewardedVideoResult> OnResultRewardedVideo;

        public UniqueAction<AdType> OnShowVideoAction { get; } = new UniqueAction<AdType>();
        public UniqueAction<AdType> OnShowBannerAction { get; } = new UniqueAction<AdType>();
        public UniqueAction<AdType> OnHideBannerAction { get; } = new UniqueAction<AdType>();
        public UniqueAction<AdType> OnShowRewardedVideoAction { get; } = new UniqueAction<AdType>();
        public UniqueAction<AdType> OnBeginLoadVideoAction { get; } = new UniqueAction<AdType>();
        public UniqueAction<AdType> OnBeginLoadRewardedVideoAction { get; } = new UniqueAction<AdType>();
        public UniqueAction<AdType> OnEndLoadVideoAction { get; } = new UniqueAction<AdType>();
        public UniqueAction<AdType> OnEndLoadRewardedVideoAction { get; } = new UniqueAction<AdType>();

        public event Action OnChangeIsLoadedVideo;
        public event Action OnChangeIsLoadedRewardedVideo;

        public event Action OnInitialized;

        public event Func<bool> ResultIsReadyAdmobVideo;
        public event Func<bool> ResultIsReadyAdmobRewardedVideo;

        public event Func<bool> ResultIsReadyVideo; //maybe change to property with preload state
        public event Func<bool> ResultIsReadyRewardedVideo; //maybe change to property with preload state

        public AdType CurrentAdType { get; private set; }

        public bool IsReadyVideo
        {
            get
            {
                switch (CurrentAdType)
                {
                    case AdType.UnityAds: return ResultIsReadyVideo?.Invoke() ?? false;
                    case AdType.Admob: return ResultIsReadyAdmobVideo?.Invoke() ?? false;
                }
                return false;
            }
        }

        public bool IsReadyRewardedVideo
        {
            get
            {
                switch (CurrentAdType)
                {
                    case AdType.UnityAds: return ResultIsReadyRewardedVideo?.Invoke() ?? false;
                    case AdType.Admob: return ResultIsReadyAdmobRewardedVideo?.Invoke() ?? false;
                }
                return false;
            }
        }

        public void Initialize()
        {
            if (IsInitialized) return;

            IsInitialized = true;
            CurrentAdType = PreferredAd;
            OnInitialized?.Invoke();
        }

        private void SwitchCurrentAdType()
        {
            if (CurrentAdType == AdType.UnityAds)
                CurrentAdType = AdType.Admob;
            else if (CurrentAdType == AdType.Admob)
                CurrentAdType = AdType.UnityAds;
        }

        private void SetDefaultCurrentAdType()
        {
            CurrentAdType = PreferredAd;
        }

        public void SetCallbackRewardedVideo(RewardedVideoResult rewardedVideoResult)
        {
            IsHasCallbackRewardedVideo = true;
            CallbackRewardedVideoResult = rewardedVideoResult;
        }

        public void SetCallbackVideo(VideoResult videoResult)
        {
            IsHasCallbackVideo = true;
            CallbackVideoResult = videoResult;
        }

        public void ClearCallbackRewardedVideo() => IsHasCallbackRewardedVideo = false;
        public void ClearCallbackVideo() => IsHasCallbackVideo = false;

        public void ShowBanner() => OnShowBannerAction.Invoke(AdType.Admob);

        public void HideBanner() => OnHideBannerAction.Invoke(AdType.Admob);

        public void ShowVideo()
        {
            OnShowVideoAction.Invoke(CurrentAdType);
        }

        public void ShowRewardedVideo()
        {
            OnShowRewardedVideoAction.Invoke(CurrentAdType);
        }

        public void ResultVideo(VideoResult videoResult)
        {
            IsLoadingVideo = false;
            IsHasCallbackVideo = false;
            OnResultVideo?.Invoke(videoResult);

            if (videoResult == VideoResult.Failed)
                SwitchCurrentAdType();
            else if (SetPreferredAdsOnSuccess)
                SetDefaultCurrentAdType();
        }

        public void ResultRewardedVideo(RewardedVideoResult rewardedVideoResult)
        {
            IsLoadingRewardedVideo = false;
            IsHasCallbackRewardedVideo = false;
            OnResultRewardedVideo?.Invoke(rewardedVideoResult);

            if (rewardedVideoResult == RewardedVideoResult.Failed)
                SwitchCurrentAdType();
            else if (SetPreferredAdsOnSuccess)
                SetDefaultCurrentAdType();
        }

        public void SetIsReadyVideo(AdType adType, Func<bool> func)
        {
            if (adType == AdType.UnityAds)
                ResultIsReadyVideo = func;
            else if (adType == AdType.Admob)
                ResultIsReadyAdmobVideo = func;
        }

        public void SetIsReadyRewardedVideo(AdType adType, Func<bool> func)
        {
            if (adType == AdType.UnityAds)
                ResultIsReadyRewardedVideo = func;
            else if (adType == AdType.Admob)
                ResultIsReadyAdmobRewardedVideo = func;
        }

        public void LoadVideo()
        {
            if (!(IsLoadingVideo || IsReadyVideo))
            {
                OnBeginLoadVideoAction.Invoke(CurrentAdType);
                BeginLoadingVideo();
            }
        }

        public void LoadRewardedVideo()
        {
            if (!(IsLoadingRewardedVideo || IsReadyRewardedVideo))
            {
                OnBeginLoadRewardedVideoAction.Invoke(CurrentAdType);
                BeginLoadingRewardedVideo();
            }
        }

        public void LoadedVideo()
        {
            SetIsLoadedVideo(true);
            StopLoadingVideo();
            OnEndLoadVideoAction.Invoke(CurrentAdType);
        }

        public void LoadedRewardedVideo()
        {
            SetIsLoadedRewardedVideo(true);
            StopLoadingRewardedVideo();
            OnEndLoadRewardedVideoAction.Invoke(CurrentAdType);
        }

        private void ClearVideo() => SetIsLoadedVideo(false);
        private void ClearRewardedVideo() => SetIsLoadedRewardedVideo(false);

        private void BeginLoadingVideo() => SetIsLoadingVideo(true);
        private void StopLoadingVideo() => SetIsLoadingVideo(false);

        private void BeginLoadingRewardedVideo() => SetIsLoadingRewardedVideo(true);
        private void StopLoadingRewardedVideo() => SetIsLoadingRewardedVideo(false);

        private void SetIsLoadingVideo(bool isLoaging) => IsLoadingVideo = isLoaging;
        private void SetIsLoadingRewardedVideo(bool isLoaging) => IsLoadingRewardedVideo = isLoaging;

        private void SetIsLoadedVideo(bool isLoaded) => OnChangeIsLoadedVideo?.Invoke();
        private void SetIsLoadedRewardedVideo(bool isLoaded) => OnChangeIsLoadedRewardedVideo?.Invoke();
    }
}
