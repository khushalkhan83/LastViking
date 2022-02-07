using Extensions;
using NaughtyAttributes;
using UnityEngine;

namespace Encounters
{
    [CreateAssetMenu(fileName = "SO_config_timeEncounter_new", menuName = "Configs/Encounters/TimeEncountersConfig", order = 0)]
    public class TimeEncounterConfig : ScriptableObject
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private float _firstDiceRollTime = 60;
        [SerializeField] private float _diceRerollTime = 120;
        [SerializeField] private float _baseChance = 0.5f;
        [SerializeField] private float _chanceStep = 0.25f;
#pragma warning restore 0649
        #endregion

        public float firstDiceRollTime => _firstDiceRollTime;
        public float diceRerollTime => _diceRerollTime;

        public float baseChance => _baseChance;
        public float chanceStep => _chanceStep;

        [Button]
        void PrintTimeToConsole()
        {
            Debug.Log($"_firstDiceRollTime: {_firstDiceRollTime.GetFormatedSeconds()}");
            Debug.Log($"_diceRerollTime: {_diceRerollTime.GetFormatedSeconds()}");
        }

        #region ScriptableObject
        private void OnValidate()
        {
            if (_firstDiceRollTime < 0) _firstDiceRollTime = 0;

            if (_diceRerollTime < 0) _diceRerollTime = 0;

            if (_baseChance < 0) _baseChance = 0;

            if (_baseChance > 1) _baseChance = 1;

            if (_chanceStep < 0) _chanceStep = 0;

            if (_chanceStep > 1) _chanceStep = 1;
        }
        #endregion
    }
}