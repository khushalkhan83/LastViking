using System;
using System.Collections;
using EnemiesAttack;
using Game.Models;
using UnityEngine;

namespace Game.QuestSystem.Map.Extra
{
    public class ShelterAttack : MonoBehaviour
    {
        private ShelterAttackModeModel ShelterAttackModeModel => ModelsSystem.Instance._shelterAttackModeModel;
        private ApplicationCallbacksModel ApplicationCallbacksModel => ModelsSystem.Instance._applicationCallbacksModel;
        
        private void OnEnable()
        {
            ShelterAttackModeModel.SetAttackModeAvaliable(true);
        }

        private void OnDisable()
        {
            // call shelter attack disable only when state is changed. not when application is quiting
            if(ApplicationCallbacksModel.IsApplicationQuitting) return;
            
            ShelterAttackModeModel.SetAttackModeAvaliable(false);
        }
    }
}