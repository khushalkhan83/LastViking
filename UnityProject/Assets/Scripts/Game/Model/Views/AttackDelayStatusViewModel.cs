using System;
using UnityEngine;

namespace Game.Models
{
    public class AttackDelayStatusViewModel : MonoBehaviour
    {
        public event Action OnStartWaitAttack;
        public event Action OnEndWaitAttack;

        public float TargetTime { set; get; }

        public void StartWaitAttack() => OnStartWaitAttack?.Invoke();
        public void EndWaitAttack() => OnEndWaitAttack?.Invoke();
    }
}
