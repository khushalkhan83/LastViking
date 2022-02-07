using System.Collections;
using UnityEngine;

namespace Core.Mapper
{
    public abstract class MapperBase<Key, Value> : MonoBehaviour, IMap<Key, Value>
    {
        private Hashtable _map = new Hashtable();

        public Value this[Key key] => (Value)_map[key];

        protected void Map(Key key, Value value)
        {
            _map[key] = value;
        }
    }
}
