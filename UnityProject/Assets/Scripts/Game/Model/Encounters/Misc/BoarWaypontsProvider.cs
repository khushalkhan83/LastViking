using System.Collections.Generic;
using UnityEngine;

namespace Encounters
{
    public class BoarWaypontsProvider : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private List<GameObject> wayPoints;
        #pragma warning restore 0649
        #endregion
        public List<GameObject> WayPoints {get => wayPoints;}
    }
}