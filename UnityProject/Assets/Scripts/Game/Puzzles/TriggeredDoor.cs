using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


namespace Game.Puzzles
{
    public class TriggeredDoor : TriggeredMechanismBase
    {
        [SerializeField] private Transform doorTransform = default;
        [SerializeField] private Vector3 doorOpenedPosition = default;
        [SerializeField] private Vector3 doorClosedPosition = default;
        [SerializeField] private float animationTime = 0.5f;
        [SerializeField] private bool activated;

        public override bool Activated 
        { 
            get => activated; 
            protected set => activated = value;
        }

        protected override void UpdateMechanismView()
        {
            DOTween.Kill(doorTransform);
            Vector3 endValue = Activated? doorOpenedPosition : doorClosedPosition;
            doorTransform.DOLocalMove(endValue, animationTime);
        }
    }
}
