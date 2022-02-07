using UnityEngine;

namespace Encounters
{
    [CreateAssetMenu(fileName = "SO_config_locationEncounters_new", menuName = "Configs/Encounters/LocationEncounteres", order = 0)]
    public class LocationEncountersConfig : ScriptableObject
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private float _baseChance;
        [SerializeField] private float _chanceStep;
        #pragma warning restore 0649
        #endregion

        public float baseChance => _baseChance;
        public float chanceStep => _chanceStep;


        #region ScriptableObject
        private void OnValidate()
        {
            if(_baseChance < 0) _baseChance = 0;

            if(_baseChance > 1) _baseChance = 1;

            if(_chanceStep < 0) _chanceStep = 0;

            if(_chanceStep > 1) _chanceStep = 1;
        }
        #endregion
    }
}