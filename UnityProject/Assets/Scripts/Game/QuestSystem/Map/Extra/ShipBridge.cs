using System;
using Game.Models;
using Game.QuestSystem.Data;
using UnityEngine;

namespace Game.QuestSystem.Map.Extra
{
    public class ShipBridge : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private GameObject _bridge;
#pragma warning restore 0649
        #endregion
        private ShelterAttackModeModel ShelterAttackModeModel => ModelsSystem.Instance._shelterAttackModeModel;

        #region MonoBehaviour
        private void OnEnable()
        {
            ShelterAttackModeModel.OnAttackModeActiveChanged += UpdateView;

            UpdateView();
        }

        private void OnDisable()
        {
            ShelterAttackModeModel.OnAttackModeActiveChanged -= UpdateView;
        }
        #endregion

        private void UpdateView()
        {
            bool hideBridge = ShelterAttackModeModel.AttackModeActive;
            _bridge.SetActive(!hideBridge);
        }
    }
}