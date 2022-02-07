using NaughtyAttributes;
using UnityEngine;

namespace Game.Components
{
    public class TriggerZoneLoader : MonoBehaviour
    {
        [SerializeField] private GameObject targetGameObject;
        [ReadOnly] [SerializeField] private bool isLoaded;
        private bool shouldLoad;

        private const string PlayerTag = "Target";
        private const string PlayerName = "ThirdPersonPlayer";

        private void Update()
        {
            TriggerCheck();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsPlayer(other))
            {
                shouldLoad = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsPlayer(other))
            {
                shouldLoad = false;
            }
        }

        private void TriggerCheck()
        {
            if (shouldLoad)
            {
                Load();
            }
            else
            {
                UnLoad();
            }
        }

        private void Load()
        {
            targetGameObject.SetActive(true);
        }

        private void UnLoad()
        {
            targetGameObject.SetActive(false);
        }

        private bool IsPlayer(Collider other)
        {
            Debug.Log("Is Player check: " + other.gameObject.name);
            return other.tag == PlayerTag && other.gameObject.name == PlayerName;
        }
    }
}

