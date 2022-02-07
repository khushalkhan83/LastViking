using System.Collections.Generic;
using Game.Models;
using Game.ObjectPooling;
using UnityEngine;

namespace Game.Components.AI
{
    [RequireComponent(typeof(EnemyHealthModel))]
    public class DeathVisuals : MonoBehaviour, IResettable
    {
        private EnemyHealthModel _health;
        private EnemyHealthModel health
        {
            get
            {
                if(_health == null)
                    _health = GetComponent<EnemyHealthModel>();

                return _health;
            }
        }

        #region Data
        #pragma warning disable 0649
        [SerializeField] private List<GameObject> deactivateOnDeathObjects;
    #pragma warning restore 0649
        #endregion
        

        private void Awake()
        {
            Subscribe();
            ShowVisuals(true);
        }

        #region IResetable
        public void ResetObject()
        {
            Subscribe();
            ShowVisuals(true);
        }
            
        #endregion

        private void Subscribe()
        {
            health.OnDeath -= DeathHandler;
            health.OnDeath += DeathHandler;
        }

        private void UnSubscribe()
        {
            health.OnDeath -= DeathHandler;
        }

        private void DeathHandler()
        {
            UnSubscribe();
            ShowVisuals(false);
        }

        private void ShowVisuals(bool show)
        {
            foreach (var obj in deactivateOnDeathObjects)
            {
                obj.SetActive(show);
            }
        }
    }
}
