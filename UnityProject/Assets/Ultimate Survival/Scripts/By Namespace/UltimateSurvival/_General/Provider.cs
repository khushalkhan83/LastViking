using System.Collections.Generic;
using UnityEngine;

public abstract class Provider<Key, Value> : MonoBehaviour
{
    #region Data
#pragma warning disable 0649

    [SerializeField] public Value[] _data;

#pragma warning restore 0649
    #endregion

    public Value this[Key key]
    {
        get
        {
#if UNITY_EDITOR
            if (((int)(object)key - 1) >= _data.Length)
            {
                $"key|{key}| provider name|{name}|".Error("Key not found");
            }
#endif
            return _data[(int)(object)key - 1];
        }
    }

    public IEnumerator<Value> GetEnumerator()
    {
        for (int i = 0; i < _data.Length; i++)
        {
            yield return _data[i];
        }
    }

    public int Count => _data.Length;
}
