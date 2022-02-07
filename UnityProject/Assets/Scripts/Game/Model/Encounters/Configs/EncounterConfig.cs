using Extensions;
using NaughtyAttributes;
using UnityEngine;

namespace Encounters
{
    [CreateAssetMenu(fileName = "SO_config_encounter_new", menuName = "Configs/Encounters/EncounterConfig", order = 0)]
    public class EncounterConfig : ScriptableObject
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private float _resetTime;
        [SerializeField] private float _chanceWeight;

        [Range(0,10)]
        [SerializeField] private int _requiaredStoryChapter;
        #pragma warning restore 0649
        #endregion

        public float resetTime => _resetTime;
        public float chanceWeight => _chanceWeight;
        public int requiaredStoryChapter => _requiaredStoryChapter;

        [Button] void PrintTimeToConsole()
        {
            Debug.Log($"reset time: {_resetTime.GetFormatedSeconds()}");
        }

        #region ScriptableObject
        private void OnValidate()
        {
            if(_chanceWeight < 0) _chanceWeight = 0;

            if(_resetTime < 0) _resetTime = 0;
        }
        #endregion
    }
}