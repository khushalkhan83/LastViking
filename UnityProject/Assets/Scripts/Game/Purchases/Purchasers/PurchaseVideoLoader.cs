using System;
using Core.Views;
using Game.Views;
using UnityEngine;

namespace Game.Purchases.Purchasers
{
    public class PurchaseVideoLoader : WatchPurchaseBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private WatchPurchaseBase _watchPurchaseBase;
        [SerializeField] private ViewsSystem _viewsSystem;

#pragma warning restore 0649
        #endregion

        public WatchPurchaseBase WatchPurchase => _watchPurchaseBase;
        public ViewsSystem ViewsSystem => _viewsSystem;

        public LoadingCircleView LoadingView { get; private set; }

        public override event Action<bool> OnReady;

        private void OnEnable()
        {
            WatchPurchase.OnReady += OnReadyHandler;
        }

        private void OnDisable()
        {
            WatchPurchase.OnReady -= OnReadyHandler;
            HideLoadingView();
        }

        private void OnReadyHandler(bool isReady) => OnReady?.Invoke(isReady);

        public override void Purchase(PurchaseCallback callback)
        {
            ShowLoadingView();
            WatchPurchase.Purchase(callback + Callback);
        }

        private void Callback(PurchaseResult purchaseResult) => HideLoadingView();

        public override void Prepere() => WatchPurchase.Prepere();
        public override void Cancel() => WatchPurchase.Cancel();
        public override bool IsCanPurchase => WatchPurchase.IsCanPurchase;

        private void ShowLoadingView()
        {
            LoadingView = ViewsSystem.Show<LoadingCircleView>(ViewConfigID.LoadingCircle);
            LoadingView.OnClose += OnCloseLoadingViewHandler;
            LoadingView.OnHide += OnHideLoadingViewHandler;
        }

        private void OnHideLoadingViewHandler(IView view) => OnHideLoadingView((LoadingCircleView)view);

        private void OnHideLoadingView(LoadingCircleView loadingView)
        {
            loadingView.OnClose -= OnCloseLoadingViewHandler;
            loadingView.OnHide -= OnHideLoadingViewHandler;
        }

        private void HideLoadingView()
        {
            if (LoadingView != null)
            {
                ViewsSystem.Hide(LoadingView);
                LoadingView = null;
            }
        }

        private void OnCloseLoadingViewHandler()
        {
            LoadingView = null;
            Cancel();
        }
    }
}
