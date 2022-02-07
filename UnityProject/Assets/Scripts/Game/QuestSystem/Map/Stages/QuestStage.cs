using System.Collections.Generic;
using Extensions;
using Game.Models;
using Game.Providers;
using Game.QuestSystem.Data.QuestTriggers;
using Game.QuestSystem.Map.Triggers;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game.QuestSystem.Map.Stages
{
    public class QuestStage : MonoBehaviour
    {
        [SerializeField] private List<QuestTrigger> triggers = new List<QuestTrigger>();


        [Button]
        private void ResetData()
        {
            #if UNITY_EDITOR
            if(!this.IsOpenedInPrefabMode())
            {
                EditorUtility.DisplayDialog("Важная информация", "ResetData работает только в режиме префаба", "Ok");
                return;
            }

            triggers.Clear();
            var newTriggers = GetComponentsInChildren<QuestTrigger>(includeInactive:true);
            triggers.AddRange(newTriggers);

            EditorUtility.SetDirty(this);

            #endif
        }
    }
}