using System;
using UnityEngine;

namespace Game.Models
{
    public class ChunksLoadersModel : MonoBehaviour
    {
        public event Action OnChunkLoadersConfigChanged;
        public void ChangeChunkLoaderConfig() => OnChunkLoadersConfigChanged?.Invoke();
    }
}
