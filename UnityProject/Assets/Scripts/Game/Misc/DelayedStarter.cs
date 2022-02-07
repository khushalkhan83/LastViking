using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Misc
{
    public class DelayedStarter : MonoBehaviour
    {
        [SerializeField] private float delay = default;
        [SerializeField] private UnityEvent unityEvent = default;
        private void OnEnable()
        {
            StartCoroutine(RiseEvent());
        }
        private IEnumerator RiseEvent()
        {
            yield return new WaitForSeconds(delay);
            unityEvent?.Invoke();
        }
    } 
}
