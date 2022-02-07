using Core.Storage;
using Game.Objectives.Data;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;

namespace Game.Objectives
{
    public class ObjectivesModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] public ObjectivesModelData _objectivesModelData;

#pragma warning restore 0649
        #endregion

        public DataBase ObjectivesModelData => _objectivesModelData;

        public event Action<ObjectiveModel> OnAddObjective;
        public event Action<ObjectiveModel> OnRemoveObjective;
        public event Action OnReadyData;

        public event Func<ObjectivesData, IEnumerable<ObjectiveModel>> OnUnpackData;
        public event Func<IEnumerable<ObjectiveModel>, ObjectivesData> OnPackData;
        public event Func<ObjectiveID, ObjectiveModel> OnCreate;

        public List<ObjectiveModel> ObjectiveModels { get; } = new List<ObjectiveModel>();

        public bool IsReadyData { get; private set; }

        #region Public Methods
            
        public void UnpackData()
        {
            SetObjectiveModels(OnUnpackData(_objectivesModelData.ObjectivesData));
            ReadyData();
        }

        public void PackData() => SetObjectivesData(OnPackData(ObjectiveModels));

        public IEnumerable<ObjectiveModel> GetUnpackedData() => OnUnpackData(_objectivesModelData.ObjectivesData);

        public ObjectiveModel CreateAndAddObjective(ObjectiveID objectiveID) // TODO: rename to RemoveModel or objective
        {
            var result = OnCreate(objectiveID);
            AddObjective(result);
            return result;
        }

        public ObjectiveModel GetObjective(ObjectiveID objectiveID)
        {
            var result = OnCreate(objectiveID);
            return result;
        }

        public ObjectiveModel AddObjective(ObjectiveModel model)
        {
            ObjectiveModels.Add(model);
            OnAddObjective?.Invoke(model);
            return model;
        }
        public void RemoveObjective(ObjectiveModel objectiveModel) // TODO: rename to RemoveModel or objective
        {
            if(objectiveModel == null) return;
            ObjectiveModels.Remove(objectiveModel);
            OnRemoveObjective?.Invoke(objectiveModel);
        }

        private void SetObjectivesData(ObjectivesData objectivesData) => _objectivesModelData.SetObjectivesData(objectivesData);

        public void SetObjectiveModels(IEnumerable<ObjectiveModel> objectiveModels)
        {
            ObjectiveModels.Clear();
            ObjectiveModels.AddRange(objectiveModels);
        }

        public ushort GetId(ObjectiveModel model) => (ushort)ObjectiveModels.IndexOf(model);

        public ObjectiveModel Get(ushort id) => ObjectiveModels[id];
        public bool TryGet(ushort id, out ObjectiveModel answer)
        {
            answer = null;
            bool indexValid = ObjectiveModels.IndexIsValid(id);
            
            if(indexValid) answer = ObjectiveModels[id];
             
            return indexValid;
        }

        #endregion

        #region Private methods
            
        private void ReadyData()
        {
            IsReadyData = true;
            OnReadyData?.Invoke();
        }
        
        #endregion

        [Button] void ClearData()
        {
            _objectivesModelData.ObjectivesData = new ObjectivesData();
            ObjectiveModels.Clear();
        }
    }
}
