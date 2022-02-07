using System.Collections;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class WorldObjectDeath : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private WorldObjectModel _worldObjectModel;

#pragma warning restore 0649
        #endregion

        private WorldObjectModel WorldObjectModel => _worldObjectModel;

        private IHealth _health;
        private IHealth Health => _health ?? (_health = GetComponent<IHealth>());

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

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
            WorldObjectModel.Delete();
            Destroy(gameObject);
        }
    }
}
