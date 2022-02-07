using UnityEngine;
using System;
using Core.Storage;


namespace Game.Models
{
    public class ConstructionTutorialModel : MonoBehaviour
    {

        #region Data
#pragma warning disable 0649
        [SerializeField] private int _controlStepsCount;
        [SerializeField] private int _sellectedCellStepFoundation = 0;
        [SerializeField] private int _sellectedCellStepWalls = 0;
        [SerializeField] private int _sellectedCellStepRoof = 0;
        [SerializeField] private int _sellectedCellStepDoor = 1;
        [SerializeField] private int _sellectedOptionCellStepDoor = 1;

#pragma warning restore 0649
        #endregion

        public int ControlStepsCount => _controlStepsCount;
        public int SellectedCellStepFoundation => _sellectedCellStepFoundation;
        public int SellectedCellStepWalls => _sellectedCellStepWalls;
        public int SellectedCellStepRoof => _sellectedCellStepRoof;
        public int SellectedCellStepDoor => _sellectedCellStepDoor;
        public int SellectedOptionCellStepDoor => _sellectedOptionCellStepDoor;

        public event Action OnNextStep;
        public event Action OnShown;

        public int ControlStep { set; get; } = 1;

        public bool CanShowSteps => ControlStep < ControlStepsCount;
        public bool IsControlStepsOver => ControlStep >= ControlStepsCount + 1;

        public void NextStep()
        {
            ControlStep++;
            OnNextStep?.Invoke();
        }

        public void Shown() => OnShown?.Invoke();
    }
}
