using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class StarterPackView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ResourceCellView[] _survivalCells;
        [SerializeField] private ResourceCellView[] _dominationCells;

        [SerializeField] private Text _remainOfferTimeText;

        [SerializeField] private DisablableButtonView _survivalButton;
        [SerializeField] private DisablableButtonView _dominationButton;

        [SerializeField] private Text _survivalButtonText;
        [SerializeField] private Text _dominationButtonText;

#pragma warning restore 0649
        #endregion

        public ResourceCellView[] SurvivalCells => _survivalCells;
        public ResourceCellView[] DominationCells => _dominationCells;

        public DisablableButtonView SurvivalButton => _survivalButton;
        public DisablableButtonView DominationButton => _dominationButton;

        public void SetRemainOfferTimeText(string text) => _remainOfferTimeText.text = text;
        public void SetSurvivalButtonText(string text) => _survivalButtonText.text = text;
        public void SetDominatioButtonText(string text) => _dominationButtonText.text = text;

        //UI
        public event Action OnBuySurvival;
        public void BuySurvival() => OnBuySurvival?.Invoke();

        public event Action OnBuyDominant;
        public void BuyDominant() => OnBuyDominant?.Invoke();

        public event Action OnClose;
        public void Close() => OnClose?.Invoke();
    }
}
