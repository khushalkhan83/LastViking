using UnityEngine;
using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Part;
using System;

namespace Game.Models
{
    /// <summary>
    /// BuildingProcessModel (& controller) - adapter / wrapper  of EasyBuildSystem stuff
    /// </summary>
    public class BuildingProcessModel : MonoBehaviour
    {
        public event Action OnPartPlaced;
        public event Action<GameObject> OnPartPlacedStable;
        public event Action OnBuildingStarted;
        public event Action OnBuildingEnded;

        public bool IsBuildingEnabled { get; private set; }

        public PartBehaviour SelectedPart => BuilderBehaviour.Instance.SelectedPrefab;

        public void PlacePart() => OnPartPlaced?.Invoke();
        public void PlacePartStable(GameObject part) => OnPartPlacedStable?.Invoke(part);

        public void SelectBuildItem(PartBehaviour item)
        {
            BuilderBehaviour.Instance.SelectPrefab(item);
        }

        public void SelectBuildItem(int id)
        {
            BuilderBehaviour.Instance.SelectPrefab(id);
        }

        public void StartBuild()
        {
            BuilderBehaviour.Instance.ChangeMode(BuildMode.Placement);
            IsBuildingEnabled = true;
            OnBuildingStarted?.Invoke();
        }

        public void Place()
        {
            if (BuilderBehaviour.Instance.CurrentMode == BuildMode.Placement)
                BuilderBehaviour.Instance.PlacePrefab();
        }

        public void Rotate(Vector3 angle)
        {
            if (BuilderBehaviour.Instance.CurrentMode == BuildMode.Placement)
                BuilderBehaviour.Instance.RotatePreview(angle);
        }

        public void Rotate(float angle)
        {
            Rotate(Vector3.up * angle);
        }

        public void CancelBuild()
        {
            BuilderBehaviour.Instance.ChangeMode(BuildMode.None);
            IsBuildingEnabled = false;
            OnBuildingEnded?.Invoke();
        }
    }
}
