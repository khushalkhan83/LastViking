using System;
using Core.Storage;
using UnityEngine;

namespace Game.Models
{
    public class WipeSaveModel : InitableModel<WipeSaveModel.Data>
    {
        [Serializable]
        public class Data: DataBase
        {
            [SerializeField] private bool tryPerformAction = true;

            public bool TryPerformAction
            {
                get { return tryPerformAction; }
                set { tryPerformAction = value; ChangeData(); }
            }
        }
        #region Data
        #pragma warning disable 0649
        [SerializeField] private Data _data;
        #pragma warning restore 0649
        #endregion

        protected override Data DataBase => _data;

        public bool TryPerformAction
        {
            get => _data.TryPerformAction;
            set => _data.TryPerformAction = value;
        }
    }
}
