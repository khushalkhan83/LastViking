using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class ConstructionTutorialView : ViewBase
    {
         #region Data
#pragma warning disable 0649
        [VisibleObject, SerializeField] private GameObject _foundationStep;
        [VisibleObject, SerializeField] private GameObject _wallsStep;
        [VisibleObject, SerializeField] private GameObject _roofStep;
        [VisibleObject, SerializeField] private GameObject[] _doorSteps;

        [Header("Localization")]
        [SerializeField] private Text _foundationText;
        [SerializeField] private Text _wallsText;
        [SerializeField] private Text _roofText;
        [SerializeField] private Text _doorText1;
        [SerializeField] private Text _doorText2;

#pragma warning restore 0649
        #endregion

        public void SetActiveFoundationStep()
        {
            _foundationStep.SetActive(true);
            _wallsStep.SetActive(false);
            _roofStep.SetActive(false);
            foreach(var stepObject in _doorSteps)
            {
                stepObject.SetActive(false);
            }
        }

        public void SetActiveWallsStep()
        {
            _foundationStep.SetActive(false);
            _wallsStep.SetActive(true);
            _roofStep.SetActive(false);
            foreach(var stepObject in _doorSteps)
            {
                stepObject.SetActive(false);
            }
        }

        public void SetActiveRoofStep()
        {
            _foundationStep.SetActive(false);
            _wallsStep.SetActive(false);
            _roofStep.SetActive(true);
            foreach(var stepObject in _doorSteps)
            {
                stepObject.SetActive(false);
            }
        }

        public void SetActiveDoorStep(int index)
        {
            _foundationStep.SetActive(false);
            _wallsStep.SetActive(false);
            _roofStep.SetActive(false);
            foreach(var stepObject in _doorSteps)
            {
                stepObject.SetActive(false);
            }
            _doorSteps[index].SetActive(true);
        }

        public void SetTextFoundation(string text) => _foundationText.text = text;
        public void SetTextWalls(string text) => _wallsText.text = text;
        public void SetTextRoof(string text) => _roofText.text = text;
        public void SetTextDoor1(string text) => _doorText1.text = text;
        public void SetTextDoor2(string text) => _doorText2.text = text;
        

        public event Action OnClick;
        public void Click() => OnClick?.Invoke();

        public event Action OnHold;
        public void Hold() => OnHold?.Invoke();
    }
}
