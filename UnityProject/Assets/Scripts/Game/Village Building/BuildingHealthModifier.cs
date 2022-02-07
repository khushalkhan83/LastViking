using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UnityEngine;

namespace Game.VillageBuilding
{
    public class BuildingHealthModifier : MonoBehaviour
    {
        [SerializeField] private BuildingHealthModel buildingHealthModel = default;
        [SerializeField] private float healthMax = 100f;
        [SerializeField] private bool setHealthMaxOnEnable = true;

        private void OnEnable() 
        {
            if(setHealthMaxOnEnable)
            {
                SetHealthMax();
            }
        }
    
        public void SetHealthMax()
        {
            buildingHealthModel.SetHealthMax(healthMax);
        }
    }
}
