using CodeStage.AntiCheat.ObscuredTypes;
using Core;
using Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
#if !UNITY_EDITOR
using UnityEngine.Purchasing.Security;
#endif

namespace Game.Purchases
{
    public class PurchasersStoreModel : MonoBehaviour, IStoreListener
    {
        [Serializable]
        protected class Data
        {
            #region Data
#pragma warning disable 0649

            [ObscuredID(typeof(ProductID))]
            [SerializeField] private ObscuredInt _productID;
            [ObscuredID(typeof(ProductType))]
            [SerializeField] private ObscuredInt _productType;

#pragma warning restore 0649
            #endregion

            public ProductID ProductID => (ProductID)(int)_productID;
            public ProductType ProductType => (ProductType)(int)_productType;
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data[] _products;
        [SerializeField] private ProductIDsProvider _productIDsProvider;

#pragma warning restore 0649
        #endregion

        public ProductIDsProvider ProductIDsProvider => _productIDsProvider;

        protected IEnumerable<Data> Products => _products;

        private UniqueAction<ProductID> OnFailedAction { get; } = new UniqueAction<ProductID>();
        private UniqueAction<ProductID> OnSuccessAction { get; } = new UniqueAction<ProductID>();

        public IUniqueEvent<ProductID> OnFailed => OnFailedAction;
        public IUniqueEvent<ProductID> OnSuccess => OnSuccessAction;


        public event Action Initialized;
        public event Action<ProductID> _TmpOnSuccess;

        public event Action OnAnyPurchaseSuccess;

        protected IStoreController StoreController { get; private set; }
        protected IExtensionProvider ExtensionProvider { get; private set; }

        private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;

        public bool IsReady { get; private set; }

        private void OnEnable()
        {
            PlayerScenesModel.OnEnvironmentLoaded += OnEnvironmentLoaded;
        }

        private void OnEnvironmentLoaded()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProducts(Products.Select(x => new ProductDefinition(ProductIDsProvider[x.ProductID], x.ProductType)));
            UnityPurchasing.Initialize(this, builder);
            PlayerScenesModel.OnEnvironmentLoaded -= OnEnvironmentLoaded;
        }

        void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            StoreController = controller;
            ExtensionProvider = extensions;
            IsReady = true;
            Initialized?.Invoke();
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
        {
            $"OnInitializeFailed {error}".Log();
        }

        void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason purchaseFailureReason)
        {
            $"OnPurchaseFailed {product}".Log();
            OnFailedAction.Invoke(GetProductID(product.definition.id));
        }

        PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs eventArgs)
        {
            $"ProcessPurchase {eventArgs.purchasedProduct.definition.id}".Log();

            if (IsValidPurchase(eventArgs.purchasedProduct))
            {
                OnSuccessAction.Invoke(GetProductID(eventArgs.purchasedProduct.definition.id));
                _TmpOnSuccess?.Invoke(GetProductID(eventArgs.purchasedProduct.definition.id));
                OnAnyPurchaseSuccess?.Invoke();
            }
            else
            {
                $"validPurchase FALSE".Log();
            }

            return PurchaseProcessingResult.Complete;
        }

        public bool IsValidPurchase(Product product)
        {
#if UNITY_EDITOR
            return true;
#else

            //code from Unity3d documentation
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

            try
            {
                // On Google Play, result has a single product ID.
                // On Apple stores, receipts contain multiple products.
                var result = validator.Validate(product.receipt);

                // For informational purposes, we list the receipt(s)
                Debug.Log("Receipt is valid. Contents:");

                foreach (var productReceipt in result)
                {
                    Debug.Log(productReceipt.productID);
                    Debug.Log(productReceipt.purchaseDate);
                    Debug.Log(productReceipt.transactionID);
                }
            }
            catch
            {
                return false;
            }

            return true;
#endif
        }

        private ProductID GetProductID(string productId)
        {
            var i = 0;
            foreach (var item in ProductIDsProvider)
            {
                ++i;
                if (item == productId)
                {
                    return (ProductID)i;
                }
            }

            return ProductID.None;
        }

        public Product GetProduct(ProductID productID) => StoreController?.products.WithID(ProductIDsProvider[productID]);

        public void BuyProduct(ProductID productID) => StoreController?.InitiatePurchase(GetProduct(productID));
    }
}
