using System;
using Game.Models;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.Events;

namespace Game.QuestSystem.Map.Triggers
{
    public class ColliderTriggerListener : MonoBehaviour
    {
        [SerializeField] private ColliderTriggerModel[] colliders;

        [SerializeField] private UnityEvent onEnter;
        [SerializeField] private bool activateOnPlayer = true;

        private void OnEnable()
        {
            foreach (var collider in colliders)
            {
                collider.OnEnteredTrigger += OnEnteredTrigger;
                collider.OnExitedTrigger += OnExitedTrigger;
                collider.OnStayingTrigger += OnStayingTrigger;
            }
        }

        private void OnDisable()
        {
            foreach (var collider in colliders)
            {
                collider.OnEnteredTrigger -= OnEnteredTrigger;
                collider.OnExitedTrigger -= OnExitedTrigger;
                collider.OnStayingTrigger -= OnStayingTrigger;
            }
        }

        private void OnEnteredTrigger(Collider obj)
        {
            if(activateOnPlayer)
            {
                bool isPlayer = obj.GetComponent<PlayerEventHandler>();
                if(isPlayer)
                    onEnter?.Invoke();
            }
            else
                onEnter?.Invoke();
        }

        private void OnExitedTrigger(Collider obj)
        {
            
        }

        private void OnStayingTrigger(Collider obj)
        {
            
        }
    }
}