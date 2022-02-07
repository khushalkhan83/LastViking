using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class TutorialFoundationView : ViewBase
    {
          #region Data
#pragma warning disable 0649

        [VisibleObject, SerializeField] private GameObject[] _stepObjects;

        [Header("Localization")]
        [SerializeField] private Text _descriptionText;

#pragma warning restore 0649
        #endregion

        public void SetActiveStep(int index)
        {
            foreach(var stepObject in _stepObjects)
            {
                stepObject.SetActive(false);
            }
            _stepObjects[index].SetActive(true);
        }

        public void SetTextDescription(string text) => _descriptionText.text = text;

        public event Action OnClick;
        public void Click() => OnClick?.Invoke();
    }
}
