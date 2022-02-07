using UltimateSurvival;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Interactables
{
    public class InteractableRod : ItemsRelatedInteractableBase
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private UnityEvent _onUsed;
        
        private bool _used = false;

#pragma warning restore 0649
        #endregion

        #region ItemsRelatedInteractableBase

        public override SavableItem[] RequiredItems {get;} = new SavableItem[0];
        public override bool CanUse() => !_used;
        public override void Use()
        {
            _used = true;
            _onUsed.Invoke();
        }
            
        #endregion
    }
}
