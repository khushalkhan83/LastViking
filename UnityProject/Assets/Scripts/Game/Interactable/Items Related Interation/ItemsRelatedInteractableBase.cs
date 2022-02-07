using System.Collections;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;

namespace Game.Interactables
{
    public abstract class ItemsRelatedInteractableBase : MonoBehaviour
    {
        public abstract SavableItem[] RequiredItems { get; }
        public abstract bool CanUse();
        public abstract void Use();
    }
}
