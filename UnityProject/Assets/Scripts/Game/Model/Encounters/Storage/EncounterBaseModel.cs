using System;
using Core.Storage;
using Encounters;
using UnityEngine;

namespace Game.Models.Encounters
{
    public class EncounterBaseModel : InitableModel<EncounterBaseModel.Data>, IEncounterStorage
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private Data _data;
#pragma warning restore 0649
        #endregion
        protected override Data DataBase => _data;

        [Serializable]
        public class Data : DataBase
        {
            #region Data
#pragma warning disable 0649
            [SerializeField] private long lastSpawnTimeTicks = 0;
            [SerializeField] private int counter;

#pragma warning restore 0649
            #endregion

            public long LastSpawnTimeTicks { get => lastSpawnTimeTicks; set { lastSpawnTimeTicks = value; ChangeData(); } }
            public int CompletionCounter { get => counter; set { counter = value; ChangeData(); } }
        }

        public long LastSpawnTimeTicks { get => _data.LastSpawnTimeTicks; set => _data.LastSpawnTimeTicks = value; }
        public int CompletionCounter { get => _data.CompletionCounter; set => _data.CompletionCounter = value; }

        public void Reset()
        {
            LastSpawnTimeTicks = 0;
            CompletionCounter = 0;
        }
    }
}