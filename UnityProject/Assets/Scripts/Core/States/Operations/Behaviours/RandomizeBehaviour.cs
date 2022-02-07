using UnityEngine;

namespace Core.StateMachine.Operations.Behaviours
{
    public class RandomizeBehaviour : BehaviourBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private BehaviourBase[] _behaviours;

#pragma warning restore 0649
        #endregion

        public BehaviourBase[] Behaviours => _behaviours;

        public BehaviourBase Current { get; private set; }

        public override void Begin()
        {
            Current = Behaviours[Random.Range(0, Behaviours.Length)];
            Current.Begin();

            Current.OnEnd += OnEndHandler;
        }

        public override void ForceEnd()
        {
            Current.OnEnd -= OnEndHandler;
            Current.ForceEnd();
        }

        public override void Refresh()
        {
            Current.Refresh();
        }

        private void OnEndHandler(BehaviourBase behaviour)
        {
            Current.OnEnd -= OnEndHandler;
            End();
        }
    }
}
