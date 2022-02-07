using UnityEngine;

namespace Core.Providers
{
    public abstract class Provider<Key, Value> : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private Value[] _data;

#pragma warning restore 0649
        #endregion

        public Value this[Key key] => _data[(int)(object)key - 1];
    }
}
