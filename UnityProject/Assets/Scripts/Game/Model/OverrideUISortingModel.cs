using System;
using UnityEngine;
using System.Collections.Generic;
using Extensions;

namespace Game.Models
{
    public class OverrideUISortingModel : MonoBehaviour
    {
        private Dictionary<string,List<GameObject>> objectsByGroupId = new Dictionary<string, List<GameObject>>();

        public List<GameObject> GetObjectsByGroup(string groupId)
        {
            objectsByGroupId.TryGetValue(groupId, out var list);

            return list;
        }

        public void SetOverrideSorting(string groupId, GameObject target, bool overrideSorting)
        {
            if(target == null) return;
            Override(target, overrideSorting);

            if (overrideSorting)
                AddTargetByGroup(groupId,target);
            else
                RemoveTargetByGroup(groupId,target);
        }

        private void AddTargetByGroup(string groupId, GameObject target)
        {
            objectsByGroupId.TryGetValue(groupId, out var list);

            if(list == null)
            {
                list = new List<GameObject>() {target};
                objectsByGroupId.Add(groupId,list);
            }
            else
                list.Add(target);
        }
        private void RemoveTargetByGroup(string groupId, GameObject target)
        {
            if(objectsByGroupId.TryGetValue(groupId, out var list))
            {
                list.Remove(target);
            }
        }

        private void Override(GameObject target, bool overrideSorting)
        {
            var hasCanvas = target.TryGetComponent(out Canvas canvas);

            if(!hasCanvas)
                canvas = target.AddComponent<Canvas>();

            canvas.SetOverrideSorting(overrideSorting);
        }

        public void RemoveOverrides(string groupId)
        {
            if(!objectsByGroupId.TryGetValue(groupId, out var list)) return;

            foreach (var item in list)
            {
                Override(item, false);
            }

            objectsByGroupId[groupId].Clear();
        }
    }
}