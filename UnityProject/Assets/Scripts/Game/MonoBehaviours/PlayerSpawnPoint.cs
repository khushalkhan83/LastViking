using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
    private void Start()
    {
        PlayerEventHandler.transform.position = this.transform.position;
    }
}
