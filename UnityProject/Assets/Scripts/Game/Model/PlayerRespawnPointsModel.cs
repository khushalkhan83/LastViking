using System;
using NaughtyAttributes;
using UnityEngine;

namespace Game.Models
{
    public class PlayerRespawnPointsModel : MonoBehaviour
    {
        public bool RespawnPlayerOnGameStart {get; private set;}
        public void SetRespawnPlayerOnGameStart(bool value) => RespawnPlayerOnGameStart = value;
        
        public event Action OnPlayerRespawnedAndChunksAreLoaded;

        [Button]
        public void PlayerRespawnedAndChunksAreLoaded() => OnPlayerRespawnedAndChunksAreLoaded?.Invoke();
    }
}
