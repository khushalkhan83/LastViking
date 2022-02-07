using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Misc
{
    public class TransformHelper : MonoBehaviour
    {
        [SerializeField] private Transform destination = default;
        public void MoveTarget(Transform target)
        {
            target.transform.position = target.transform.position;
        }

        public void SetParrent(Transform target)
        {
            target.SetParent(destination);
        }
    }
}
