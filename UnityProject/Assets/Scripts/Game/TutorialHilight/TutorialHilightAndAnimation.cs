using System;
using System.Collections;
using Game.Models;
using NaughtyAttributes;
using UnityEngine;

namespace Game.Components
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CanvasGroup))]
    public class TutorialHilightAndAnimation : TutorialHilight
    {
        #region Depdendencies
        private CoroutineModel CoroutineModel => ModelsSystem.Instance._coroutineModel;
        private TutorialHilightModel Model => ModelsSystem.Instance._tutorialHilightModel;
        #endregion
        
        private Animator animator;

        protected override void Init()
        {
            base.Init();
            animator = GetComponent<Animator>();
        }

        [Button]
        protected override void ApplyEffect()
        {
            base.ApplyEffect();
            animator.runtimeAnimatorController = Model.AnimatorController;
            animator.enabled = true;
            animator.Play(Model.AnimationClipName);
        }

        [Button]
        protected override void RemoveEffect()
        {
            base.RemoveEffect();
            animator.Play(Model.NoAnimationClipName);
            DoActionAfterFrame(() => animator.enabled = false);
        }

        private void DoActionAfterFrame(Action action) => CoroutineModel.InitCoroutine(CDoActionAfterFrame(action));

        // TODO: Code duplicate here and in other classes. Move to CoroutineModel
        private IEnumerator CDoActionAfterFrame(Action action)
        {
            yield return null;
            action?.Invoke();
        }
    }
}