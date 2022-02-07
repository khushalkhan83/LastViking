using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class PlayerRunModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredBool IsRun;
            public ObscuredBool IsRunToggle;

            public void SetIsRun(bool isRun)
            {
                IsRun = isRun;
                ChangeData();
            }

            public void SetIsRunToggle(bool isRunToggle)
            {
                IsRunToggle = isRunToggle;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;

#pragma warning restore 0649
        #endregion

        public bool IsRun
        {
            get
            {
                return _data.IsRun;
            }
            private set
            {
                _data.SetIsRun(value);
            }
        }

        public bool IsRunToggle
        {
            get
            {
                return _data.IsRunToggle;
            }
            private set
            {
                _data.SetIsRunToggle(value);
            }
        }

        public event Action OnChangeIsRun;
        public event Action OnChangeIsRunToggle;

        public void RunStart()
        {
            var last = IsRun;
            IsRun = true;

            if (IsRun != last)
            {
                OnChangeIsRun?.Invoke();
            }
        }

        public void RunStop()
        {
            var last = IsRun;
            IsRun = false;

            if (IsRun != last)
            {
                OnChangeIsRun?.Invoke();
            }
        }

        public void RunToggleActive()
        {
            IsRunToggle = true;

            OnChangeIsRunToggle?.Invoke();
        }

        public void RunTogglePassive()
        {
            IsRunToggle = false;

            OnChangeIsRunToggle?.Invoke();
        }

    }
}
