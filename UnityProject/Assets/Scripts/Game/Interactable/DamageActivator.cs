using Game.Models;
using PlayerEventHandler = UltimateSurvival.PlayerEventHandler;
using UnityEngine;
using UnityEngine.Events;
using Game.Audio;
using System.Linq;

namespace Game.Interactables
{
    public class DamageActivator : MonoBehaviour, IDamageable
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private UnityEvent _onDamaged;
        [SerializeField] private bool _ignoreRangeWeapon;

        private bool _triggered;
        private AudioIdentifier _audioIdentifier;

        #pragma warning restore 0649
        #endregion
        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
        private AudioSystem AudioSystem => AudioSystem.Instance;


        #region MonoBehaviour

        private void Awake()
        {
            _audioIdentifier = GetComponent<AudioIdentifier>();    
        }

        #endregion
        public void Damage(float value, GameObject from = null)
        {
            if(_triggered) return;
            if(_ignoreRangeWeapon && PlayerEventHandler.PlayerShootSomething) return;

            if(_audioIdentifier != null) 
                AudioSystem.PlayOnce(_audioIdentifier.AudioID.FirstOrDefault());
            _onDamaged?.Invoke();

            _triggered = true;
        }

        public void Reset() 
        {
            _triggered = false;   
        }
    }
}
