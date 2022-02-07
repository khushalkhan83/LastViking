using UnityEngine;

namespace ChunkLoaders
{
    [CreateAssetMenu(fileName = "SO_config_chunkLoaders_default", menuName = "Configs/Chunks/ChunkLoaders", order = 0)]
    public class ChunkLoadersConfig : ScriptableObject
    {
        [SerializeField] private Vector3 _loadSize;
        public Vector3 LoadSize => _loadSize;
    }
}