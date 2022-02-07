using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class ResetShipHealthController : MonoBehaviour
    {
        [SerializeField] private BuildingHealthModel buildingHealthModel = default;
        public ShelterAttackModeModel ShelterAttackModeModel => ModelsSystem.Instance._shelterAttackModeModel;

        #region MonoBehaviour
        private void OnEnable() 
        {
            ShelterAttackModeModel.OnAttackModeStart += ResetHealth;
            ShelterAttackModeModel.OnAttackModeFailed += ResetHealth;
        }

        private void OnDisable() 
        {
            ShelterAttackModeModel.OnAttackModeStart -= ResetHealth; 
            ShelterAttackModeModel.OnAttackModeFailed -= ResetHealth;
        }
        #endregion

        private void ResetHealth()
        {
            buildingHealthModel.SetHealth(buildingHealthModel.HealthMax);
        }
    }
}
