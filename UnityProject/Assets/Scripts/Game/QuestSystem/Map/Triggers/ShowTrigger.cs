using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.QuestSystem.Map.Triggers
{
    public class ShowTrigger : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer = default;

        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;

        private void OnEnable() 
        {
            EditorGameSettings.OnShowQuestTriggers += OnShowQuestTriggers;
            OnShowQuestTriggers(EditorGameSettings.ShowQuestTriggers);
        }

        private void OnDisable() 
        {
            EditorGameSettings.OnShowQuestTriggers -= OnShowQuestTriggers;
        }

        private void OnShowQuestTriggers(bool showQuestTriggers)
        {
            meshRenderer.enabled = showQuestTriggers;
        }
    }
}