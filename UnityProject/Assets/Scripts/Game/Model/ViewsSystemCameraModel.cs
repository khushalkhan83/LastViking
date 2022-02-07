using System;
using UnityEngine;

namespace Game.Models
{
    public class ViewsSystemCameraModel : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private Camera _camera;
        
        #pragma warning restore 0649
        #endregion

        public Camera Camera => _camera;
    }
}
