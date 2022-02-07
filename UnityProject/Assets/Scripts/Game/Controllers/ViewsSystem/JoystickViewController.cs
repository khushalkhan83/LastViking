using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Controllers
{
    public class JoystickViewController : ViewControllerBase<JoystickView>
    {
        [Inject] public GameLateUpdateModel GameLateUpdateModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public JoystickModel JoystickModel { get; private set; }

        protected bool IsDownCurrent { get; private set; }
        protected int TouchId { get; private set; }

        protected override void Show()
        {
            View.transform.SetSiblingIndex(1);
            View.OnDownEvent += OnDownHandler;
            View.OnUpEvent += OnUpHandler;
            View.OnEnterEvent += OnEnterHandler;
            View.OnDragEvent += OnDragEventHandler;
            GameUpdateModel.OnUpdate += OnUpdateHandler;
            GameLateUpdateModel.OnLaterUpdate += OnLateUpdateHandler;
        }

        protected override void Hide()
        {
            View.OnDownEvent -= OnDownHandler;
            View.OnUpEvent -= OnUpHandler;
            View.OnEnterEvent -= OnEnterHandler;
            View.OnDragEvent -= OnDragEventHandler;
            GameUpdateModel.OnUpdate -= OnUpdateHandler;
            GameLateUpdateModel.OnLaterUpdate -= OnLateUpdateHandler;

            IsDownCurrent = false;
            TouchId = -1;
        }

        private void OnUpdateHandler()
        {
            View.Joystick.position = Vector3.Slerp(View.Joystick.position, View.Pivot.position + (Vector3)JoystickModel.Axes, JoystickModel.SmoothJoystick);
        }

        private void OnLateUpdateHandler()
        {
            if (!IsDownCurrent)
            {
                JoystickModel.SetAxes(Vector2.Lerp(JoystickModel.Axes, Vector2.zero, Mathf.Clamp01(Time.smoothDeltaTime * JoystickModel.SmoothAxes)));
            }

            PlayerEventHandler.MovementInput.Set(JoystickModel.Axes / JoystickModel.Radius);
        }

        private void OnDragEventHandler(PointerEventData eventData)
        {
            if (TouchId == eventData.pointerId)
            {
                UpdateAxes(eventData);
            }
        }

        private void OnEnterHandler(PointerEventData eventData)
        {
            if (IsDownCurrent && TouchId == eventData.pointerId)
            {
                eventData.pointerDrag = View.gameObject;
                eventData.pointerPress = View.gameObject;
            }
        }

        private void OnUpHandler(PointerEventData eventData)
        {
            if (TouchId == eventData.pointerId)
            {
                IsDownCurrent = false;
            }
        }

        private void OnApplicationFocus(bool focus) 
        { 
            if(!focus) IsDownCurrent = false;
        }

        private void OnApplicationPause(bool pause) 
        {
            if (pause) IsDownCurrent = false;
        }

        private void OnDownHandler(PointerEventData eventData)
        {
            if (!IsDownCurrent)
            {
                IsDownCurrent = true;
                TouchId = eventData.pointerId;

                UpdateAxes(eventData);
                JoystickModel.Interact();
            }
        }

        private void UpdateAxes(PointerEventData eventData)
        {
            if (eventData.pressEventCamera)
            {
                JoystickModel.SetAxes(Vector2.ClampMagnitude(eventData.pressEventCamera.ScreenToWorldPoint(eventData.position) - View.Pivot.position, JoystickModel.Radius));
            }
        }
    }
}
