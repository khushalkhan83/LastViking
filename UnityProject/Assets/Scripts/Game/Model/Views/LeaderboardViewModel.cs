using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Game.Models
{
    public class LeaderboardViewModel : MonoBehaviour
    {

        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredInt _numFirstPartPlayers = 5;
        [SerializeField] private ObscuredInt _numSecondPartPlayers = 2;

        [SerializeField] private ObscuredFloat _accumulatingTime = 1f;
        [SerializeField] private ObscuredFloat _initAccumulatingDelay = 1f;
        [SerializeField] private ObscuredFloat _accumulatingDelay = 0.5f;

        [SerializeField] private ObscuredFloat _targetElementsScale = 1.25f;

#pragma warning restore 0649
        #endregion

        public int NumFirstPartPlayers => _numFirstPartPlayers;
        public int NumSecondPartPlayers => _numSecondPartPlayers;

        public float AccumulatingTime => _accumulatingTime;
        public float AccumulatingDelay => _accumulatingDelay;
        public float InitAccumulatingDelay => _initAccumulatingDelay;

        public float TargetElementsScale => _targetElementsScale;

        public bool HasUpdatedAfterLogin { set; get; } = false;
    }
}
