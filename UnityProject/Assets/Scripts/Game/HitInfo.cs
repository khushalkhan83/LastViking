using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AI
{
    public class HitInfo : MonoBehaviour
    {
        #region Data

        private Vector3 _hitPoint;
        private Vector3 _hitDirection;
        private float _kickback;

        #endregion

        public Vector3 HitPoint => _hitPoint;
        public Vector3 HitDirection => _hitDirection;
        public float Kickback => _kickback;

        public void SetHitPoint(Vector3 point)
        {
            _hitPoint = point;
        }

        public void SetHitDirection(Vector3 direction)
        {
            _hitDirection = direction;
        }

        public void SetyKickback(float kickback)
        {
            _kickback = kickback;
        }
    }
}

