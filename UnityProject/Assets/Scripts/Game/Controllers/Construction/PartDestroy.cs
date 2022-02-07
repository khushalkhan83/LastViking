using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Part;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class PartDestroy : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private WorldObjectModel _worldObjectModel;
        [SerializeField] private PartBehaviour _partBehaviour;

#pragma warning restore 0649
        #endregion

        private WorldObjectModel WorldObjectModel => _worldObjectModel;
        private PartBehaviour PartBehaviour => _partBehaviour;

        private void OnEnable()
        {
            EventHandlers.OnDestroyedPart += OnDestroyedPart;
        }

        private void OnDisable()
        {
            EventHandlers.OnDestroyedPart -= OnDestroyedPart;
        }

        private void OnDestroyedPart(PartBehaviour part)
        {
            if (PartBehaviour == part)
            {
                EventHandlers.OnDestroyedPart -= OnDestroyedPart;
                WorldObjectModel.Delete();
            }
        }
    }
}
