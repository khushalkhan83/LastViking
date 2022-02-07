using Extensions;
using Game.Models;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Progressables
{
    public class ProgressResetter : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private EnvironmentSceneID _sceneID;
        [SerializeField] private List<MonoBehaviour> progressables;
        
        #pragma warning restore 0649
        #endregion
        private DungeonProgressModel DungeonProgressModel => ModelsSystem.Instance._dungeonsProgressModel.GetDungeonProgressModel(_sceneID);
        private IEnumerable<IProgressable> Progressables => progressables.Select(p => p.GetComponent<IProgressable>());

        #region MonoBehaviour
        private void OnEnable()
        {
            DungeonProgressModel.OnPassedLocation += OnPassedLocation;
        }
        private void OnDisable()
        {
            DungeonProgressModel.OnPassedLocation -= OnPassedLocation;
        }

        // ! должно выполниться после инициализации data всех progressables
        private void Start()
        {
            if (DungeonProgressModel.ProgressStatus == ProgressStatus.NotInProgress)
            {
                foreach (var p in Progressables)
                {
                    p.ProgressStatus = ProgressStatus.NotInProgress;
                }
                DungeonProgressModel.EnterLocation();
            }
        }
        #endregion

        private void OnPassedLocation(DungeonProgressModel dungeonProgressModel)
        {
            foreach (var p in Progressables)
            {
                p.ClearProgress();
            }
        }

        [Button ("Reset All Progressables")]
        private void ResetAllProgressables()
        {
            #if UNITY_EDITOR
            if(!this.IsOpenedInPrefabMode())
            {
                EditorUtility.DisplayDialog("Важная информация", "Reset All Progressables работает только в режиме префаба", "Ok");
                return;
            }

            var children = transform.GetComponentsInChildren<MonoBehaviour>();
            progressables.Clear();
            progressables = children.Where(c => c is IProgressable).ToList();

            EditorUtility.SetDirty(this);
            #endif
        }
    }
}