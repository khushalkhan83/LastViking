using Game.Models;
using Game.StateMachine.Conditions;
using UnityEngine;

namespace Game.StateMachine.Behaviours
{
    public class ChangeCollider : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private IsTrueBoolParametrCondition _is2Legs;
        [SerializeField] private SphereCollider _sphereCollider;
        [SerializeField] private float _size2Legs;
        [SerializeField] private float _zPos2Legs;
        [SerializeField] private float _size4Legs;
        [SerializeField] private float _zPos4Legs;
        [SerializeField] private float _changeSpeed;

#pragma warning restore 0649
        #endregion

        public IsTrueBoolParametrCondition Is2Legs => _is2Legs;
        private SphereCollider SphereCollider => _sphereCollider;
        public float Size2Legs => _size2Legs;
        public float ZPos2Legs => _zPos2Legs;
        public float Size4Legs => _size4Legs;
        public float ZPos4Legs => _zPos4Legs;
        public float ChangeSpeed => _changeSpeed;

        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;

        public float TargetSize { get; private set; }
        public float TargetZPos { get; private set; }

        private void OnEnable()
        {
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        private void OnDisable()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void OnUpdate()
        {
            TargetSize = Is2Legs.IsTrue ? Size2Legs : Size4Legs;
            TargetZPos = Is2Legs.IsTrue ? ZPos2Legs : ZPos4Legs;

            SphereCollider.radius = Mathf.MoveTowards(SphereCollider.radius, TargetSize, _changeSpeed * Time.deltaTime);

            var center = SphereCollider.center;
            center.z = Mathf.MoveTowards(center.z, TargetZPos, _changeSpeed * Time.deltaTime);
            SphereCollider.center = center;
        }
    }
}
