using System;
using UnityEngine;

namespace Game.Models
{
    public class FishingModel : MonoBehaviour
    {
        public enum FishingState { none, biting, pulling,reciveFish }

        #region Config
        [SerializeField, Tooltip("Start fishing distance")]
        float _startFishingDistance = 10f;
        [SerializeField, Tooltip("Minimum water depth for fishing")]
        float _fishingMinDepth = 0.8f;
        [SerializeField, Tooltip("Fishing Layer Mask")]
        LayerMask _fishingLayerMask = default;
        [SerializeField, Tooltip("LayerMask to detect sea bottom")]
        LayerMask _fishingBottomMask = default;

        [Header("Bite Phase")]
        [SerializeField]
        float _biteMinimalWaitTime = 5f;
        [SerializeField]
        float _biteMaximalWaitTime = 30f;
        [SerializeField]
        float _biteTime = 1f;

        [Header("Lure Phase")]
        [SerializeField, Range(0f, 1f), Tooltip("Token position at start")]
        float _initPosition = 0.5f;
        [SerializeField]
        float _initAcceleration = 0.3f;
        [SerializeField, Tooltip("acceleration increase per second")]
        float _initAccelerationAcceleration = 0f;


        //[SerializeField]
        float _throwTime = 1.5f;

        public string fishKey { get; } = "food_fish_raw";

        public float startFishingDistance => _startFishingDistance;
        public float FishingMinDepth => _fishingMinDepth;
        public float biteMinimalWaitTime => _biteMinimalWaitTime;
        public float biteMaximalWaitTime => _biteMaximalWaitTime;
        public float biteTime => _biteTime;
        public float initPosition => _initPosition;
        public float initAcceleration => _initAcceleration;
        public float initAccelerationAcceleration => _initAccelerationAcceleration;
        public float throwTime => _throwTime;
        public LayerMask FishingLayerMask => _fishingLayerMask;
        public LayerMask FishingBottomMask => _fishingBottomMask;
        #endregion
        private bool _isOnFishingZone = false;

        public bool isOnFishingZone => _isOnFishingZone;
        public void SetIsOnFishingZone(bool isOnZone) => _isOnFishingZone = isOnZone;

        private FishingState _currentState = FishingState.none;
        public FishingState state => _currentState;

        public Vector3 fishingPoint { get; private set; }
        public FishingZone fishingZone { get; private set; }

        public string GivenItem { get; set; }
        public bool IsFishGiven => GivenItem == fishKey;

        #region Events
        public event Action OnThrowStart;
        public event Action OnThrowStarted;
        public event Action<Vector3> OnThrowLineShow;
        public event Action OnThrowAnimationEnd;
        public event Action<bool> OnStartFishing;
        public event Action<bool> OnBiting;
        public event Action OnTryHook;
        public event Action<bool> OnHookResult;
        public event Action<bool> OnPull;
        public event Action<float> OnTension;
        public event Action OnFishingFailEscape;
        public event Action OnFishingFailTorn;
        public event Action OnFisingSuccessful;
        public event Action OnEndOfFishing;

        public event Action OnStateChange;
        #endregion
        public void Throw() => OnThrowStart?.Invoke();
        public void ThrowInited(Vector3 initPoint)
        {
            Debug.DrawRay(initPoint, Vector3.forward,Color.red);
            fishingPoint = initPoint;
            OnThrowStarted?.Invoke();
        }
        public void ThrowShowLine(Vector3 from) => OnThrowLineShow?.Invoke(from);
        public void ThrowAnimationEnd() => OnThrowAnimationEnd?.Invoke();

        public void Fishing(bool isOk, Vector3 fishPos,FishingZone fishZone)
        {
            fishingZone = fishZone;
            if (isOk)
                fishingPoint = fishPos;
            SetState(isOk ? FishingState.biting : FishingState.none);
            OnStartFishing?.Invoke(isOk);
        }

        public void Biting(bool isBiting) => OnBiting?.Invoke(isBiting);

        public void TryHook() => OnTryHook?.Invoke();
        

        public void HookResult(bool isOK) {
            SetState(isOK ? FishingState.pulling : FishingState.none);
            OnHookResult?.Invoke(isOK);
        }

        public void Pull(bool isPull) => OnPull?.Invoke(isPull);

        public void SetTension(float t) => OnTension?.Invoke(t);

        public void FailEscape()
        {
            SetState(FishingState.none);
            OnFishingFailEscape?.Invoke();
        }

        public void FailTorn()
        {
            SetState(FishingState.none);
            OnFishingFailTorn?.Invoke();
        }

        public void FisingSuccessful()
        {
            SetState(FishingState.reciveFish);
            OnFisingSuccessful?.Invoke();
            if (fishingZone!=null)
                fishingZone.fishCount--;
        }

        public void FishingEnd()
        {
            SetState(FishingState.none);
            OnEndOfFishing?.Invoke();
        }

        void SetState(FishingState newState)
        {
            bool isChage = newState != _currentState;
            _currentState = newState;
            if (isChage)
                OnStateChange?.Invoke();
        }
    }
}