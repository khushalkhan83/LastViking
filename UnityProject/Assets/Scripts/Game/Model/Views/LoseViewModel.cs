using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    
    public class LoseViewModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredVector3 PivotPosition;
            public ObscuredVector3 PivotForward;
            public ObscuredVector3 PivotUp;
            public ObscuredVector3 PivotRigth;

            public void SetPivotPosition(Vector3 pivotPosition)
            {
                PivotPosition = pivotPosition;
                ChangeData();
            }

            public void SetPivotForward(Vector3 pivotForward)
            {
                PivotForward = pivotForward;
                ChangeData();
            }

            public void SetPivotUp(Vector3 pivotUp)
            {
                PivotUp = pivotUp;
                ChangeData();
            }

            public void SetPivotRight(Vector3 pivotRight)
            {
                PivotRigth = pivotRight;
                ChangeData();
            }
        }

        [Serializable]
        public class ResurrectData : DataBase
        {
            public ObscuredInt ResurrectWatchCount;
            public ObscuredBool ShouldCreateTomb;

            public void SetResurrectWatchCount(int resurrectWatchCount)
            {
                ResurrectWatchCount = resurrectWatchCount;
                ChangeData();
            }

            public void SetShouldCreateTomb(bool shouldCreate)
            {
                ShouldCreateTomb = shouldCreate;
                ChangeData();
            }
        }

        [Serializable]
        public class TutorialResurrectData : DataBase
        {
            public ObscuredBool HasResurrected;

            public void SetHasResurrected(bool hasResurrected)
            {
                HasResurrected = hasResurrected;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private ResurrectData _resurrectData;
        [SerializeField] private TutorialResurrectData _tutorialResurrectData;

        [SerializeField] private ObscuredFloat _timeFadeInBlood;
        [SerializeField] private ObscuredFloat _timeFadeInBlack;
        [SerializeField] private ObscuredFloat _timeFadeOutBlack;

        [SerializeField] private ObscuredFloat _resurrectTimeWait;
        [SerializeField] private ObscuredFloat _showPreDeathViewDelay;
        [SerializeField] private ObscuredInt _watchCountToResurrectGold;

        [SerializeField] public List<string> _bonusItems;

        [SerializeField] private ObscuredVector3 _cameraTombOffset;
        [SerializeField] private ObscuredVector3 _cameraPivotOffset;
        [SerializeField] private StorageModel _storageModel;

#pragma warning restore 0649
        #endregion

        public StorageModel StorageModel => _storageModel;

        public float TimeFadeInBlood => _timeFadeInBlood;
        public float TimeFadeInBlack => _timeFadeInBlack;
        public float TimeFadeOutBlack => _timeFadeOutBlack;

        public float ResurrectTimeWait => _resurrectTimeWait;
        public float ShowPreDeathViewDelay => _showPreDeathViewDelay;
        public int WatchCountToResurrectGold => _watchCountToResurrectGold;

        public Vector3 CameraTombOffset => _cameraTombOffset;
        public Vector3 CameraPivotOffset => _cameraPivotOffset;

        public Vector3 PivotPosition
        {
            get => _data.PivotPosition;
            protected set => _data.SetPivotPosition(value);
        }

        public Vector3 PivotForward
        {
            get => _data.PivotForward;
            protected set => _data.SetPivotForward(value);
        }

        public Vector3 PivotUp
        {
            get => _data.PivotUp;
            protected set => _data.SetPivotUp(value);
        }

        public Vector3 PivotRigth
        {
            get => _data.PivotRigth;
            protected set => _data.SetPivotRight(value);
        }

        public int ResurrectWatchCount
        {
            get => _resurrectData.ResurrectWatchCount;
            protected set => _resurrectData.SetResurrectWatchCount(value);
        }

        public bool HasTutorialResurrected {
            get => _tutorialResurrectData.HasResurrected;
            protected set => _tutorialResurrectData.SetHasResurrected(value);
        }
        public bool ShouldCreateTomb
        {
            get => _resurrectData.ShouldCreateTomb;
            protected set => _resurrectData.SetShouldCreateTomb(value);
        }

        public int PlayerRank { get; set; }
        public int OldScore { get; set; }
        public int OldRank { get; set; }
        public int OldIndex { get; set; }
        public float Speed { get; set; }
        public float Delay { get; set; }

        public event Action OnPlayAgain;

        public event Action OnEndResurrectTime;

        #region MonoBehaviour
        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
            StorageModel.TryProcessing(_resurrectData);
            StorageModel.TryProcessing(_tutorialResurrectData);
        }
        #endregion

        public void PlayAgain() => OnPlayAgain?.Invoke();
        public void EndResurrectTime() => OnEndResurrectTime?.Invoke();

        public void SetPivot(Transform pivot)
        {
            PivotPosition = pivot.position;
            PivotForward = pivot.forward;
            PivotUp = pivot.up;
            PivotRigth = pivot.right;
        }

        public void UpdateResurrectWatchCount() => ResurrectWatchCount++;

        public void TutorialResurrect() => HasTutorialResurrected = true;

        public event Action OnRessurect;
        public void RessurectAction() => OnRessurect?.Invoke();

        public void SetShouldCreateTomb() => ShouldCreateTomb = true;
        public void ResetShouldCreateTomb() => ShouldCreateTomb = false;
    }
}
