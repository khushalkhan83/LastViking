using UnityEngine;

namespace Game.Controllers
{
    public class ShelterLevelView : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Transform _containerCore;

#pragma warning restore 0649
        #endregion

        public Transform ContainerTarget => _containerCore;
    }
}
