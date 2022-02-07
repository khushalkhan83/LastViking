using System;
using UnityEngine;

namespace UltimateSurvival
{

    [Serializable]
    public class ChangedValue<T>
    {
        public event Action OnChange;

        #region Data
#pragma warning disable 0649

        [SerializeField] private T _value;
        [SerializeField] private T _valueLast;

#pragma warning restore 0649
        #endregion

        public T Value => _value;
        public T LastValue => _valueLast;

        public ChangedValue(T value)
        {
            SetValue(value);
        }

        public bool Is(T value)
        {
            return _value != null && _value.Equals(value);
        }

        public void RemoveChangeListener(Action callback)
        {
            OnChange -= callback;
        }

        public void Set(T value)
        {
            SetValue(value);

            if (!Is(_valueLast))
            {
                OnChange?.Invoke();
            }
        }

        public void SetForce(T value)
        {
            SetValue(value);
            OnChange?.Invoke();
        }

        public void SetSilence(T value) => SetValue(value);

        private void SetValue(T value)
        {
            _valueLast = _value;
            _value = value;
        }
    }
}