using System;
using Core.Storage;
using UnityEngine;

namespace Game.Models
{
    public class VikingTutorialModel : InitableModel<VikingTutorialModel.Data>
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private Data _data;
#pragma warning restore 0649
        #endregion
        protected override Data DataBase => _data;

        [Serializable]
        public class Data : DataBase
        {
            [SerializeField] private int _analyticsStep;

            public int AnalyticsStep
            {
                get => _analyticsStep;
                set { _analyticsStep = value; ChangeData();}
            }
        }

        public int AnalyticsStep
        {
            get => _data.AnalyticsStep;
            private set => _data.AnalyticsStep = value;
        }

        public event Action OnAnalyticsStepChanged;

        public void SetAnalyticsStep(int tutorialEventIndex)
        {
            if(tutorialEventIndex <= AnalyticsStep)
            {
                return;
            }

            AnalyticsStep =  tutorialEventIndex;
            OnAnalyticsStepChanged?.Invoke();
        }
    }
}