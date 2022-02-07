using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extensions;
using Game.Models;
using Game.QuestSystem.Data;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.QuestSystem.Map.Quests
{
    // [ExecuteAlways]
    public class QuestsRoot : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private List<Quest> quests = new List<Quest>();
        [SerializeField] private List<Quest> optionalQuests = new List<Quest>();
        
        #pragma warning restore 0649
        #endregion

        private QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;
        private SideQuestsModel SideQuestsModel => ModelsSystem.Instance._sideQuestsModel;
        private TutorialModel TutorialModel => ModelsSystem.Instance?._tutorialModel;
        private CinematicModel CinematicModel => ModelsSystem.Instance?._cinematicModel;

        private bool sceneIsSaving;

        #region MonoBehaviour
        private void OnEnable()
        {
            if(sceneIsSaving) return;
            if(this.IsOpenedInPrefabMode()) return;

            if(!this.IsEditMode() && !TutorialModel.IsComplete)
            {
                TutorialModel.OnComplete += MainLogic;
            }
            else
                MainLogic();
        }

        private void OnDisable()
        {
            if(sceneIsSaving) return;
            if(this.IsOpenedInPrefabMode()) return;

            if(TutorialModel != null)
                TutorialModel.OnComplete -= MainLogic;

            #if UNITY_EDITOR
            EditorSceneManager.sceneSaving -= OnSceneSaving;
            EditorSceneManager.sceneSaved -= OnSceneSaved;
            #endif
            
            QuestsModel.OnActivateQuest -= ActivateOnlyActiveQuest;
            SideQuestsModel.OnActivate -= ActivateSideQuest;
            SideQuestsModel.OnCompleate -= CompleateSideQuest;
        }
        #endregion


        private void MainLogic()
        {
            #if UNITY_EDITOR
            EditorSceneManager.sceneSaving += OnSceneSaving;
            EditorSceneManager.sceneSaved += OnSceneSaved;
            #endif

            QuestsModel.OnActivateQuest += ActivateOnlyActiveQuest;
            SideQuestsModel.OnActivate += ActivateSideQuest;
            SideQuestsModel.OnCompleate += CompleateSideQuest;

            CinematicModel.SetCanResetInEditor(false);
            ActivateOnlyActiveQuest();
            ActivateOnlyActiveSideQuests();
            CinematicModel.SetCanResetInEditor(true);
        }

        private void ActivateSideQuest(QuestData questData) => SetSideQuestActive(questData,true);
        private void CompleateSideQuest(QuestData questData) => SetSideQuestActive(questData,false);

        private void SetSideQuestActive(QuestData questData, bool active)
        {
            var sideQuest = GetSideQuest(questData);
            if(sideQuest != null)
            {
                sideQuest.gameObject.SetActive(active);
            }
        }
        

        // TODO: move remove prefab overrides to other component
        private void OnSceneSaving(Scene scene, string path)
        {
            #if UNITY_EDITOR

            #region Logic
            if(scene != gameObject.scene) return;
            sceneIsSaving = true;

            var modifications = PrefabUtility.GetPropertyModifications(gameObject);
            var filteredModifications = modifications.Where(x => ShowPropertyModificationInDialogeDisplay(x)).ToList();

            if(filteredModifications.Count == 0)
            {
                RemoveOverrides();
                return;
            }

            bool removeOverrides =  EditorUtility.DisplayDialog("Отмена изменений в префабе на сцене", GetMessage(), "Да", "Сохранить изменения");

            if(removeOverrides)
                RemoveOverrides();
            else
                KeepOverrides();
                
                
            #endregion

            #region Help methods
                
            void RemoveOverrides()
            {
                PrefabUtility.RevertPrefabInstance(this.gameObject,InteractionMode.UserAction);
                Debug.Log("Undone QuestsRoot overrides");
            }

            void KeepOverrides()
            {
                Debug.Log("Keep QuestsRoot overrides");
            }

            string GetMessage()
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine("Данные изменения для квестов будут отменены. Продолжить?").AppendLine();
                foreach (var property in filteredModifications)
                {
                    message.AppendLine(property.propertyPath + ":" + property.value + " in " + property.target.name);
                }
                return message.ToString();
            }
            #endregion
            #endif
        }

        #if UNITY_EDITOR
        private bool ShowPropertyModificationInDialogeDisplay(PropertyModification p)
        {
            if(p.target.name == gameObject.name) return false;
            if(p.target.name.Contains("Quest") && p.propertyPath == "m_IsActive") return false;
            if(p.target.name.Contains("Stage") && p.propertyPath == "m_IsActive") return false;

            return true;
        }
        #endif

        private void OnSceneSaved(Scene scene)
        {
            #if UNITY_EDITOR
            if(scene != gameObject.scene) return;

            ActivateOnlyActiveQuest();
            ActivateOnlyActiveSideQuests();

            sceneIsSaving = false;
            Debug.Log("QuestsRoot saved");
            #endif
        }

        private void ActivateOnlyActiveQuest()
        {
            ActivateOnlyActiveQuest(QuestsModel.ActiveQuest);
        }

        private void ActivateOnlyActiveSideQuests()
        {
            var activeSideQuests = SideQuestsModel.GetActiveQuests();

            foreach (QuestData data in activeSideQuests)
            {
                ActivateSideQuest(data);
            }
        }


        public Quest GetQuest(QuestData quest)
        {
            var match = quests.Find(x => x.Data == quest);
            return match;
        }

        public Quest GetSideQuest(QuestData quest)
        {
            var match = optionalQuests.Find(x => x.Data == quest);
            return match;
        }

        private void ActivateOnlyActiveQuest(QuestData quest)
        {
            var manager = GetQuest(quest);

            foreach (var questManager in quests)
            {
                bool questIsActive = questManager == manager;

                questManager.gameObject.SetActive(questIsActive);
            }
        }
   
        public bool IsMainQuest(Quest quest)
        {
            bool isMainQuest = quests.Contains(quest);
            return isMainQuest;
        }

        [Button]
        private void ResetData()
        {
            #if UNITY_EDITOR
            if(!this.IsOpenedInPrefabMode())
            {
                EditorUtility.DisplayDialog("Важная информация", "ResetData работает только в режиме префаба", "Ok");
                return;
            }

            quests.Clear();
            optionalQuests.Clear();

            foreach(Transform child in transform)
            {
                if (child.name == "MainQuest")
                {
                    var newQuests = child.GetComponentsInChildren<Quest>(includeInactive:true);
                    quests.AddRange(newQuests);
                }
                else if(child.name == "SideQuests")
                {
                    var newQuests = child.GetComponentsInChildren<Quest>(includeInactive:true);
                    optionalQuests.AddRange(newQuests);
                }
            }
            
            EditorUtility.SetDirty(this);

            #endif
        }
    }
}