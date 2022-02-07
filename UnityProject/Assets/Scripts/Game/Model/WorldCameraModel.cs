using System;
using UnityEngine;

namespace Game.Models
{
    public class WorldCameraModel : MonoBehaviour
    {
        public Camera WorldCamera;
        public Camera DefaultWorldCamera;

        public event Action OnChangeCamera;

        public void SetCamera(Camera camera)
        {
            WorldCamera = camera;
            OnChangeCamera?.Invoke();
        }

        public void ResetCamera() => OnChangeCamera?.Invoke();
    }
}
