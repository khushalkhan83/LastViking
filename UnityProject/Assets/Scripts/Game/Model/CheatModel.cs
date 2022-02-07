using System;
using Core.Storage;
using UnityEngine;

namespace Game.Models
{
    public class CheatModel : InitableModel<CheatModel.Data>
    {
        [Serializable]
        public class Data: DataBase
        {
            [SerializeField] private bool isCheater;
            [SerializeField] private bool eventSent;

            public bool IsCheater
            {
                get { return isCheater; }
                set { isCheater = value; ChangeData(); }
            }

            public bool IsCheatEventSent
            {
                get { return eventSent; }
                set { eventSent = value; ChangeData(); }
            }
        }
        #region Data
        #pragma warning disable 0649
        [SerializeField] private Data _data;
        [SerializeField] private int _coinsLimit;
#pragma warning restore 0649
        #endregion

        public event Action OnCheatDetected;

        protected override Data DataBase => _data;

        public void MarkPlayerAsCheater()
        {
            _data.IsCheater = true;
            OnCheatDetected?.Invoke();
        }

        public void SetEventSent() => _data.IsCheatEventSent = true;

        public bool PlayerIsCheater => _data.IsCheater;

        public bool IsCheatEventSent => _data.IsCheatEventSent;

        public int CoinsLimit => _coinsLimit;
    }
}