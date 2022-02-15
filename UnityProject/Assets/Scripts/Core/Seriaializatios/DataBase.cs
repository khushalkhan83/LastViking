using System;
using UnityEngine;

namespace Core.Storage
{
    [Serializable]
    public abstract class DataBase : IUnique
    {
        #region Data
#pragma warning disable 0649

        [UUID]
        [SerializeField] private string _uuid;

        private uint _uuidPrefix;

#pragma warning restore 0649
        #endregion

        [Flags]
        public enum SaveTime
        {
            None = 0,
            Instantly = 1 << 0,
            Deffered = 1 << 1,
            ChangeContext = 1 << 2,
        }

        public string UUID
        {
            get => _uuid;
            set => _uuid = value;
        }

        public uint UUIDPrefix
        {
            get => _uuidPrefix;
            set => _uuidPrefix = value;
        }

        public virtual SaveTime TimeSave { get; } = SaveTime.Deffered;

        public event Action<IUnique, SaveTime> OnDataChanged;
        public void ChangeData() => OnDataChanged?.Invoke(this, TimeSave);

        public void WriteData(Action action)
        {
            action?.Invoke();
            ChangeData();
        }
    }
}
