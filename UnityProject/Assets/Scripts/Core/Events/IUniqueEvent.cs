using System;

namespace Core
{
    public interface IUniqueEvent
    {
        void AddListener(object key, Action action);
        void RemoveListener(object key, Action action);
    }

    public interface IUniqueEvent<T>
    {
        void AddListener(T key, Action action);
        void RemoveListener(T key, Action action);
    }

    public interface IUniqueEvent<T, Arg1>
    {
        void AddListener(T key, Action<Arg1> action);
        void RemoveListener(T key, Action<Arg1> action);
    }
}
