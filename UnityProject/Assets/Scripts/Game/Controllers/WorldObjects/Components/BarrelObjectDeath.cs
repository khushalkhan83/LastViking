using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class BarrelObjectDeath : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

#pragma warning restore 0649
        #endregion

        private IHealth _health;
        public IHealth Health => _health ?? (_health = GetComponent<IHealth>());

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private StatisticsModel StatisticsModel => ModelsSystem.Instance._statisticsModel;

        private void OnEnable()
        {
            Health.OnDeath += OnDeathHandler;
        }

        private void OnDisable()
        {
            Health.OnDeath -= OnDeathHandler;
        }

        private void OnDeathHandler()
        {
            StatisticsModel.BarrelDestroy();
        }
    }
}
