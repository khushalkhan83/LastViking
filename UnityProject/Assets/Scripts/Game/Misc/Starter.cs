using UnityEngine;
using UnityEngine.Events;

namespace Game.Misc
{
    public class Starter : MonoBehaviour
    {
        [SerializeField] private UnityEvent onEnableEvent = default;
        private void OnEnable()
        {
            onEnableEvent?.Invoke();
        }
    }
}

