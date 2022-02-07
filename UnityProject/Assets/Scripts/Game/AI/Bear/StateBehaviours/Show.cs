using CodeStage.AntiCheat.ObscuredTypes;
using Core.StateMachine;
using UnityEngine;

namespace Game.AI.Behaviours.Bear
{
    public class Show : BehaviourBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredFloat _duration;

#pragma warning restore 0649
        #endregion

        public ObscuredFloat Duration => _duration;

        public ObscuredFloat TimeToEnd { get; private set; }

        public override void Begin()
        {
            TimeToEnd = Duration;
        }

        public override void ForceEnd()
        {
            TimeToEnd = 0;
        }

        public override void Refresh()
        {
            TimeToEnd -= Time.deltaTime;

            if (TimeToEnd <= 0)
            {
                End();
            }
        }
    }
}
