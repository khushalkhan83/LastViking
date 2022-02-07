using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    public class LoadingCircleViewModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private float _buttonAppearTime;

#pragma warning restore 0649
        #endregion

        public float ButtonAppearTime => _buttonAppearTime;
    }
}