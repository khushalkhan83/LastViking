using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.ThirdPerson
{
    public class AttackAnimationCallbacks : MonoBehaviour
    {
        public UnityEvent onMeleeAttackStart;
        public UnityEvent onMeleeAttack;
        public UnityEvent onMeleeAttackEnd;


        public void MeleeAttackStart(int throwing = 0)
        {
            onMeleeAttackStart?.Invoke();
        }

        public void MeleeAttack()
        {
            onMeleeAttack?.Invoke();
        }

        public void MeleeAttackEnd()
        {
            onMeleeAttackEnd?.Invoke();
        }
    }
}
