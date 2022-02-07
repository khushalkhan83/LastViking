using System;
using System.Collections;
using CustomeEditorTools;
using EnemiesAttack;
using Game.Models;
using UnityEngine;

namespace Game.QuestSystem.Map.Extra
{
    public class ChangeLocation : MonoBehaviour
    {
        [SerializeField] private EnvironmentSceneID sceneID;
        [SerializeField] private EnvironmentTransition destination;
        private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;
        
        public void Change()
        {
            PlayerScenesModel.TransitionToEnvironment(sceneID,destination);
        }
    }
}