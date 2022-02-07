using Game.Audio;
using Game.Controllers;
using Game.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class BarrierDamageReceiver : MonoBehaviour, IDamageable, IOutlineTarget
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private GameObject _initRenderer;
        [SerializeField] private GameObject _brokenRenderer;
        [SerializeField] private List<Renderer> _renderers;
        

#pragma warning restore 0649
        #endregion
        
        private IHealth _health;
        public IHealth Health => _health ?? (_health = GetComponentInParent<IHealth>());

        public GameObject InitRenderer => _initRenderer;
        public GameObject BrokenRenderer => _brokenRenderer;
        public List<Renderer> Renderers => _renderers;
        private bool _renderersUpdated = false;

        public void Damage(float value, GameObject from = null)
        {
            Health.AdjustHealth(-value);
            if (Health.IsDead)
            {
                OnDead();
            }
        }
        private void OnDead()
        {
            InitRenderer.SetActive(false);
            BrokenRenderer.SetActive(true);
            BrokenRenderer.GetComponent<DestroyAfterLifeTimeController>().enabled = true;
            BrokenRenderer.transform.parent = transform.parent.parent;
        }

        #region Oultine related
            
        public event Action<IOutlineTarget> OnUpdateRendererList;

        public int GetColor()
        {
            return 0;
        }

        public List<Renderer> GetRenderers()
        {
            if(_renderersUpdated == false)
                OnUpdateRendererList?.Invoke(this);

            return Renderers;
        }

        public bool IsUseWeaponRange()
        {
            return true;
        }
        #endregion

    }
}
