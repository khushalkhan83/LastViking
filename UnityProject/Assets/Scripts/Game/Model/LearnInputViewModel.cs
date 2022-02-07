using System;
using UnityEngine;

namespace Game.Models
{
    public class LearnInputViewModel : MonoBehaviour
    {
        public bool IsShow { get; private set; }
        public event Action OnShowChanged;

        public void SetShow(bool value)
        {
            IsShow = value;
            OnShowChanged?.Invoke();
        }

        public int Step {get; private set;} = 1;
        public event Action OnStepChanged;

        public void SetStep(int step)
        {
            Step = step;
            OnStepChanged?.Invoke();
        }

        public int MaxStep;

        public bool IsLastStep => Step >= MaxStep;


        public event Action OnFinish;

        public void SetFinish()
        {
            OnFinish?.Invoke();
        }
    }
}
