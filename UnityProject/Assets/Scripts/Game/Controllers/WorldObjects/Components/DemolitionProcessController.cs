using System.Collections;
using Game.Models;
using UnityEngine;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Part;

namespace Game.Controllers
{
    public class DemolitionProcessController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private DamageReceiver _damageReceiver;
        [SerializeField] private DemolitionProcessModel _demolitionProcessModel;
        [SerializeField] private Collider _ownCollider;
        [SerializeField] private LayerMask _overlapMask;
        [SerializeField] private Vector3 _overlapBoxPosition;
        [SerializeField] private Vector3 _overlapBoxSize;
        
#pragma warning restore 0649
        #endregion

        private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;

        private Coroutine _demolitionCoroutine;
        private Collider[] _overlapColliders = new Collider[5];

        #region MonoBehaviour
        private void OnEnable()
        {
            _demolitionProcessModel.OnDataLoaded += OnDataLoaded;
            PlayerScenesModel.OnEnvironmentLoaded += OnEnvironmentLoaded;
        }

        private void OnDisable()
        {
            EventHandlers.OnDestroyedPart -= OnDestroyPart;
            _demolitionProcessModel.OnDataLoaded -= OnDataLoaded;
            PlayerScenesModel.OnEnvironmentLoaded -= OnEnvironmentLoaded;
        }

        private void OnDrawGizmos() 
        {
            Gizmos.color = Color.green;
            Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(_overlapBoxPosition, _overlapBoxSize * 2);
            Gizmos.matrix = oldGizmosMatrix;
        }
        #endregion

        private void OnDataLoaded()
        {
            if(_demolitionProcessModel.PlacedOn == DemolitionProcessModel.PlaceType.None)
            {
                CheckPlace();
            }

            if(_demolitionProcessModel.PlacedOn == DemolitionProcessModel.PlaceType.OnConstruction)
            {
                EventHandlers.OnDestroyedPart += OnDestroyPart;
            }

            _demolitionProcessModel.OnDataLoaded -= OnDataLoaded;
        }

        private void CheckPlace()
        {
            if(IsOnConstruction())
            {
                _demolitionProcessModel.PlacedOn = DemolitionProcessModel.PlaceType.OnConstruction;
            }
            else
            {
                _demolitionProcessModel.PlacedOn = DemolitionProcessModel.PlaceType.OnGround;
            }
        }

        private void OnDestroyPart(PartBehaviour part)
        {
            CheckDemolition();
        }

        private void CheckDemolition()
        {
            if(_demolitionCoroutine == null)
            {
                StartCoroutine(DemolitionProcess());
            }
        }

        private IEnumerator DemolitionProcess()
        {
            yield return new WaitForSeconds(0.5f);
            if(!IsOnConstruction())
            {
                ((IDamageable)_damageReceiver).Damage(_damageReceiver.Health.HealthMax);
            }
            _demolitionCoroutine = null;
        }

        private bool IsOnConstruction()
        {
            int count = Physics.OverlapBoxNonAlloc(transform.position + _overlapBoxPosition, _overlapBoxSize, _overlapColliders, transform.rotation, _overlapMask);
            for(int i = 0; i < count; i++)
            {
                if(_overlapColliders[i] == _ownCollider)
                    continue;   

                if(_overlapColliders[i].tag == "Construction")
                {
                    return true;
                } 
            }
            return false;
        }

        private void OnEnvironmentLoaded()
        {
            if(_demolitionProcessModel.PlacedOn == DemolitionProcessModel.PlaceType.OnConstruction)
            {
                CheckDemolition();
            }
        }
    }

}