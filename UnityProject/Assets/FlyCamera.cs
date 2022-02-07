using Game.Models;
using UnityEngine;

namespace DebugRelated
{
    public class FlyCamera : MonoBehaviour
    {

        private Vector3 _angles;
        public float speed = 1.0f;
        public float fastSpeed = 2.0f;
        public float mouseSpeed = 4.0f;

        
        private Vector2 moveInput;
        private float move_Y;
        private float move_X;
        private Vector2 rotateInput;
        private float rotate_X;
        private float rotate_Y;

        private JoystickModel JoystickModel => ModelsSystem.Instance._joystickModel;
        private TouchpadModel TouchpadModel => ModelsSystem.Instance._touchpadModel;

        private void OnEnable()
        {
            _angles = transform.eulerAngles;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable() { Cursor.lockState = CursorLockMode.None; }

        private void Update()
        {
            #region Read input

            rotateInput = TouchpadModel.Axes;
            rotate_X = rotateInput.x;
            rotate_Y = rotateInput.y;

            moveInput = JoystickModel.AxesNormalized;
            move_Y = moveInput.y;
            move_X = moveInput.x;

            #endregion


            #region Apply movement

            _angles.x -= rotate_Y * mouseSpeed;
            _angles.y += rotate_X * mouseSpeed;
            // _angles.x -= Input.GetAxis("Mouse Y") * mouseSpeed;
            // _angles.y += Input.GetAxis("Mouse X") * mouseSpeed;


            transform.eulerAngles = _angles;
            float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : speed;
            transform.position +=
                // Input.GetAxis("Horizontal") * moveSpeed * transform.right +
                // Input.GetAxis("Vertical") * moveSpeed * transform.forward;
                move_X * moveSpeed * transform.right +
                move_Y * moveSpeed * transform.forward;

            #endregion
        }
    }
}
