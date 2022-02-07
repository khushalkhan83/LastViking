using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UnityEngine;
using System;

namespace Game.Controllers
{
    public class TutorialObjectsEnabler : MonoBehaviour
    {
        [Serializable]
        public class ObjectSettings
        {
            public GameObject gameObject;
            public int targetStep;
            public ActivationType activationType;
        }

        public enum ActivationType
        {
            BeforeStep,
            OnlyOnStep,
            AfterStep,
        }

        [SerializeField] ObjectSettings[] settings = default;

        private TutorialModel TutorialModel => ModelsSystem.Instance._tutorialModel;

        private void OnEnable() 
        {
            TutorialModel.OnStepChanged += UpdateObjects;
            UpdateObjects();
        }

        private void OnDisable() 
        {
            TutorialModel.OnStepChanged -= UpdateObjects;
        }

        private void UpdateObjects()
        {
            foreach(var s in settings)
            {
                switch(s.activationType)
                {
                    case ActivationType.BeforeStep:
                        s.gameObject.SetActive(TutorialModel.Step <= s.targetStep);
                        break;
                    case ActivationType.OnlyOnStep:
                        s.gameObject.SetActive(TutorialModel.Step == s.targetStep);
                        break;
                    case ActivationType.AfterStep:
                        s.gameObject.SetActive(TutorialModel.Step >= s.targetStep);
                        break;
                }
            }
        }
    }
}
