using UnityEngine;
using System;
using Core.Storage;

namespace Game.Models
{
    public class TutorialDarkScreenModel : MonoBehaviour
    {
        [Serializable]
        public class ShowData : DataBase, IImmortal
        {
            public bool HasShown;

            public void SetHasShown(bool shown)
            {
                HasShown = shown;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private ShowData _showData;
        [SerializeField] private int _stepToShowScreen;
        [SerializeField] private int _controlStepsCount;

#pragma warning restore 0649
        #endregion

        public ShowData Data => _showData;
        public int StepToShowScreen => _stepToShowScreen;
        public int ControlStepsCount => _controlStepsCount;

        public event Action OnNextStep;

        public int ControlStep { private set; get; } = 1;

        public bool HasShown
        {
            set => Data.SetHasShown(value);
            get => Data.HasShown;
        }

        public bool CanShowSteps => ControlStep < ControlStepsCount;
        public bool IsControlStepsOver => ControlStep >= ControlStepsCount + 1;

        public void NextStep()
        {
            ControlStep++;
            OnNextStep?.Invoke();
        }
    }
}
