using UnityEngine;

namespace Core.StateMachine
{
    public class State : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private bool _isLockInterruptTransition;
        [SerializeField] private BehaviourBase _behaviours;
        [SerializeField] private Interrupt[] _interrupts;
        [SerializeField] private Interrupt[] _next;

#pragma warning restore 0649
        #endregion
        public bool IsLockInterruptTransition => _isLockInterruptTransition;
        public Interrupt[] Interrupts => _interrupts;
        public Interrupt[] Next => _next;
        public BehaviourBase Behaviour => _behaviours;

        public void SetNext(params Interrupt[] interrupts) => _next = interrupts;
    }
}
