using System.Collections;
using System.Collections.Generic;
using Core;
using Game.Interactables;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

public class TeleportPlayerToInitPoint : Activatable
{
    [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
    [Inject] public PlayerRespawnPoints PlayerRespawnPoints { get; private set; }

    private void Awake()
    {
        var injector = FindObjectOfType<InjectionSystem>();
        injector.Inject(this);
    }

    public override void OnActivate()
    {
        if(PlayerEventHandler == null) return;

        PlayerEventHandler.transform.position = PlayerRespawnPoints.GetRandomRespawnPoint().position;
        var characterController = PlayerEventHandler.GetComponent<CharacterController>();

        if(characterController == null) return;

        PlayerEventHandler.Velocity.Set(Vector3.zero);
        characterController.Move(Vector3.zero);
    }
}
