using System.Collections;
using Game.Models;
using Game.QuestSystem.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Game.QuestSystem.Map.Extra
{
    public class SideQuestCompleateCheck : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private UnityEvent onCompleate;
        [SerializeField] private QuestData sideQuest;
        #pragma warning restore 0649
        #endregion

        private SideQuestsModel SideQuestsModel => ModelsSystem.Instance._sideQuestsModel;

        #region MonoBehaviour
        private void OnEnable()
        {
            StartCoroutine(InitWithDelay());
        }

        private IEnumerator InitWithDelay()
        {
            yield return null;
            SideQuestsModel.OnCompleate += OnCompleate;

            Check();
        }

        private void OnDisable()
        {
            SideQuestsModel.OnCompleate -= OnCompleate;
        }
        #endregion

        private void OnCompleate(QuestData data) => Check();
        
        public void Check()
        {
            bool isCompleate = SideQuestsModel.IsQuestCompleated(sideQuest);

            if(isCompleate)
            {
                onCompleate?.Invoke();
            }
        }
    }
}