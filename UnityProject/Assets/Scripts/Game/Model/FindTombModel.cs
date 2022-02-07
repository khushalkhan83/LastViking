using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class FindTombModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public bool HasStarted;
            public bool HasEnded;

            public void SetHasEnded(bool ended)
            {
                HasEnded = ended;
                ChangeData();
            }

            public void SetHasStarted(bool started)
            {
                HasStarted = started;
                ChangeData();
            }
        }

        [SerializeField] private Data _data;

        public Data _Data => _data;

        public event Action OnEventStarted;
        public event Action OnEventEnded;

        public bool HasStarted
        {
            set => _Data.SetHasStarted(value);
            get => _Data.HasStarted;
        }

        public bool HasEnded
        {
            set => _Data.SetHasEnded(value);
            get => _Data.HasEnded;
        }

        public bool IsEventInProgress => HasStarted && !HasEnded;

        public void StartEvent()
        {
            HasStarted = true;
            OnEventStarted?.Invoke();
        }

        public void EndEvent()
        {
            HasEnded = true;
            OnEventEnded?.Invoke();
        }
    }
}
