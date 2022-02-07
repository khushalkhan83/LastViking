using System;
using System.Collections.Generic;
using Core.Storage;
using Game.Objectives;
using UnityEngine;

namespace Game.Models
{
    public class FixOldObjectivesModel : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [Serializable]
        public class Data: DataBase, IImmortal
        {
            [SerializeField] private bool fixUsed;
            public bool FixUsed
            {
                get => fixUsed;
                set {fixUsed = value; ChangeData();}
            }
        }

        [SerializeField] private Data _data;
        
        #pragma warning restore 0649
        #endregion

        public DataBase _Data => _data;


        public bool FixUsed => _data.FixUsed;
        public bool SetFixUsed(bool value) => _data.FixUsed = value;

        public List<ObjectiveID> OjbectivesToIgnoreResetObjectives {get;} = new List<ObjectiveID>
            {ObjectiveID.TombInteract};
    }
}
