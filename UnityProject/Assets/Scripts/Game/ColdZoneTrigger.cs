using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UnityEngine;

public class ColdZoneTrigger : MonoBehaviour
{
    [SerializeField] private ZoneType type = default;

    public ZoneType Type => type;
    private PlayerWarmModel PlayerWarmModel => ModelsSystem.Instance._playerWarmModel;

    private void OnTriggerStay(Collider other) 
    {
        if(type == ZoneType.Cold)
        {
            PlayerWarmModel.SetInColdZone();
        }
        else if(type == ZoneType.Warm)
        {
            PlayerWarmModel.SetInWarmZone();
        }
    }

    public enum ZoneType
    {
        Cold,
        Warm,
    }
}


