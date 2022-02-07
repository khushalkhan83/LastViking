using System;
using System.Collections;
using Game.Models;
using MarchingBytes;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class DeathController : MonoBehaviour
    {
        private CoroutineModel CoroutineModel {get => ModelsSystem.Instance._coroutineModel;}

        public event Action<DeathController> OnDeathAction;

        public EnemyID EnemyID { get; private set; }
        private IHealth _health;
        private LootObject _lootObject;
        private int _coroutineIndex;
        private const float _krabDeathAnimationDuration = 4f;
        private const string k_enemyWithoutDeathAnimation = "skeleton";

        public void Init(IHealth health, EnemyID enemyID)
        {
            UnSubscibe();
            this._health = health;
            EnemyID = enemyID;
            Subscribe();
        }

        private void Subscribe()
        {
            if(_health == null) return;

            _health.OnDeath += DeathHandler;
        }

        private void UnSubscibe()
        {
            if(_health == null) return;

            _health.OnDeath -= DeathHandler;
            _health = null;
            CoroutineModel.BreakeCoroutine(_coroutineIndex);
        }

        private void DeathHandler()
        {
            OnDeathAction?.Invoke(this);

            if(HasDeathAnimation())
            {
                _lootObject = GetComponentInChildren<LootObject>();
                EnableLoot(false);

                _coroutineIndex = CoroutineModel.InitCoroutine(DoAfterSeconds(_krabDeathAnimationDuration,() => ReadyForDespawn()));
            }
            else
            {
                // wait one frame because of position must be the same (coins + particles)
                _coroutineIndex = CoroutineModel.InitCoroutine(DoAfterFrame(() => ReadyForDespawn()));
            }
        }

        private void ReadyForDespawn()
        {
            if(gameObject == null)
            {
                Debug.LogException(new System.NullReferenceException("Death controller. GameObject is null"));
                return;
            }
            if(EasyObjectPool.instance == null)
            {
                Debug.LogException(new System.NullReferenceException("Objects pull reference is null"));

                if(gameObject != null)
                    Destroy(gameObject);
            }
            else
            {
                EasyObjectPool.instance.ReturnObjectToPool(gameObject);
                RestViewPosition();
                EnableLoot(true);
            }
        }

        // TODO: Code duplicate here and in other classes. Move to CoroutineModel
        private IEnumerator DoAfterSeconds(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }
        private IEnumerator DoAfterFrame(Action action)
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }

        private bool HasDeathAnimation()
        {
            bool isSkeleton = EnemyID.ToString().Contains(k_enemyWithoutDeathAnimation);
            
            return !isSkeleton;
        }

        private void EnableLoot(bool active)
        {
            if(_lootObject != null) _lootObject.enabled = active;
        }

        private void RestViewPosition()
        {
            transform.GetChild(0).localPosition = Vector3.zero;
        }
    }
}
