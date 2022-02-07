using UnityEngine;
using Game.Models;
using System;

namespace Game.Controllers
{
    public class VillageCinematics : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private GameObject hallUpgradeCutscene;
        #pragma warning restore 0649
        #endregion


        private VillageCinematicsModel VillageCinematicsModel {get => ModelsSystem.Instance._villageCinematicsModel;}

        #region MonoBehaviour
        private void OnEnable()
        {
            VillageCinematicsModel.OnHallCinematicStart += OnHallCinematicStart;
        }

        private void OnDisable()
        {
            VillageCinematicsModel.OnHallCinematicStart -= OnHallCinematicStart;
        }
        #endregion

        private void OnHallCinematicStart()
        {
            if(!hallUpgradeCutscene.activeSelf)
                hallUpgradeCutscene.SetActive(true);
        }

        // triggered outside
        public void OnHallCinematicEnd()
        {
            hallUpgradeCutscene.SetActive(false);
        }
    }
}