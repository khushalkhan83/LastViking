using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UltimateSurvival;
using UnityEngine;


public class TutorialHousePlayerTrigger : MonoBehaviour
{
    [SerializeField] private Collider triggerCollider;

    private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;
    private TutorialModel TutorialModel => ModelsSystem.Instance._tutorialModel;
    private TutorialHouseModel TutorialHouseModel => ModelsSystem.Instance._tutorialHouseModel;
    private PlayerDeathModel PlayerDeathModel => ModelsSystem.Instance._playerDeathModel;

    private void OnEnable() 
    {
        if(TutorialModel.IsComplete)
        {
            triggerCollider.enabled = false;
        }
        else
        {
            PlayerScenesModel.OnEnvironmentLoaded += OnEnvironmentLoaded;
            TutorialModel.OnNextStep += OnNextStep;
            TutorialModel.OnComplete += OnTutorialComplete;
            PlayerDeathModel.OnRevival += OnRevival;
            PlayerDeathModel.OnRevivalPrelim += OnRevivalPrelim;
        }
    }

    private void OnDisable() 
    {
        PlayerScenesModel.OnEnvironmentLoaded -= OnEnvironmentLoaded;
        TutorialModel.OnNextStep -= OnNextStep;
        TutorialModel.OnComplete -= OnTutorialComplete;
        PlayerDeathModel.OnRevival -= OnRevival;
        PlayerDeathModel.OnRevivalPrelim -= OnRevivalPrelim;
    }

    private void OnEnvironmentLoaded() => ResetTrigger();

    private void OnNextStep() => ResetTrigger();

    private void OnTutorialComplete()
    {
        triggerCollider.enabled = false;
        PlayerScenesModel.OnEnvironmentLoaded -= OnEnvironmentLoaded;
        TutorialModel.OnNextStep -= OnNextStep;
        TutorialModel.OnComplete -= OnTutorialComplete;
        PlayerDeathModel.OnRevival -= OnRevival;
        PlayerDeathModel.OnRevivalPrelim -= OnRevivalPrelim;
    }

    private void OnTriggerEnter(Collider other) 
    {
        bool isPlayer = other.GetComponent<PlayerEventHandler>();
        if(isPlayer)
        {
            triggerCollider.enabled = false;
            TutorialHouseModel.PlayerEnterConstructionZone();
        }
    }

    private void OnRevival() => ResetTrigger();

    private void OnRevivalPrelim() => ResetTrigger();

    private void ResetTrigger()
    {
        triggerCollider.enabled = false;
        triggerCollider.enabled = true;
    }
}
