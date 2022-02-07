using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Storage;
using System;
using Game.Views;

namespace Game.Models
{
    public enum TreasureHuntState { wait,reciveBottle,waitActivate,digging}
    public class TreasureHuntModel : MonoBehaviour
    {
        [System.Serializable]
        public class TreasureHuntSave : DataBase, IImmortal
        {
            [SerializeField]
            TreasureHuntState _currentState = TreasureHuntState.wait;
            public TreasureHuntState currentState => _currentState;

            public void SetState(TreasureHuntState newState)
            {
                if (_currentState != newState)
                {
                    _currentState = newState;
                    ChangeData();
                }
            }

            [SerializeField]
            long _launchDate = -1;

            public bool isInited => _launchDate != -1;
            public DateTime launchDate
            {
                get
                {
                    return DateTime.FromBinary(_launchDate);
                }
                set
                {
                    _launchDate = value.ToBinary();
                    ChangeData();
                }
            }

            [SerializeField]
            long _diggingEndTimeout = -1;
            public DateTime diggingEndTimeout
            {
                get
                {
                    return DateTime.FromBinary(_diggingEndTimeout);
                }
                set
                {
                    _diggingEndTimeout = value.ToBinary();
                    ChangeData();
                }
            }

            [SerializeField]
            int _placeIndex = -1, _holeIndex = -1;
            public int placeIndex => _placeIndex;
            public int holeIndex => _holeIndex;

            public void SetNewDigPlace(int p,int h)
            {
                _placeIndex = p;
                _holeIndex = h;
                ChangeData();
            }

            [SerializeField]
            List<int> _usedHoles;

            public void ClearUsedHoles()
            {
                if (_usedHoles != null)
                    _usedHoles.Clear();
            }
            public bool IsHoleUsed(int indx)
            {
                if (_usedHoles == null)
                    return false;

                if (indx < 0)
                    return false;

                return _usedHoles.Contains(indx);
            }

            public void UseHole(int indx)
            {
                if (_usedHoles == null)
                    _usedHoles = new List<int>();
                if (!_usedHoles.Contains(indx))
                {
                    _usedHoles.Add(indx);
                    ChangeData();
                }
            }
        }

        public int currentPlace => data.placeIndex;
        public int currentHole => data.holeIndex;

        private bool DebugOptionsAvaliable => EditorGameSettings.Instance.debugControllersSettings;

        public const string kLootBottle = "bottle_with_treasure";

        [SerializeField]
        bool _isDebug = false;
        [SerializeField]
        int firstWaitForEventInMinutes = 60;
        [SerializeField]
        int nextRestartTreasureHuntInMinutes = 1200; // 20h
        [SerializeField]
        int diggingTimeoutInMinutes = 480; // 8h

        public bool IsDebug
        {
            get => _isDebug;
            set{
                if(!DebugOptionsAvaliable) return;
                _isDebug = value;
                OnDebugOptionChanged?.Invoke();
            }
        }

        [SerializeField]
        TreasureHuntSave data;

        private void OnEnable()
        {
            ModelsSystem.Instance._storageModel.TryProcessing(data);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public DateTime endOfDiggingDate => data.diggingEndTimeout;

        public TreasureHuntState state => data.currentState;
        
        public event Action OnTimeForCheckWait;
        public event Action OnDiggingTimeOut;
        public event Action OnBeginRecive;

        public void StartReciveBottleMode()
        {
            data.SetState(TreasureHuntState.reciveBottle);
            OnBeginRecive?.Invoke();
        }

        public event Action OnBeginDigMode;
        public event Action OnWaitActivate;
        public event Action OnDebugOptionChanged;

        public void StartWaitActivateMode()
        {
            data.SetState(TreasureHuntState.waitActivate);
            OnWaitActivate?.Invoke();
        }

        public void StartDigMode()
        {
            data.SetState(TreasureHuntState.digging);
            data.ClearUsedHoles();
            OnBeginDigMode?.Invoke();
        }

        public event System.Action<int> OnShowDigPlace;
        public event System.Action OnHideDigPlace;

        public event Action<bool,bool> OnEnterNearZone;
        public void EntedNearZone(bool isNear,bool withShovel) => OnEnterNearZone?.Invoke(isNear,withShovel);

        public void SetDiggingPlaces(int placeIndex,int holeIndex)
        {
            data.SetNewDigPlace(placeIndex, holeIndex);
            OnShowDigPlace?.Invoke(data.placeIndex);
        }

        public void SetDiggingTimeout(DateTime now)
        {
            data.diggingEndTimeout = now.AddMinutes(diggingTimeoutInMinutes);
            CreateWaitAlarm(diggingTimeoutInMinutes * 60);
        }

        public DateTime GetEndOfWaitDate(DateTime nowDate)
        {
            if (data.isInited)
            {
                return data.launchDate;
            }
            else
            {
                data.launchDate = nowDate.AddMinutes(firstWaitForEventInMinutes);
                return data.launchDate;
            }
        }

        public bool IsHoleUsed(int indx) => data.IsHoleUsed(indx);

        Coroutine _waitCoroutine;

        public void ResetAlarm()
        {
            if (_waitCoroutine != null)
                StopCoroutine(_waitCoroutine);
        }

        public void CreateWaitAlarm(float seconds)
        {
            if (_waitCoroutine != null)
                StopCoroutine(_waitCoroutine);
            _waitCoroutine = StartCoroutine(AlarmTimer(seconds));
        }

        IEnumerator AlarmTimer(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds + 0.1f);
            _waitCoroutine = null;
            OnTimeForCheckWait?.Invoke();
        }

        public void DiggingTimeOut(DateTime now)
        {
            data.launchDate = now.AddMinutes(nextRestartTreasureHuntInMinutes);
            data.SetState(TreasureHuntState.wait);

            OnDiggingTimeOut?.Invoke();
        }
        
        public event Action<int> OnTryDigPlace;
        public void TryDigPlace(int indx) => OnTryDigPlace?.Invoke(indx);

        public void SetPlaceDigged(int indx)
        {
            data.UseHole(indx);
        }

        public event Action OnGetTreasure;
        public void ReciveTreasure(DateTime now) => OnGetTreasure?.Invoke();

        public event Action OnInternetError;
        public void InternetErrorMessage()
        {
            OnInternetError?.Invoke();
            InternetErrorView.InternetErrorMessage();
        }

        public event Action<int> ChestOpen;
        public void OnChestOpen(int emptyCellsCount) {
            ChestOpen?.Invoke(emptyCellsCount);
        }

        public event Action StartDigging;
        public void OnStartDigging() {

            StartDigging?.Invoke();
        }

        public bool IsActive { get; private set; }
        public event Action OnActiveChanged;

        public void SetActive(bool on)
        {
            IsActive = on;
            OnActiveChanged?.Invoke();
        }
    }
}