using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

public class PlayerParticlesEffectTrigger : MonoBehaviour
{   
    [SerializeField] private ParticleSystem particles = default;

    private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
    private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;

    private bool isParticlesActive = false;

    private void OnEnable() 
    {
        GameUpdateModel.OnUpdate += OnUpdate;
    }

    private void OnDisable() 
    {
        GameUpdateModel.OnUpdate -= OnUpdate;
        DeactivateParticles();    
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.GetComponent<PlayerEventHandler>() != null)
        {
            ActivateParticles();
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.GetComponent<PlayerEventHandler>() != null)
        {
            DeactivateParticles();
        }
    }


    private void ActivateParticles()
    {
        isParticlesActive = true;
        particles.Play();
    }

    private void DeactivateParticles()
    {
        isParticlesActive = false;
        particles.Stop();
    }

    private void OnUpdate()
    {
        if(isParticlesActive)
        {
            particles.transform.position = PlayerEventHandler.transform.position;
        }
    }
}
