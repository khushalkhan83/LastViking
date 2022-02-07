using UnityEngine;

namespace Game.Controllers
{
    public class ShelterView : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Transform _container;

#pragma warning restore 0649
        #endregion

        public Transform Container => _container;
    }
}
