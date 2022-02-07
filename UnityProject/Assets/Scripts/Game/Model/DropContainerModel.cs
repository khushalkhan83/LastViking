using System;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class DropContainerModel : MonoBehaviour
    {
        public event Func<Vector3, float, List<SavableItem>, bool, List<LootObject>> OnDropContainer;

        public List<LootObject> DropContainer(Vector3 point, float randomizePosition, List<SavableItem> items, bool bigChest = false)
        {
            return OnDropContainer?.Invoke(point, randomizePosition, items, bigChest);
        }
    }
}
