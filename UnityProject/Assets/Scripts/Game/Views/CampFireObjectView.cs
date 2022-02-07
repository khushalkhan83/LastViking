using System;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;

namespace Game.Views
{
    public class CampFireObjectView : MonoBehaviour, IOutlineTarget
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ParticleSystem _fireParticles;
        [SerializeField] private ParticleSystem _emberParticles;
        [SerializeField] private Firelight _fireLight;
        [SerializeField] private GameObject _haloObject;
        [SerializeField] private Transform _containerAudio;

        [SerializeField] private Transform _dropItemSpawnPoint;

#pragma warning restore 0649
        #endregion

        public void PlayFireEffect()
        {
            _emberParticles.Play();
            _fireParticles.Play(true);
        }

        public void StopFireEffect()
        {
            _emberParticles.Play();
            _fireParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        public Transform ContainerAudio => _containerAudio;

        public void IsActiveLigthFire(bool isActive) => _fireLight.Toggle(isActive);
        public void IsActiveHalo(bool isActive) => _haloObject.SetActive(isActive);

        public Transform DropItemSpawnPoint => _dropItemSpawnPoint;

        public void IsActiveFire(bool isActive)
        {
            if (isActive)
            {
                PlayFireEffect();
            }
            else
            {
                StopFireEffect();
            }
            IsActiveLigthFire(isActive);
            //IsActiveHalo(isActive);
        }

        public int GetColor()
        {
            return 1;
        }

        public bool IsUseWeaponRange()
        {
            return false;
        }

        [SerializeField]
        List<Renderer> _renderers;

        public event Action<IOutlineTarget> OnUpdateRendererList;

        public List<Renderer> GetRenderers()
        {
            return _renderers;
        }
    }
}
