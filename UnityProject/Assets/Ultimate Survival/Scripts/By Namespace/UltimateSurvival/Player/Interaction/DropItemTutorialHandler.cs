using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
    public class DropItemTutorialHandler : MonoBehaviour
    {
        [SerializeField]
        DroperItemsOnDeath _target;

        private void Start()
        {
            _target.onSpawnLoot += ClearTimer;
        }

        void ClearTimer(GameObject obj)
        {
            Debug.Log("Spawned Loot "+ obj.name);
            var comp = obj.GetComponent<DestroyItemTimeDelay>();
            if (comp != null)
                comp.RemoveComponentAndSaveIt();
        }
    }
}