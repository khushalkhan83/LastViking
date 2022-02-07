using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    // [DefaultExecutionOrder(9999)]
    public class UpdateFollow : MonoBehaviour
    {
        public Transform toFollow;

        private void Update()
        {
        }

        private void LateUpdate() {
            transform.position = toFollow.position;
            transform.rotation = toFollow.rotation;
            
        }
    } 
}
