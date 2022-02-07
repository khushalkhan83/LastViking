using System.Collections;
using UnityEngine;

namespace Game.Models
{
    public delegate void Message<T>(T message) where T : class;

    public class ActionsLogModel : MonoBehaviour
    {
        public Hashtable map = new Hashtable();

        public void AddListener<T>(Message<T> messageListener) where T : class
        {
            var type = typeof(T);
            if (map.ContainsKey(type))
            {
                var @event = (Message<T>)map[type];
                @event += messageListener;
                map[type] = @event;
            }
            else
            {
                map[type] = messageListener;
            }
        }

        public void RemoveListener<T>(Message<T> messageListener) where T : class
        {
            var type = typeof(T);
            if (map.ContainsKey(type))
            {
                var @event = (Message<T>)map[type];
                @event -= messageListener;
                map[type] = @event;
            }
        }

        public void SendMessage<T>(T message) where T : class
        {
            var type = typeof(T);
            if (map.Contains(type))
            {
                var @event = (Message<T>)map[type];
                @event?.Invoke(message);
            }
        }
    }
}
