using System;
using Core.Storage;
using UnityEngine;

namespace Game.Models
{
    public class QuestsSaveModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            [SerializeField] private int mainQuestIndex;
            [SerializeField] private int mainQuestStageIndex;

            public int MainQuestIndex
            {
                get {return mainQuestIndex;}
                set {mainQuestIndex = value; ChangeData();}
            }

            public int MainQuestStageIndex
            {
                get {return mainQuestStageIndex;}
                set {mainQuestStageIndex = value; ChangeData();}
            }
        }

        #region Data
        #pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private bool _debugMode;
        
        #pragma warning restore 0649
        #endregion
        public DataBase _Data => _data;
        public int MainQuestIndex => _data.MainQuestIndex;
        public int MainQuestStageIndex => _data.MainQuestStageIndex;
        public bool DebugMode => _debugMode;

        public void SetMainQuestIndex(int index) => _data.MainQuestIndex = index;
        public void SetMainQuestStageIndex(int index) => _data.MainQuestStageIndex = index;

    }
}
