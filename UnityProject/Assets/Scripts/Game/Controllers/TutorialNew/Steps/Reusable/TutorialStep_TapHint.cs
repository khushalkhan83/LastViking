using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_TapHint : TutorialStepBase
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public DragItemViewModel DragItemViewModel { get; private set; }
        [Inject] public ViewsStateModel ViewsStateModel { get; private set; }

        private GameObject raycastGameObject = null;
        private Func<GameObject, bool> showTapCondition = null;

        public TutorialStep_TapHint(TutorialEvent StepStartedEvent, Func<GameObject, bool> ShowTapCondition, Action OnCompleatedAsSubStep): base(StepStartedEvent,OnCompleatedAsSubStep)
        {
            showTapCondition = ShowTapCondition;
        }

        public override void OnStart()
        {
            PlayerEventHandler.RaycastData.OnChange += OnRaycastDataChaged;
            ViewsStateModel.OnOpenPopupsChanged += UpdateTapHintView;
            ViewsStateModel.OnOpenWindowsChanged += UpdateTapHintView;
        }
        public override void OnEnd()
        {
            PlayerEventHandler.RaycastData.OnChange -= OnRaycastDataChaged;
            ViewsStateModel.OnOpenPopupsChanged -= UpdateTapHintView;
            ViewsStateModel.OnOpenWindowsChanged -= UpdateTapHintView;
            DragItemViewModel.SetShow(false);
        }


        private void OnRaycastDataChaged()
        {
            var newRaycastGameObject = PlayerEventHandler.RaycastData.Value?.GameObject;
            if(raycastGameObject != newRaycastGameObject)
            {
                raycastGameObject = newRaycastGameObject;
                UpdateTapHintView();
            }
        }

        private void UpdateTapHintView()
        {
            if(showTapCondition != null && !ViewsStateModel.WindowOrPopupOpened() && showTapCondition.Invoke(raycastGameObject))
            {
                DragItemViewModel.SetClickData();
                DragItemViewModel.SetShow(true);
            }
            else
            {
                DragItemViewModel.SetShow(false);
            }
        }
    }
}
