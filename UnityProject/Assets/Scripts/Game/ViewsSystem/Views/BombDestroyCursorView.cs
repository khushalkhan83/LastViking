using Core.Views;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views
{
    public class BombDestroyCursorView : CursorViewExtended
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private Image _bombTimerAmountImage;
        [SerializeField] private Text _bombTimerText;
        [SerializeField] private GameObject _noBomb;
        [SerializeField] private GameObject _useBomb;
        [SerializeField] private GameObject _bombPlantedTimer;

#pragma warning restore 0649
        #endregion

        public void SetBombFillAmount(float value) => _bombTimerAmountImage.fillAmount = value;
        public void SetBombTimerText(string text) => _bombTimerText.text = text;

        public void SetNobombVisible(bool isVisible) => _noBomb.SetActive(isVisible);

        public void SetUseBombVisible(bool isVisible) => _useBomb.SetActive(isVisible);

        public void SetBombPlantedTimerVisible(bool isVisible) => _bombPlantedTimer.SetActive(isVisible);
    }

}