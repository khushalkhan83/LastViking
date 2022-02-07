using Game.Interactables;
using Game.Models;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using NaughtyAttributes;

namespace Game.QuestSystem.Map.Extra
{
    public class RequireItemsGroup : MonoBehaviour
    {
        [InfoBox("Этот компонент ресетит прогресс указанных объектов при переходе на следующую стадию")]
        #region Data
#pragma warning disable 0649
        [SerializeField] private List<RequiredItemsObject> requiredItemsObjects = new List<RequiredItemsObject>();
        [SerializeField] private UnityEvent onAllItemsPlaced;

#pragma warning restore 0649
        #endregion

        private ApplicationCallbacksModel ApplicationCallbacksModel => ModelsSystem.Instance._applicationCallbacksModel;
        private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;

        #region MonoBehaviour
        private void OnEnable()
        {

        }

        private void OnDisable()
        {
            // call RequireItemsGroup reset when state is changed. not when application is quiting
            if(ApplicationCallbacksModel.IsApplicationQuitting || PlayerScenesModel.SceneTransition) return;

            ResetItems();
        }

        private void Start() 
        {
            CheckCondition();
        }

        #endregion


        public void CheckCondition()
        {
            RequiredItemsObject notPlacedObjects = requiredItemsObjects.Find(x => x.ItemsPlaced == false);

            if (notPlacedObjects == null)
            {
                onAllItemsPlaced?.Invoke();
            }
        }

        private void ResetItems()
        {
            requiredItemsObjects.ForEach(x => x.SetItemsPlaced(false));
        }
    }
}