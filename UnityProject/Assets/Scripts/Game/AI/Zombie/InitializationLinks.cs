using Game.StateMachine.Parametrs;
using UnityEngine;

namespace Game.AI.Behaviours.Zombie
{
    public class InitializationLinks : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private TransformProvider _targetBase;

#pragma warning restore 0649
        #endregion

        public TransformProvider TargetBase => _targetBase;
    }
}
