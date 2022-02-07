using UnityEngine;
using Game.Models;
using UnityEngine.Events;

namespace Game.VillageBuilding
{
    public class BuildingDamageListener : MonoBehaviour
    {
        private BuildingHealthModel[] models;

        [SerializeField] private UnityEvent onDamagedEffect;

        #region MonoBehaviour
        private void OnEnable()
        {
            models = GetComponentsInParent<BuildingHealthModel>(true);

            foreach (var model in models)
            {
                model.OnChangeHealth += DamageHandler;
            }
        }
        private void OnDisable()
        {
            foreach (var model in models)
            {
                model.OnChangeHealth -= DamageHandler;
            }
        }
        #endregion

        private void DamageHandler()
        {
            onDamagedEffect?.Invoke();
        }
    }
}