using UnityEngine;
using Game.Models;
using System;

namespace Game.Controllers
{
    public class TutorialRestrictionController : MonoBehaviour
    {
        [Serializable]
        public class StepRestriction
        {
            public int firstStep;
            public int lastStep;
            public GameObject restrictionGO;
        }

        #region Data
        #pragma warning disable 0649
        [SerializeField] StepRestriction[] restrictions;
        
        #pragma warning restore 0649
        #endregion

        private TutorialModel TutorialModel => ModelsSystem.Instance._tutorialModel;

        #region MonoBehaviour
        private void OnEnable()
        {
            TutorialModel.Init();
            TutorialModel.OnComplete += UpdateRestrictions;
            TutorialModel.OnStepChanged += UpdateRestrictions;
            UpdateRestrictions();
        }

        private void OnDisable()
        {
            TutorialModel.OnComplete -= UpdateRestrictions;
            TutorialModel.OnStepChanged -= UpdateRestrictions;
        }
        #endregion

        private void UpdateRestrictions()
        {
            if(TutorialModel.IsComplete)
            {
                foreach(var restriction in restrictions)
                {
                    restriction.restrictionGO.SetActive(false);
                }
            }
            else
            {
                foreach(var restriction in restrictions)
                {
                    restriction.restrictionGO.SetActive(TutorialModel.Step >= restriction.firstStep && TutorialModel.Step <= restriction.lastStep);
                }
            }
        }
    }
}