using System;
using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UnityEngine;

namespace Game.QuestSystem.Map.Extra.AI
{
    public class QuestEnemyDeath : MonoBehaviour
    {
        private IHealth health;
        private void Awake()
        {
            health = GetComponentInChildren<IHealth>();
        }
        private void OnEnable()
        {
            health.OnDeath += OnDeath;
        }
        private void OnDisable()
        {
            health.OnDeath -= OnDeath;
        }

        private void OnDeath()
        {
            gameObject.transform.SetParent(null);
        }
    }
}

