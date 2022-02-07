using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChunkLoaders
{
    public class ChunkLoadersProvider : MonoBehaviour
    {
        [SerializeField] private SECTR_RegionLoader _regionLoader;
        public SECTR_RegionLoader RegionLoader => _regionLoader;
    }
}
