using Core.Storage;
using Game.Objectives;
using System;
using UnityEngine;

namespace Game.Models
{
    public class PlayerEventObjectivesModel : MonoBehaviour
    {
        [Serializable]
        public class EventObjectivesData : DataBase
        {
            public bool IsHasObjectiveId;
            public ushort ObjectiveId;

            public void SetIsHasObjectiveId(bool isHasObjectiveId)
            {
                IsHasObjectiveId = isHasObjectiveId;
                ChangeData();
            }

            public void SetObjectiveId(ushort objectiveId)
            {
                ObjectiveId = objectiveId;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private EventObjectivesData _data;
        [SerializeField] StorageModel _storageModel;

#pragma warning restore 0649
        #endregion

        public EventObjectivesData Data => _data;
        public StorageModel StorageModel => _storageModel;

        public ObjectiveID CurrentObjectiveID { get; private set; }

        public ushort ObjectiveId
        {
            get => _data.ObjectiveId;
            private set => _data.SetObjectiveId(value);
        }

        public bool IsHasObjectiveId
        {
            get => _data.IsHasObjectiveId;
            private set => _data.SetIsHasObjectiveId(value);
        }

        public event Action OnStartObjective;
        public event Action OnEndObjective;

        public bool IsObjectiveNow => IsHasObjectiveId;

        public void OnEnable()
        {
            StorageModel.TryProcessing(_data);
        }

        public void SetObjectiveId(ushort id) => ObjectiveId = id;
        public void SetCurrentObjectiveID(ObjectiveID objectiveID) => CurrentObjectiveID = objectiveID;

        public void Save(ushort id)
        {
            IsHasObjectiveId = true;
            ObjectiveId = id;
        }

        public void StartObjective(ObjectiveID objectiveID)
        {
            // FIXME: fix issue when old objective removed
            // if (IsHasObjectiveId) return;

            CurrentObjectiveID = objectiveID;
            OnStartObjective?.Invoke();
        }

        public void EndObjective()
        {
            OnEndObjective?.Invoke();
            IsHasObjectiveId = false;
        }

        public void ResetHasObjective() => IsHasObjectiveId = false;
    }
}
