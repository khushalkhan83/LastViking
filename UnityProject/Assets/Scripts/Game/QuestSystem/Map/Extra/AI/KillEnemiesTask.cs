using System;
using System.Collections;
using System.Collections.Generic;
using Game.Models;
using SOArchitecture;
using UnityEngine;
using UnityEngine.Events;

namespace Game.QuestSystem.Map.Extra.AI
{
    public class KillEnemiesTask : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private EnemiesContextRuntimeSet group;
        [SerializeField] private UnityEvent onKilledGroup; 
        
        #pragma warning restore 0649
        #endregion

        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;

        private bool eventRegistered;

        #region MonoBehaviour

        private void OnEnable()
        {
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        private void OnDisable()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        #endregion

        private void OnUpdate()
        {
            if (group.Items.Count == 0)
                KilledAll();
        }
        private void KilledAll()
        {
            if(eventRegistered) return;
            eventRegistered = true;

            onKilledGroup?.Invoke();
        }
    }
}

