using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weather
{
    [ExecuteInEditMode]
    public class LightSettingsControl : MonoBehaviour
    {
        [SerializeField] private bool active;

        [Header("Properties")]
        [SerializeField] private bool updateSkybox;
        [SerializeField] private Material skyBox;

        [Space]
        [SerializeField] private  bool fogActive;
        [SerializeField] private Color fogColor;
        [SerializeField] private float fogDensity;

        private Material activeSkybox;
        private void Awake()
        {
            activeSkybox = skyBox;
        }

        void Update()
        {
            if(!active) return;

            RenderSettings.fog = fogActive;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = fogDensity;


            if(!updateSkybox) return;
            if(!SkyBoxChanged()) return;
            RenderSettings.skybox = activeSkybox;
        }

        private bool SkyBoxChanged()
        {
            bool changed = activeSkybox != skyBox;
            if(changed)
            {
                activeSkybox = skyBox;
            }
            return changed;
        }
    }
}