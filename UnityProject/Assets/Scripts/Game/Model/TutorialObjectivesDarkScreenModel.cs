using System;
using System.Collections.Generic;
using Core.Storage;
using Game.Views;
using UnityEngine;

namespace Game.Models
{
    public class TutorialObjectivesDarkScreenModel : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private int _stepToShowScreen;
        [SerializeField] private int _controlStepsCount;
        [SerializeField] private float _showDelay;

        #pragma warning restore 0649
        #endregion

        public event Action OnNextStep;
        public event Action OnStepsOver;

        public int StepToShowScreen => _stepToShowScreen;
        public int ControlStepsCount => _controlStepsCount;
        public float ShowDelay => _showDelay;

      

        public bool CanShowSteps => ControlStep < ControlStepsCount;
        public int ControlStep { private set; get; } = 1;
        public bool IsControlStepsOver => ControlStep >= ControlStepsCount + 1;

        public void NextStep()
        {
            ControlStep++;
            OnNextStep?.Invoke();
            if(IsControlStepsOver) OnStepsOver?.Invoke();
        }
    }
}
