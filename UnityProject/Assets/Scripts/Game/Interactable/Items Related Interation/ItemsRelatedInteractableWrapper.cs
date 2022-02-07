using System;
using System.Collections;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;

namespace Game.Interactables
{
    public class ItemsRelatedInteractableWrapper : ItemsRelatedInteractableBase, IOutlineTarget, Game.Models.IDamageable
    {
        [SerializeField] private ItemsRelatedInteractableBase itemsRelatedInteractable = default;
        [SerializeField] List<Renderer> _renderers;
        [SerializeField] bool _useByHit = false;

        public override SavableItem[] RequiredItems => itemsRelatedInteractable.RequiredItems;

        public event Action<IOutlineTarget> OnUpdateRendererList;

        public override bool CanUse() => itemsRelatedInteractable.CanUse();

        public int GetColor()
        {
             return 1;
        }

        public List<Renderer> GetRenderers()
        {
            return _renderers;
        }

        public bool IsUseWeaponRange()
        {
            return false;
        }
        
        public void Damage(float value, GameObject from = null)
        {
            if(_useByHit && CanUse())
            {
                Use();
            }
        }

        public override void Use() => itemsRelatedInteractable.Use();
    }
}
