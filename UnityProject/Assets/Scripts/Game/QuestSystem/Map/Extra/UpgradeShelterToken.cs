using System;
using System.Collections;
using Game.Models;
using NaughtyAttributes;
using UnityEngine;

namespace Game.QuestSystem.Map.Extra
{
    public class UpgradeShelterToken : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private TokenTarget tokenTarget;
        
        #pragma warning restore 0649
        #endregion

        private ShelterAttackModeModel ShelterAttackModeModel => ModelsSystem.Instance._shelterAttackModeModel;

        #region MonoBehaviour
        private void OnEnable()
        {
            ShelterAttackModeModel.OnAttackModeActiveChanged += UpdateTocken;
            UpdateTocken();
        }

        private void OnDisable()
        {
            ShelterAttackModeModel.OnAttackModeActiveChanged -= UpdateTocken;

            DisableTocken();
        }
        #endregion

        private void UpdateTocken()
        {
            tokenTarget.enabled = !ShelterAttackModeModel.AttackModeActive;
        }

        private void DisableTocken() => tokenTarget.enabled = false;
    }
}