using System;
using System.Collections.Generic;
using UltimateSurvival.Debugging;
using UnityEngine;

namespace UltimateSurvival
{
    [Serializable]
    public class ReorderableGenericList<T> : IEnumerable<T>
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private List<T> m_List;

#pragma warning restore 0649
        #endregion

        public T this[int key] { get { return m_List[key]; } set { m_List[key] = value; } }

        public int Count { get { return m_List.Count; } }

        public List<T> List { get { return m_List; } }

        public IEnumerator<T> GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [Serializable]
    public class ReorderableBoolList : ReorderableGenericList<bool> { }

    [Serializable]
    public class ReorderableIntList : ReorderableGenericList<int> { }

    [Serializable]
    public class ReorderableFloatList : ReorderableGenericList<float> { }

    [Serializable]
    public class ReorderableStringList : ReorderableGenericList<string> { }

    [Serializable]
    public class ReorderableVector2List : ReorderableGenericList<Vector2> { }

    [Serializable]
    public class ReorderableVector3List : ReorderableGenericList<Vector3> { }

    [Serializable]
    public class ReorderableQuaternionList : ReorderableGenericList<Quaternion> { }

    [Serializable]
    public class ReorderableTransformList : ReorderableGenericList<Transform> { }

    [Serializable]
    public class ReorderableRectTransformList : ReorderableGenericList<RectTransform> { }

    [Serializable]
    public class ItemsList : ReorderableGenericList<ItemMeta> { }
}