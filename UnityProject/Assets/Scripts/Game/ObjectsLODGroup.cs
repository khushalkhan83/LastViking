using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

public class ObjectsLODGroup : MonoBehaviour
{
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private GameObject[] objects;
    [SerializeField] private Renderer[] renderers;

    private float sqrMaxDistance;
    private float sqrDistance;
    private bool objectsEnabled = true;

    private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
    private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;

    private void OnEnable() 
    {
        sqrMaxDistance = maxDistance * maxDistance;
        GameUpdateModel.OnUpdate += OnUpdate;
    }

    private void OnDisable() 
    {
        GameUpdateModel.OnUpdate -= OnUpdate;
    }

    private void OnUpdate()
    {
        sqrDistance = (transform.position - PlayerEventHandler.transform.position).sqrMagnitude;
        if(sqrDistance > sqrMaxDistance)
        {
            if(objectsEnabled)
            {
                foreach(var obj in objects)
                {
                    obj.SetActive(false);
                }

                foreach(var rend in renderers)
                {
                    rend.enabled = false;
                }

                objectsEnabled = false;
            }
        }
        else
        {
            if(!objectsEnabled)
            {
                foreach(var obj in objects)
                {
                    obj.SetActive(true);
                }

                foreach(var rend in renderers)
                {
                    rend.enabled = true;
                }

                objectsEnabled = true;
            }
        }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        var oldColor = UnityEditor.Handles.color;
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, maxDistance);
        UnityEditor.Handles.color = oldColor;
    }
    #endif

}
