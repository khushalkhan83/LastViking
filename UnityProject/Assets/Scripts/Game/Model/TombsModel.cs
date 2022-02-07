using System;
using UnityEngine;

namespace Game.Models
{
    public class TombsModel : MonoBehaviour
    {
        public enum CreationType {Default,RespawnPoint}

        #region Data
#pragma warning disable 0649
        [SerializeField] public int _tokenConfigId;

#pragma warning restore 0649
        #endregion

        public int TokenConfigId => _tokenConfigId;
        public CreationType Creation {get; private set;}

        public event Action OnCreateTomb;

        public void CreateTomb()
        {
            OnCreateTomb?.Invoke();
        }

        public void SetTombCreationType(CreationType creationType) => Creation = creationType;
    }
}
