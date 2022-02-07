using System;

namespace UltimateSurvival
{
    public delegate bool TryerDelegate();

    public class Attempt
    {
        private TryerDelegate m_Tryer;
        private Action m_Listeners;

        /// <summary>
        /// Registers a method that will try to execute this action.
        /// NOTE: Only 1 tryer is allowed!
        /// </summary>
        public void SetTryer(TryerDelegate tryer) => m_Tryer = tryer;

        public void AddListener(Action listener) => m_Listeners += listener;

        public void RemoveListener(Action listener) => m_Listeners -= listener;

        public bool Try()
        {
            if (m_Tryer?.Invoke() ?? true)
            {
                m_Listeners?.Invoke();
                return true;
            }

            return false;
        }
    }

    public class Attempt<T>
    {
        public delegate bool GenericTryerDelegate(T arg);

        GenericTryerDelegate m_Tryer;
        Action<T> m_Listeners;

        /// <summary>
        /// Registers a method that will try to execute this action.
        /// NOTE: Only 1 tryer is allowed!
        /// </summary>
        public void SetTryer(GenericTryerDelegate tryer) => m_Tryer = tryer;

        public void AddListener(Action<T> listener) => m_Listeners += listener;

        public void RemoveListener(Action<T> listener) => m_Listeners -= listener;

        public bool Try(T arg)
        {
            if (m_Tryer?.Invoke(arg) ?? true)
            {
                m_Listeners?.Invoke(arg);
                return true;
            }

            return false;
        }
    }

    public class Attempt<T, V>
    {
        public delegate bool GenericTryerDelegate(T arg1, V arg2);

        private GenericTryerDelegate m_Tryer;
        private Action<T, V> m_Listeners;

        /// <summary>
        /// Registers a method that will try to execute this action.
        /// NOTE: Only 1 tryer is allowed!
        /// </summary>
        public void SetTryer(GenericTryerDelegate tryer) => m_Tryer = tryer;

        public void AddListener(Action<T, V> listener) => m_Listeners += listener;

        public void RemoveListener(Action<T, V> listener) => m_Listeners -= listener;

        public bool Try(T arg1, V arg2)
        {
            if (m_Tryer?.Invoke(arg1, arg2) ?? true)
            {
                m_Listeners?.Invoke(arg1, arg2);
                return true;
            }

            return false;
        }
    }
}