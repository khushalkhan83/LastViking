using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weather
{
    [ExecuteInEditMode]
    public class TerrainControl : MonoBehaviour
    {
        [SerializeField] private Terrain terrain;
        [SerializeField] private Color wavingGrassTint;
        
        void Update()
        {
            if(terrain == null) return;

            terrain.terrainData.wavingGrassTint = wavingGrassTint;
        }
    }
}

