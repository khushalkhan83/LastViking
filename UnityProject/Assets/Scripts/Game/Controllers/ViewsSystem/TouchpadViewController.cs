using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using System;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Controllers
{
    public class TouchpadViewController : ViewControllerBase<TouchpadView>
    {
        [Inject] public PlayerMovementModel PlayerMovementModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public TouchpadModel TouchpadModel { get; private set; }
        [Inject] public WorldCameraModel WorldCameraModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public GameLateUpdateModel GameLateUpdateModel { get; private set; }

        protected bool IsContinuoslyAttackLastAction { get; private set; }
        protected bool IsDownCurrent { get; private set; }
        protected bool IsOneAttackLastAction { get; private set; }
        protected int TouchId { get; private set; }
        protected float TouchAccumutaleDistance { get; private set; }
        protected Vector2 TouchPositionUp { get; private set; }
        protected Vector2 TouchPositionDragLast { get; private set; }

        private Vector2 baseResolotion = new Vector2(1920, 1080);

        protected override void Show()
        {
            TouchId = -100;
            IsDownCurrent = false;
            GameUpdateModel.OnUpdate += OnUpdate;
            GameLateUpdateModel.OnLaterUpdate += OnLateUpdate;
            View.transform.SetSiblingIndex(0);
            View.OnDownEvent += OnDownHandler;
            View.OnUpEvent += OnUpHandler;
            View.OnEnterEvent += OnEnterHandler;

            PlayerMovementModel.OnChangeMovementID += OnChangePlayerMovement;
            OnChangePlayerMovement();
        }

        protected override void Hide()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
            GameLateUpdateModel.OnLaterUpdate -= OnLateUpdate;
            View.OnDownEvent -= OnDownHandler;
            View.OnUpEvent -= OnUpHandler;
            View.OnEnterEvent -= OnEnterHandler;

            PlayerMovementModel.OnChangeMovementID -= OnChangePlayerMovement;
        }

        private void OnUpdate() {
#if UNITY_EDITOR
            if (TouchId != -100 && Input.GetMouseButton(0))
            {
                Debug.LogError("RES: "+baseResolotion);
                Vector2 delta = AdjustDeltaToResolution((Vector2)Input.mousePosition - TouchPositionDragLast);
                TouchpadModel.SetAxes(delta * TouchpadModel.Sensivity * Time.timeScale);
                OnDragTouchPadViewHandler(Input.mousePosition);
            }
#else
            if (TouchId != -100 && Input.touches.Length > 0) {
                foreach (var touch in Input.touches) {
                    if (TouchId == touch.fingerId) {
                        Vector2 delta = AdjustDeltaToResolution(touch.deltaPosition);
                        TouchpadModel.SetAxes(delta * TouchpadModel.Sensivity * Time.timeScale);
                        OnDragTouchPadViewHandler(touch.position);
                        break;
                    }
                }
            }
#endif
        }
        private void OnLateUpdate()
        {
            PlayerRotation(TouchpadModel.Axes);

            if (IsContinuoslyAttackLastAction) //wait end of attack and check is can continue
            {
                AttackRepeat();
            }
            TouchpadModel.SetAxes(Vector2.Lerp(TouchpadModel.Axes, Vector2.zero, Mathf.Clamp01(Time.smoothDeltaTime * TouchpadModel.Smooth)));
        }

        private void OnEnterHandler(PointerEventData eventData)
        {
            if (IsDownCurrent && TouchId == eventData.pointerId)
            {
                eventData.pointerDrag = View.gameObject;
                eventData.pointerPress = View.gameObject;
            }
        }

        private void OnDownHandler(PointerEventData eventData)
        {
            if (!IsDownCurrent)
            {
                IsDownCurrent = true;
                TouchId = eventData.pointerId;

                OnDownTouchPadViewHandler(eventData);
            }
        }

        private void OnUpHandler(PointerEventData eventData)
        {
            if (TouchId == eventData.pointerId)
            {
                IsDownCurrent = false;
                TouchId = -100;

                OnUpTouchPadViewHandler(eventData);
            }
        }

        public void PlayerRotation(Vector2 axes)
        {
            axes.x = (float)Math.Round(axes.x, TouchpadModel.SmoothAccuracy);
            axes.y = (float)Math.Round(axes.y, TouchpadModel.SmoothAccuracy);

            PlayerEventHandler.transform.Rotate(0f, axes.x, 0f);

            var cameraTransform = WorldCameraModel.WorldCamera.transform;
            var rotation = cameraTransform.localRotation.eulerAngles;
            var angle = (rotation.x + 180) % 360;

            var isCanRotation =
                (
                    axes.y > 0
                    && angle > TouchpadModel.AngleVerticalLookUp
                )
                ||
                (
                    axes.y < 0
                    && angle < TouchpadModel.AngleVerticalLookDown
                );

            if (isCanRotation)
            {
                cameraTransform.localEulerAngles = (Quaternion.Euler(-axes.y, 0, 0) * cameraTransform.localRotation).eulerAngles;
            }
        }

        private void OnDragTouchPadViewHandler(Vector2 position)
        {
            TouchAccumutaleDistance += Vector2.Distance(position, TouchPositionDragLast);
            TouchPositionDragLast = position;
        }

        private void OnDownTouchPadViewHandler(PointerEventData eventData)
        {
            if (Vector2.Distance(TouchPositionUp, eventData.position) <= TouchpadModel.TouchDeltaAttackContinuosly && IsTouchInHoldRect(eventData))
            {
                if (IsOneAttackLastAction)
                {
                    AttackRepeat();
                }

                IsContinuoslyAttackLastAction = IsOneAttackLastAction;
            }

            TouchAccumutaleDistance = 0;
            TouchPositionDragLast = eventData.position;
        }

        private void AttackRepeat()
        {
            if (!PlayerEventHandler.AttackContinuously.Try())
            {
                PlayerEventHandler.AttackOnce.Try();
            }
        }

        private void OnUpTouchPadViewHandler(PointerEventData eventData)
        {
            if (!IsContinuoslyAttackLastAction && TouchAccumutaleDistance <= TouchpadModel.TouchDeltaAttack && IsTouchInHoldRect(eventData))
            {
                PlayerEventHandler.AttackOnce.Try();
                IsOneAttackLastAction = true;
            }
            else
            {
                IsOneAttackLastAction = false;
            }

            TouchPositionUp = eventData.position;
            IsContinuoslyAttackLastAction = false;
        }

        private bool IsTouchInHoldRect(PointerEventData eventData) => RectTransformUtility.RectangleContainsScreenPoint(View.HoldDownRect, eventData.position, eventData.pressEventCamera);

        private void OnChangePlayerMovement()
        {
            switch (PlayerMovementModel.MovementID)
            {
                case PlayerMovementID.Ground:
                    View.SetVisibleAttackImage(true);
                    break;
                case PlayerMovementID.Water:
                    View.SetVisibleAttackImage(false);
                    break;
            }
        }

        private Vector2 AdjustDeltaToResolution(Vector2 pixelsDelta) {
            float x = pixelsDelta.x * (baseResolotion.x / Screen.width);
            float y = pixelsDelta.y * (baseResolotion.y / Screen.height);
            return new Vector2(x, y);
        }
    }
}
