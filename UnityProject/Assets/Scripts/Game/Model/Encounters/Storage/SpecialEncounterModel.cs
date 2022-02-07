using System;
using UnityEngine;
using Encounters;

namespace Game.Models.Encounters
{
    public class SpecialEncounterModel : InitableModel<SpecialEncounterModel.Data>, IEncounterStorage
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private Data _data;
#pragma warning restore 0649
        #endregion
        protected override Data DataBase => _data;

        [Serializable]
        public class Data : EncounterBaseModel.Data
        {
            #region Data
#pragma warning disable 0649
            [SerializeField] private bool isActivated;
            [SerializeField] private int spawnPointIndex;

#pragma warning restore 0649
            #endregion

            public bool IsActivated { get => isActivated; set { isActivated = value; ChangeData(); } }
            public int SpawnPointIndex { get => spawnPointIndex; set { spawnPointIndex = value; ChangeData(); } }
        }

        public long LastSpawnTimeTicks { get => _data.LastSpawnTimeTicks; set => _data.LastSpawnTimeTicks = value; }
        public int CompletionCounter { get => _data.CompletionCounter; set => _data.CompletionCounter = value; }
        public bool IsActivated { get => _data.IsActivated; set => _data.IsActivated = value; }
        public int SpawnPointIndex { get => _data.SpawnPointIndex; set => _data.SpawnPointIndex = value; }

        public void Reset()
        {
            LastSpawnTimeTicks = 0;
            CompletionCounter = 0;
            IsActivated = false;
            SpawnPointIndex = 0;
        }
    }
}