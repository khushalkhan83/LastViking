using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using UnityEngine;

namespace Game.Purchases.Purchasers
{
    abstract public class PurchaserStoreBase : PurchaserBase, IPurchaseStoreInfo
    {
        #region Data
#pragma warning disable 0649
        [ObscuredID(typeof(ProductID))]
        [SerializeField] private ObscuredInt _productID;

#pragma warning restore 0649
        #endregion

        private ProductID ProductID => (ProductID)(int)_productID;
        private PurchasersStoreModel PurchasersStoreModel => ModelsSystem.Instance.purchasersStoreModel;
        private PurchasesModel PurchasesModel => ModelsSystem.Instance._purchasesModel;

        public string PriceString => PurchasersStoreModel.GetProduct(ProductID)?.metadata?.localizedPriceString;
        public string Title => PurchasersStoreModel.GetProduct(ProductID)?.metadata?.localizedTitle;
        public string Description => PurchasersStoreModel.GetProduct(ProductID)?.metadata?.localizedDescription;
        public decimal Price => PurchasersStoreModel.GetProduct(ProductID)?.metadata?.localizedPrice ?? 0;

        private PurchaseCallback PurchaseCallback { get; set; }
        private bool WaitForPuchaseCallback {get; set; }

        private Dictionary<ProductID,PurchaseID> PurchaseIDByProductID = new Dictionary<ProductID,PurchaseID>()
        {
            {ProductID.GoldTier_01,PurchaseID.GoldenPackTier01},
            {ProductID.GoldTier_02,PurchaseID.GoldenPackTier02},
            {ProductID.GoldTier_03,PurchaseID.GoldenPackTier03},
            {ProductID.GoldTier_04,PurchaseID.GoldenPackTier04},
            {ProductID.GoldTier_05,PurchaseID.GoldenPackTier05},
            {ProductID.GoldTier_06,PurchaseID.GoldenPackTier06}
        };

        public override bool IsCanPurchase => PurchasersStoreModel.IsReady;



        #region MonoBehaviour
        private void OnEnable()
        {
            PurchasersStoreModel.OnFailed.AddListener(ProductID, OnFailed);
            PurchasersStoreModel.OnSuccess.AddListener(ProductID, OnSuccess);
        }

        private void OnDisable()
        {
            PurchasersStoreModel.OnFailed.RemoveListener(ProductID, OnFailed);
            PurchasersStoreModel.OnSuccess.RemoveListener(ProductID, OnSuccess);
        }

        #endregion

        protected abstract void AddReward();

        public override void Purchase(PurchaseCallback callback)
        {
            if (!IsCanPurchase)
            {
                return;
            }

            PurchaseCallback = callback;
            WaitForPuchaseCallback = true;
            PurchasersStoreModel.BuyProduct(ProductID);
        }

        private void OnFailed()
        {
            EndPurchaseProcessing(PurchaseResult.Failure);
        }

        private void OnSuccess()
        {
            AddReward();
            EndPurchaseProcessing(PurchaseResult.Successfully);
        }

        private void EndPurchaseProcessing(PurchaseResult purchaseResult)
        {
            #region Logic
            
            if(WaitForPuchaseCallback)
            {
                Callback();
            }
            else
            {
                FinalizeCallback();
            }

            WaitForPuchaseCallback = false;
                
            #endregion
            
            #region Methods
                
            void Callback()
            {
                PurchaseCallback?.Invoke(purchaseResult);
            }

            void FinalizeCallback()
            {
                PurchaseID purshaceID = PurchaseIDByProductID[ProductID];
                PurchasesModel.FinalizePurchase(purshaceID,purchaseResult);
            }
            #endregion
        }
    }
}
