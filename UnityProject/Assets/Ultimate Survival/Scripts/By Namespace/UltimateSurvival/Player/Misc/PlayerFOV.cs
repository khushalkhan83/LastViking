using System;
using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UnityEngine;

public class PlayerFOV : MonoBehaviour
{

    #region Data
#pragma warning disable 0649

    [SerializeField] private Transform _eyes;
    [SerializeField] private Camera _camera;
 
#pragma warning restore 0649
    #endregion

    private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;

    private void OnEnable() {
        PlayerScenesModel.OnEnvironmentChange += OnEnvironmentLoadHandler;
    }

    private void OnDisable() {
        PlayerScenesModel.OnEnvironmentChange -= OnEnvironmentLoadHandler;
    }

    private void OnEnvironmentLoadHandler()
    {
        switch (PlayerScenesModel.ActiveEnvironmentSceneID)
        {
            case EnvironmentSceneID.Waterfall:
                _camera.farClipPlane = 200;
                break;
            default:
                _camera.farClipPlane = 125;
                break;
        }
    }
}
