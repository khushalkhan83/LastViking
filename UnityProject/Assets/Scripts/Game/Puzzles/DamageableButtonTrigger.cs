using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game.Puzzles
{
    public class DamageableButtonTrigger : DamageableTrigger
    {
        [SerializeField] private Transform buttonTransform = default;
        [SerializeField] private Vector3 buttonActivePosition = default;
        [SerializeField] private Vector3 buttonInactivePosition = default;
        [SerializeField] private float animationTime = 0.5f;

        protected override void UpdateTriggerView()
        {
            DOTween.Kill(buttonTransform);
            Vector3 endValue = isActive? buttonActivePosition : buttonInactivePosition;
            buttonTransform.DOLocalMove(endValue, animationTime);
        }
    }
}
