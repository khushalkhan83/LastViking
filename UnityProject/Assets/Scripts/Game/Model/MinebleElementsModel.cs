using System;
using System.Collections.Generic;
using Game.Providers;
using UnityEngine;

namespace Game.Models
{
    public class MinebleElementsModel : MonoBehaviour
    {
        private Dictionary<OutLineMinableObjectID, List<MinebleElement>> outlineObjects = new Dictionary<OutLineMinableObjectID, List<MinebleElement>>();

        public Dictionary<OutLineMinableObjectID, List<MinebleElement>> OutlineObjects => outlineObjects;

        public event Action<MinebleElement> OnElementAdded;
        public event Action<MinebleElement> OnElementRemoved;

        public void RegisterMinable(OutLineMinableObjectID objectID, MinebleElement minebleElement)
        {
            List<MinebleElement> objects;
            if(OutlineObjects.TryGetValue(objectID, out objects))
            {
                objects.Add(minebleElement);
            }
            else
            {
                objects = new List<MinebleElement>() {minebleElement};
                OutlineObjects.Add(objectID, objects);
            }
            OnElementAdded?.Invoke(minebleElement);
        }

        public void UnRegisterMinable(OutLineMinableObjectID objectID, MinebleElement minebleElement)
        {
            if(OutlineObjects.TryGetValue(objectID, out List<MinebleElement> objects))
            {
                objects.Remove(minebleElement);
                OnElementRemoved?.Invoke(minebleElement);
            }
        }
        
    }

    public enum OutLineMinableObjectID
    {
        None = 0,
        Stone = 1,
        Tree = 2,
    }
}
