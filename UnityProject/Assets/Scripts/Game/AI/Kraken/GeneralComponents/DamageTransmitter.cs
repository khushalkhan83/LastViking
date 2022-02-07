using Game.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Game.AI.Behaviours.Kraken
{
    [RequireComponent(typeof(Collider))]
    public class DamageTransmitter : MonoBehaviour, IDamageable
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private GameObject _reciverGameObject;
        [SerializeField] private UnityEvent _onRecivedDamage;
        
        #pragma warning restore 0649
        #endregion
        private IDamageable _reciver;

        public void Damage(float value, GameObject from = null)
        {
            _reciver.Damage(value, from);
            _onRecivedDamage?.Invoke();
        }

        #region MonoBehaviour
        private void Start()
        {
            if(_reciver == null) Init();
        }

        private void OnValidate()
        {
            if(Extensions.Extensions.IsEditMode(this) && gameObject.activeInHierarchy)
                Init();
        }
        #endregion

        private void Init()
        {
            if(_reciverGameObject == null)
            {
                Debug.LogError("No target here",this);
                return;
            }

            _reciver = _reciverGameObject.GetComponent<IDamageable>();
            if(_reciver == null)
            {
                Debug.LogError("Cant find component here",this);
            }
        }
    }

}
