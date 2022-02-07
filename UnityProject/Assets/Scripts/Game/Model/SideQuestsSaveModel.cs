using System;
using System.Collections.Generic;
using Core.Storage;
using Game.Providers;
using Game.QuestSystem.Data;
using UnityEngine;

namespace Game.Models
{
    public class SideQuestsSaveModel : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649

        [Serializable]
        public class Data : DataBase
        {
            [SerializeField] private List<SideQuestProgressSave> _save = new List<SideQuestProgressSave>();

            public List<SideQuestProgressSave> Save
            {
                get {return _save;}
                set {_save = value; ChangeData();}
            }
        }

        private static SideQuestsProvider SideQuestsProvider => ModelsSystem.Instance._sideQuestsProvider;

        [Serializable]
        public class SideQuestProgress
        {
            public QuestData questData;
            public SideQuestState state;
            public int stage;

            public SideQuestProgress(QuestData questData, SideQuestState state, int stage)
            {
                this.questData = questData;
                this.state = state;
                this.stage = stage;
            }
            public SideQuestProgress(int questIndex, SideQuestState state, int stage)
            {
                this.questData = SideQuestsProvider[questIndex];
                this.state = state;
                this.stage = stage;
            }
        }
        [Serializable]
        public class SideQuestProgressSave
        {
            public int index;
            public SideQuestState state;
            public int stage;

            public SideQuestProgressSave(int index, SideQuestState state, int stage)
            {
                this.index = index;
                this.state = state;
                this.stage = stage;
            }
            public SideQuestProgressSave(QuestData quest, SideQuestState state, int stage)
            {
                SideQuestsProvider.TryGetQuestIndex(quest, out int index);

                this.index = index;
                this.state = state;
                this.stage = stage;
            }
        }

        public enum SideQuestState
        {
            NotActive,
            Active,
            Compleated
        }

        

        [SerializeField] private Data _data;
        [SerializeField] private List<SideQuestProgress> _defaultProgress = new List<SideQuestProgress>();
        [SerializeField] private bool _debugMode;
        
        #pragma warning restore 0649
        #endregion

        public DataBase _Data => _data;
        public bool DebugMode => _debugMode;

        public void SaveProgress(List<SideQuestProgress> progress)
        {
            _data.Save = GetSaveFromProgress(progress); 
        }

        public List<SideQuestProgress> GetProgress() => GetProgressFromSave(_data.Save);
        
        private List<SideQuestProgress> GetProgressFromSave(List<SideQuestProgressSave> saves)
        {
            var answer = new List<SideQuestProgress>();
            foreach (var save in saves)
            {
                var progress = new SideQuestProgress(save.index,save.state,save.stage);
                answer.Add(progress);
            }

            return answer;
        }

        private List<SideQuestProgressSave> GetSaveFromProgress(List<SideQuestProgress> progresses)
        {
            var answer = new List<SideQuestProgressSave>();
            foreach (var progress in progresses)
            {
                var save = new SideQuestProgressSave(progress.questData,progress.state,progress.stage);
                answer.Add(save);
            }

            return answer;
        }

        public void SetDefaultProgress() => _data.Save = GetSaveFromProgress(_defaultProgress);
    }
}
