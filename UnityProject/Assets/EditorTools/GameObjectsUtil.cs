using System.Collections.Generic;
using UnityEngine;

namespace CustomeEditorTools
{
    public static class GameObjectsUtil
    {
        private static List<GameObject> result = new List<GameObject>();

        public static List<GameObject> GetObjectsByFilter(GameObject rootObject, List<string> filter)
        {
            #region Logic
            
            result = new List<GameObject>();
            var answer = GetAllChilds(rootObject);
            var filteredAnswer = FilterObjects(answer, filter);

            return filteredAnswer;

            #endregion
        }

        public static List<GameObject> GetAllChildsAndParrent(GameObject go)
        {
            result.Clear();
            result.Add(go);
            var childs = GetAllChilds(go);
            result.AddRange(childs);
            return result;
        }

        public static List<GameObject> GetChildsAndParrent(GameObject go)
        {
            result.Clear();
            result.Add(go);
            foreach (Transform child in go.transform)
            {
                if (result.Contains(child.gameObject)) continue;

                result.Add(child.gameObject);
            }
            return result;
        }

        private static List<GameObject> GetAllChilds(GameObject go)
        {
            foreach (Transform child in go.transform)
            {
                if (result.Contains(child.gameObject)) continue;

                result.Add(child.gameObject);

                GetAllChilds(child.gameObject);
            }

            return result;
        }
        private static List<GameObject> FilterObjects(List<GameObject> input, List<string> filter)
        {
            var inputCopy = new List<GameObject>(input);
            List<GameObject> fitleredAnswer = new List<GameObject>();
            foreach (var go in inputCopy)
            {
                bool childPassFilter = PassFilter(go,filter);
                if (childPassFilter) fitleredAnswer.Add(go);
            }

            return fitleredAnswer;
        }

        private static bool PassFilter(GameObject objectToFilter, List<string> filter)
        {
            foreach (var filterText in filter)
            {
                if (objectToFilter.name.Contains(filterText)) continue;
                else return false;
            }
            return true;
        }
    }
}