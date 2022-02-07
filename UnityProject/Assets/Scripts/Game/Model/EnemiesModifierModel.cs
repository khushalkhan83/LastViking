using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace Game.Models
{
    public class EnemiesModifierModel : MonoBehaviour
    {
        [Serializable]
        public class Data
        {
            [Range(0,1)]
            [SerializeField] private float healthScaler = 1;
            [Range(0,1)]
            [SerializeField] private float damageScaler = 1;
            [Range(0,1)]
            [SerializeField] private float speedScaler = 1;

            public float HealthScaler => healthScaler;
            public float DamageScaler => damageScaler;
            public float SpeedScaler => speedScaler;
        }

        public enum Preset {FirstDay, SecondDay, ThirdDay, Default}


        #region Data
        #pragma warning disable 0649
        [SerializeField] private List<Data> presets;
        
        #pragma warning restore 0649
        #endregion

        private Data modifier = new Data();

        public float HealthScaler => modifier.HealthScaler;
        public float DamageScaler => modifier.DamageScaler;
        public float SpeedScaler => modifier.SpeedScaler;
        
        public void ApplyPresetPerDay(int day)
        {
            // presets.
            // presets.
            if(presets.IndexOutOfRange(day)) return;

            modifier = presets[day];
        }
    }
}
