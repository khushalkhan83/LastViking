using System;
using UnityEngine;

namespace Game.Models
{
    public class CampFireViewModel : InventoryCellsViewModelBase
    {
        public event Action OnShowChanged;
        public bool IsShow { get; private set; }

        public void Show(bool show)
        {
            IsShow = show;
            OnShowChanged?.Invoke();
        }

        public Func<GameObject> OnGetSwitchFireModeButton;
        public Func<GameObject> OnGetBoostsButton;

        public GameObject GetSwitchFireModeButton() => OnGetSwitchFireModeButton?.Invoke();
        public GameObject GetBoostsButton() => OnGetBoostsButton?.Invoke();

        public event Action<ContainerID,int> OnTutorialHilightCell;
        public event Action OnRemoveAllTutorialCellsHilight;

        public void TutorialHilightCell(ContainerID container, int cellID)
        {
            OnTutorialHilightCell?.Invoke(container,cellID);
        }
        
        public void RemoveAllTutorialCellsHilight()
        {
            OnRemoveAllTutorialCellsHilight?.Invoke();
        }

        public event Action<ContainerID,int,ContainerID,int> OnTutorialDragAnimation;
        public event Action OnStopTutorialDragAnimation;

        public void DoTutorialDragAnimation(ContainerID item1_containerID, int item1_cellID, ContainerID item2_containerID, int item2_cellID)
        {
            OnTutorialDragAnimation?.Invoke(item1_containerID,item1_cellID,item2_containerID,item2_cellID);
        }

        public void StopTutorialDragAnimation()
        {
            OnStopTutorialDragAnimation?.Invoke();
        }
    }
}