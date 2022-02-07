using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Interactables
{
    public class AnimationActivatable : Activatable
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Animator _animator;
        [SerializeField] private string _triggerName;

#pragma warning restore 0649
        #endregion

        public Animator Animator => _animator;
        public int TriggerHash => Animator.StringToHash(_triggerName);

        public override void OnActivate() => Animator.SetTrigger(TriggerHash);
    }
}