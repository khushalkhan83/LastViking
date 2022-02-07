using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class PurchasesView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _coinsCalue;

        [SerializeField] private Transform _starterPackHolder;
        [SerializeField] private DisablableButtonView _mobileAddButton;
        [SerializeField] private DisablableButtonView _costTier01Button;
        [SerializeField] private DisablableButtonView _costTier02Button;
        [SerializeField] private DisablableButtonView _costTier03Button;
        [SerializeField] private DisablableButtonView _costTier04Button;
        [SerializeField] private DisablableButtonView _costTier05Button;
        [SerializeField] private DisablableButtonView _costTier06Button;
        [SerializeField] private ScrollRect _scrollRect;

        [VisibleObject] [SerializeField] private GameObject _watchPurchase;

#pragma warning restore 0649
        #endregion

        public Transform StarterPackHolder => _starterPackHolder;
        public DisablableButtonView PurchaseWatchButton => _mobileAddButton;
        public DisablableButtonView GoldenPackTier01Button => _costTier01Button;
        public DisablableButtonView GoldenPackTier02Button => _costTier02Button;
        public DisablableButtonView GoldenPackTier03Button => _costTier03Button;
        public DisablableButtonView GoldenPackTier04Button => _costTier04Button;
        public DisablableButtonView GoldenPackTier05Button => _costTier05Button;
        public DisablableButtonView GoldenPackTier06Button => _costTier06Button;
        public ScrollRect ScrollRect => _scrollRect;

        public void SetTextCoinsValue(string text) => _coinsCalue.text = text;
        public void SetCostMobile(string text) => _mobileAddButton.SetText(text);
        public void SetTextCost10(string text) => _costTier01Button.SetText(text);
        public void SetTextCost50(string text) => _costTier02Button.SetText(text);
        public void SetTextCost150(string text) => _costTier03Button.SetText(text);
        public void SetTextCost345(string text) => _costTier04Button.SetText(text);
        public void SetTextCost750(string text) => _costTier05Button.SetText(text);
        public void SetVisiblePurchaseWatch(bool isVisible) => _watchPurchase.SetActive(isVisible);

        //UI
        public event Action OnClose;
        public void ActionClose() => OnClose?.Invoke();

        //UI
        public event Action OnPurchaseWatch;
        public void ActionPurchase10() => OnPurchaseWatch?.Invoke();

        //UI
        public event Action OnPurchaseGoldenPackTier01;
        public void ActionPurchase50() => OnPurchaseGoldenPackTier01?.Invoke();

        //UI
        public event Action OnPurchaseGoldenPackTier02;
        public void ActionPurchase150() => OnPurchaseGoldenPackTier02?.Invoke();

        //UI
        public event Action OnPurchaseGoldenPackTier03;
        public void ActionPurchase345() => OnPurchaseGoldenPackTier03?.Invoke();

        //UI
        public event Action OnPurchaseGoldenPackTier04;
        public void ActionPurchase750() => OnPurchaseGoldenPackTier04?.Invoke();
        //UI
        public event Action OnPurchaseGoldenPackTier05;
        public void ActionPurchase3000() => OnPurchaseGoldenPackTier05?.Invoke();
        //UI
        public event Action OnPurchaseGoldenPackTier06;
        public void ActionPurchasePackTier06() => OnPurchaseGoldenPackTier06?.Invoke();
    }
}
