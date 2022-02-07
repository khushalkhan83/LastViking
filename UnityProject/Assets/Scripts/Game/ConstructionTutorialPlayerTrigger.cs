using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

public class ConstructionTutorialPlayerTrigger : MonoBehaviour
{
    [SerializeField] private Collider triggerCollider;

    private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;
    private TutorialModel TutorialModel => ModelsSystem.Instance._tutorialModel;
    private OpenConstructionTutorialModel OpenConstructionTutorialModel => ModelsSystem.Instance._openConstructionTutorialModel;
    private PlayerDeathModel PlayerDeathModel => ModelsSystem.Instance._playerDeathModel;
    
    #region MonoBehaviour
    private void OnEnable() 
    {
        PlayerScenesModel.OnEnvironmentLoaded += OnEnvironmentLoaded;
        TutorialModel.OnNextStep += OnNextStep;
        PlayerDeathModel.OnRevival += OnRevival;
        PlayerDeathModel.OnRevivalPrelim += OnRevivalPrelim;
    }

    private void OnDisable() 
    {
        PlayerScenesModel.OnEnvironmentLoaded -= OnEnvironmentLoaded;
        TutorialModel.OnNextStep -= OnNextStep;
        PlayerDeathModel.OnRevival -= OnRevival;
        PlayerDeathModel.OnRevivalPrelim += OnRevivalPrelim;
    }
    private void OnTriggerEnter(Collider other) 
    {
        bool isPlayer = other.GetComponent<PlayerEventHandler>();
        if(isPlayer)
        {
            triggerCollider.enabled = false;
            OpenConstructionTutorialModel.PlayerEnterConstructionZone();
        }
    }
    #endregion

    private void OnEnvironmentLoaded() => ResetTrigger();

    private void OnNextStep() => ResetTrigger();

    private void OnRevival() => ResetTrigger();

    private void OnRevivalPrelim() => ResetTrigger();

    private void ResetTrigger()
    {
        triggerCollider.enabled = false;
        triggerCollider.enabled = true;
    }
}
