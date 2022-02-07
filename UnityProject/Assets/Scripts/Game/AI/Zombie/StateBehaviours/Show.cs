using Core.StateMachine;
using UnityEngine;

namespace Game.AI.Behaviours.Zombie
{
    public class Show : BehaviourBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private float _duration;

#pragma warning restore 0649
        #endregion

        public float Duration => _duration;

        public float TimeToEnd { get; private set; }

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
