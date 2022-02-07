using ActivityLog.Data;
using Core.Storage;
using Game.Progressables;
using RoboRyanTron.SearchableEnum;
using System;
using UnityEngine;

namespace Game.Models
{
    public abstract class DungeonProgressModel :  MonoBehaviour, IProgressable, IActivityLogEnterenceProducer
    {
        [Serializable]
        public class Data : DataBase
        {
            [SerializeField] private long _nextProgressResetTime;
            [SerializeField] private int _controllPoint;
            [SerializeField] private bool _unlocked;
            [SerializeField] private int _passesCount;
            [SerializeField] private  bool _inProgress;

            public long NextProgressResetTime
            {
                get { return _nextProgressResetTime;}
                private set {_nextProgressResetTime = value; ChangeData();}
            }
            public int ControllPoint
            {
                get { return _controllPoint;}
                private set {_controllPoint = value; ChangeData();}
            }
            public bool Unlocked
            {
                get { return _unlocked;}
                private set {_unlocked = value; ChangeData();}
            }
            public int PassesCount
            {
                get { return _passesCount;}
                private set {_passesCount = value; ChangeData();}
            }
            public bool InProgress
            {
                get { return _inProgress;}
                private set {_inProgress = value; ChangeData();}
            }

            public void SetControllPoint(int controllPoint) => ControllPoint = controllPoint;
            public void SetUnlocked(bool unlocked) => Unlocked = unlocked;
            public void IncrementPassesCount() => PassesCount++;
            public void SetInProgress(bool value) => InProgress = value;
            public void SetNextProgressResetTime(long nextProgressResetTime) => NextProgressResetTime = nextProgressResetTime;
        }
        #region Data
#pragma warning disable 0649

        [SerializeField] protected Data _data;
        [SerializeField] protected float _progressResetTime;

        [SearchableEnum]
        [SerializeField] private LocalizationKeyID _activitiesLogMessage;
        [SerializeField] private Sprite _icon;

#pragma warning restore 0649
        #endregion

        public Data _Data => _data;

        #region Dependencies
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private LocalizationModel LocalizationModel => ModelsSystem.Instance._localizationModel;
        #endregion

        public virtual EnvironmentSceneID EnvironmentSceneID => EnvironmentSceneID.None;

        public float ProgressResetTime => _progressResetTime;
        public int ControllPoint 
        {
            get { return _data.ControllPoint; }
            protected set { _data.SetControllPoint(value); }
        }

        public bool Unlocked
        {
            get { return _data.Unlocked; }
            protected set { _data.SetUnlocked(value); }
        }

        public bool InProgress
        {
            get => _data.InProgress;
            private set => _data.SetInProgress(value);
        }
        public long NextProgressResetTime
        {
            get => _Data.NextProgressResetTime;
            set => _Data.SetNextProgressResetTime(value);
        }

        public event Action<DungeonProgressModel> OnPassedLocation;
        public event Action OnProgressResetted;
        public event Func<DungeonProgressModel, float> CalcRemainingTime;
        public event Action<int> OnChankEnter;
        public event Action OnUnlocked;

        // This interface not realy needed here because this model locaeted in core. IProgressable main purpose is for environment objects
        #region IProgressable
        public ProgressStatus ProgressStatus { get; set; }
        public void ClearProgress() { }
        #endregion

        #region IActivityLogEnterenceProducer

        public bool TryGetActivityLogEnterence(out ActivityLogEnterenceData activityLogEnterence)
        {
            activityLogEnterence = null;

            bool canShowEntry = ProgressStatus != ProgressStatus.WaitForResetProgress && _data.PassesCount >= 1;
            if(!canShowEntry) return false;

            activityLogEnterence = new ActivityLogEnterenceData(() => LocalizationModel.GetString(_activitiesLogMessage),_icon);
            return true;
        }

        #endregion


        #region MonoBehaviour
        private void Awake() 
        {
            StorageModel.TryProcessing(_data);
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ResetProgress();
            }
        }

        protected virtual void OnDisable()
        {
            StopAllCoroutines();
        }
        #endregion


        public void EnterLocation() 
        {
            InProgress = true;
            ProgressStatus = ProgressStatus.InProgress;
        }

        public void PassLocation()
        {
            _data.IncrementPassesCount();
            InProgress = false;
            ProgressStatus = ProgressStatus.WaitForResetProgress;
            OnPassedLocation?.Invoke(this);
        }

        public void ResetProgress()
        {
            NextProgressResetTime = 0;
            ProgressStatus = ProgressStatus.NotInProgress;
            OnProgressResetted?.Invoke();
        }

        public void ResetSavePoint()
        {
            ControllPoint = 0;
        }

        public float GetProgressRemainingTime() => CalcRemainingTime(this);

        public void UpdateChunkProgress(int chunkNumber) 
        {
            if (chunkNumber > ControllPoint) {
                ControllPoint = chunkNumber;
                OnChankEnter?.Invoke(ControllPoint);
            }
        }

        public void UnlocLocation()
        {
            Unlocked = true;
            OnUnlocked?.Invoke();
        }
    }
}
