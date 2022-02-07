using System;
using System.Collections;
using EnemiesAttack;
using Game.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Game.QuestSystem.Map.Extra
{
    public class ShelterCutscene : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private UnityEvent onNotUpgraded;
        [SerializeField] private UnityEvent onUpgraded;
        #pragma warning restore 0649
        #endregion

        private ShelterUpgradeModel ShelterUpgradeModel => ModelsSystem.Instance._shelterUpgradeModel;
        
        public void Check()
        {
            bool isUpgraded = ShelterUpgradeModel.UpgradedInThisChapter;

            if(isUpgraded)
            {
                onUpgraded?.Invoke();
            }
            else
            {
                onNotUpgraded?.Invoke();
            }
        }
    }
}