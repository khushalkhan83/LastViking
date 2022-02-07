using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class TutorialDeathPlayerView : ViewAnimateBase
    {
        #region Data
#pragma warning disable 0649

        [Header("Localization")]
        [SerializeField] private Text _title;
        [SerializeField] private Text _description;
        [SerializeField] private Text _resurrectButtonText;

#pragma warning restore 0649
        #endregion

        public void SetTextTitle(string text) => _title.text = text;
        public void SetTextDescription(string text) => _description.text = text;
        public void SetTextResurrectButtonText(string text) => _resurrectButtonText.text = text;

        //

        //UI
        public event Action OnResurrect;
        public void Resurrect() => OnResurrect?.Invoke();
    }
}
