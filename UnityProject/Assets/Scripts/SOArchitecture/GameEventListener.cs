using UnityEngine;
using UnityEngine.Events;

namespace SOArchitecture
{
    public class GameEventListener : MonoBehaviour
    {
        [SerializeField] private GameEvent Event;
        [SerializeField] private UnityEvent Response;

        private void OnEnable() => Register();

        private void OnDisable() => UnRegister();

        private void Register()
        { Event?.RegisterListener(this); }

        private void UnRegister()
        { Event?.UnregisterListener(this); }

        public void OnEventRaised()
        { Response.Invoke(); }
    }
}