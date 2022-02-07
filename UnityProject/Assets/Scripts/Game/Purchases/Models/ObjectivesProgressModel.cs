using System;
using System.Collections.Generic;
using Core.Storage;
using NaughtyAttributes;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

namespace Game.Models
{
    public class ObjectivesProgressModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [Serializable]
        public class ObjectiveProgress
        {
            public ObscuredInt id;
            public ObscuredBool done;

            public ObjectiveProgress(int id, bool done)
            {
                this.id = id;
                this.done = done;
            }
        }

        [Serializable]
        public class TierProgress
        {
            public ObscuredInt id;
            public ObscuredBool done;

            public TierProgress(int id, bool done)
            {
                this.id = id;
                this.done = done;
            }
        }

        [Serializable]
        public class StackProgress
        {
            public ObscuredInt id;
            public ObscuredString selectedObjective;
            public ObscuredInt compoziteStackId;
            public ObscuredBool IsCompozite;

            public StackProgress(int id, string selectedObjective)
            {
                this.id = id;
                this.selectedObjective = selectedObjective;
                this.IsCompozite = false;
            }

            public StackProgress(int id, string selectedObjective, int compoziteStackId)
            {
                this.id = id;
                this.selectedObjective = selectedObjective;
                this.compoziteStackId = compoziteStackId;
                this.IsCompozite = true;
            }
        }

        [Serializable]
        public class Data : DataBase, IImmortal //TODO: refactor (remove dublciate code for get set)
        {
            public ObscuredBool initedRequiredTiers;
            public List<TierProgress> tiersProgress = new List<TierProgress>();
            public List<StackProgress> stacksProgress = new List<StackProgress>();
            public List<ObjectiveProgress> objectivesProgress = new List<ObjectiveProgress>();

            public List<StackProgress> GetStacksData() => stacksProgress;

            
            public void SetInitedRequiredTiers(bool value)
            {
                initedRequiredTiers = value;
                ChangeData();
            }
            public void SetTierState(int id, bool done)
            {
                var match = tiersProgress.Find(x => x.id == id);

                if (match == null)
                    tiersProgress.Add(new TierProgress(id, done));
                else
                    match.done = done;

                ChangeData();
            }

            public bool GetTiersState(int id)
            {
                var match = tiersProgress.Find(x => x.id == id);

                if (match == null) return false;
                else return match.done;
            }

            public void SetStacksData(List<StackProgress> datas)
            {
                stacksProgress.Clear();
                stacksProgress = datas;
                ChangeData();
            }

            public void SetObjectiveState(int id, bool done)
            {
                var match = objectivesProgress.Find(x => x.id == id);

                if (match == null)
                    objectivesProgress.Add(new ObjectiveProgress(id, done));
                else
                    match.done = done;

                ChangeData();
            }

            public bool GetObjectiveState(int id)
            {
                var match = objectivesProgress.Find(x => x.id == id);

                if (match == null) return false;
                else return match.done;
            }
            public bool GetInitedRequierdTiers() => initedRequiredTiers;
            public void ClearSave()
            {
                objectivesProgress.Clear();
                tiersProgress.Clear();
                initedRequiredTiers = false;
                stacksProgress.Clear();
                ChangeData();
            }
        }
        [SerializeField] private Data _data;


#pragma warning restore 0649
        #endregion
        public DataBase _Data => _data;

        public event Action OnSave;
        public event Action OnSetObjectivewsWindow;
        public event Action OnSaveAsEmpty;
        public event Action OnLoadFromSave;

        public bool InitedRequiredTiers() => _data.initedRequiredTiers;
        public void SetInitedRequiredTiers(bool value) => _data.SetInitedRequiredTiers(value);
        public void SetTierState(int id, bool value) => _data.SetTierState(id, value);
        public bool GetTiersState(int id) => _data.GetTiersState(id);
        public void SetStackDatas(List<StackProgress> datas) => _data.SetStacksData(datas);
        public List<StackProgress> GetStackDatas() => _data.GetStacksData();
        public void SetObjectiveState(int id, bool value) => _data.SetObjectiveState(id, value);
        public bool GetObjestiveState(int id) => _data.GetObjectiveState(id);

        public void ClearSave() => _data.ClearSave();

        [Button]
        public void SaveObjectives() => OnSave?.Invoke();
        [Button]
        public void SetObjectivewsWindow() => OnSetObjectivewsWindow?.Invoke();
        [Button]
        public void SaveAsEmpty() => OnSaveAsEmpty?.Invoke();
        [Button]
        public void LoadFromSave() => OnLoadFromSave?.Invoke();
    }
}