using System;
using System.Collections;

namespace Core
{
    public class UniqueAction<T, Arg1> : IUniqueEvent<T, Arg1>
    {
        private Hashtable listenets = new Hashtable();

        private Action<Arg1> __action;

        public void AddListener(T key, Action<Arg1> action)
        {
            if (listenets.ContainsKey(key))
            {
                __action = (Action<Arg1>)listenets[key];
                __action += action;
                listenets[key] = __action;
            }
            else
            {
                listenets[key] = action;
            }
        }

        public void RemoveListener(T key, Action<Arg1> action)
        {
            if (listenets.ContainsKey(key))
            {
                __action = (Action<Arg1>)listenets[key];
                __action -= action;
                listenets[key] = __action;
            }
        }

        public void RemoveAllListeners()
        {
            listenets.Clear();
        }

        public void RemoveAllListeners(T key)
        {
            listenets.Remove(key);
        }

        public void Invoke(T key, Arg1 arg1)
        {
            if (listenets.ContainsKey(key))
            {
                ((Action<Arg1>)listenets[key])?.Invoke(arg1);
            }
        }
    }
    public class UniqueAction<T> : IUniqueEvent<T>
    {
        private Hashtable listenets = new Hashtable();

        private Action __action;

        public void AddListener(T key, Action action)
        {
            if (listenets.ContainsKey(key))
            {
                __action = (Action)listenets[key];
                __action += action;
                listenets[key] = __action;
            }
            else
            {
                listenets[key] = action;
            }
        }

        public void RemoveListener(T key, Action action)
        {
            if (listenets.ContainsKey(key))
            {
                __action = (Action)listenets[key];
                __action -= action;
                listenets[key] = __action;
            }
        }

        public void RemoveAllListeners()
        {
            listenets.Clear();
        }

        public void RemoveAllListeners(T key)
        {
            listenets.Remove(key);
        }

        public void Invoke(T key)
        {
            if (listenets.ContainsKey(key))
            {
                ((Action)listenets[key])?.Invoke();
            }
        }
    }

    public class UniqueAction : IUniqueEvent
    {
        private Hashtable listenets = new Hashtable();

        private Action __action;

        public void AddListener(object key, Action action)
        {
            if (listenets.ContainsKey(key))
            {
                __action = (Action)listenets[key];
                __action += action;
                listenets[key] = __action;
            }
            else
            {
                listenets[key] = action;
            }
        }

        public void RemoveListener(object key, Action action)
        {
            if (listenets.ContainsKey(key))
            {
                __action = (Action)listenets[key];
                __action -= action;
                listenets[key] = __action;
            }
        }

        public void RemoveAllListeners()
        {
            listenets.Clear();
        }

        public void RemoveAllListeners(object key)
        {
            listenets.Remove(key);
        }

        public void Invoke(object key)
        {
            if (listenets.ContainsKey(key))
            {
                ((Action)listenets[key])?.Invoke();
            }
        }
    }
}
