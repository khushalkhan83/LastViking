using System;
using UnityEngine;

namespace Game.Models
{
    public class JoystickModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private float _radius;
        [SerializeField] private float _smoothAxes;

        [Range(0.0001f, 1.0f)]
        [SerializeField] private float _smoothJoystick;

#pragma warning restore 0649
        #endregion

        public float Radius => _radius;
        public float SmoothAxes => _smoothAxes;
        public float SmoothJoystick => _smoothJoystick;

        public Vector2 Axes { get; private set; }
        public Vector2 AxesNormalized => Axes / Radius;

        public event Action OnInteract;

        public void SetAxes(Vector2 axes)
        {
            Axes = axes;
        }

        public void SetDefaultAxes()
        {
            SetAxes(Vector2.zero);
        }

        public void Interact() => OnInteract?.Invoke();
    }
}
