using UnityEngine;

namespace ChunkLoaders
{
    [CreateAssetMenu(fileName = "SO_config_chunkLoadersVariable_default", menuName = "Variables/Chunks/ChunkLoadersVariable", order = 0)]
    public class ChunkLoadersConfigVariable : ScriptableObject
    {
        [SerializeField] private ChunkLoadersConfig value;
        public ChunkLoadersConfig Value => value;

        public void SetValue(ChunkLoadersConfig value) => this.value = value;
        public void ResetValue() => value = null;
    }
}