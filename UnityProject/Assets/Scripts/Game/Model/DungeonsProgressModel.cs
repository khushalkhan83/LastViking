using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using ActivityLog.Data;

namespace Game.Models
{
    public class DungeonsProgressModel : MonoBehaviour, IActivityLogEnterencesModel
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] DungeonProgressModel[] _dungeonProgressModels;
        
        #pragma warning restore 0649
        #endregion

        #region Dependencies
        private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;
            
        #endregion

        public DungeonProgressModel GetDungeonProgressModel(EnvironmentSceneID sceneID)
        {
            return _dungeonProgressModels.Where(d => d.EnvironmentSceneID == sceneID).FirstOrDefault();
        }

        public DungeonProgressModel GetCurrentDungeonProgressModel()
        {
            var model = GetDungeonProgressModel(PlayerScenesModel.ActiveEnvironmentSceneID);
            return model;
        }

        public IEnumerable<DungeonProgressModel> DungeonProgressModels => _dungeonProgressModels;


        #region IActivityLogEnterencesModel
        public event Action OnActivitiesCountChanged;
        public List<ActivityLogEnterenceData> GetActivitiesEnterences()
        {
            var answer = new List<ActivityLogEnterenceData>();
            foreach (var dungeonProgressModel in DungeonProgressModels)
            {
                if(dungeonProgressModel.TryGetActivityLogEnterence(out var logEnterenceData))
                {
                    answer.Add(logEnterenceData);
                }
            }
            return answer;
        }
        #endregion

        #region MonoBehaviour

        private void OnEnable()
        {
            foreach (var dungeonProgressModel in DungeonProgressModels)
            {
                dungeonProgressModel.OnPassedLocation += UpdateActivities;
                dungeonProgressModel.OnProgressResetted += UpdateActivities;
            }
        }

        private void OnDisable()
        {
            foreach (var dungeonProgressModel in DungeonProgressModels)
            {
                dungeonProgressModel.OnPassedLocation -= UpdateActivities;
                dungeonProgressModel.OnProgressResetted -= UpdateActivities;
            }
        }
        #endregion

        private void UpdateActivities(DungeonProgressModel model) => OnActivitiesCountChanged?.Invoke();
        private void UpdateActivities() => OnActivitiesCountChanged?.Invoke();

    }
}