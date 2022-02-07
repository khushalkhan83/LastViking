using System;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;

namespace Game.Views
{
    public class LoomObjectView : MonoBehaviour,IOutlineTarget
    {
        #region Data
#pragma warning disable 0649
        
        [SerializeField] private Transform _dropItemSpawnPoint;
        [SerializeField] private Animator _animator;
        [SerializeField] private string _weavingAnim;
        [SerializeField] private Transform _containerAudio;

#pragma warning restore 0649
        #endregion

        public Transform DropItemSpawnPoint => _dropItemSpawnPoint;
        public Animator Animator => _animator;
        public Transform ContainerAudio => _containerAudio;

        public void IsActiveWeave(bool isActive)
        {
            _animator.SetBool(_weavingAnim, isActive);
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
