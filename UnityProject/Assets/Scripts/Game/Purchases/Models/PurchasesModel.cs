using System;
using UnityEngine;

namespace Game.Purchases
{
    public delegate void PurchaseCallback(PurchaseResult purchaseResult);

    public class PurchasesModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private PurchasersProvider _purchasersProvider;

#pragma warning restore 0649
        #endregion

        public PurchasersProvider PurchasersProvider => _purchasersProvider;

        public event Action<PurchaseID> OnPurchaseSuccessfully;
        public event Action<PurchaseID> OnPurchaseUnsuccessfully;

        public bool IsPurchasing { get; private set; } = false;

        public void Purchase(PurchaseID purchaseID, PurchaseCallback callback = null)
        {
            IsPurchasing = true;
            GetPurchaser(purchaseID).Purchase(callback + (result => PurchaseResultCallback(purchaseID, result)));
        }

        public void FinalizePurchase(PurchaseID purchaseID, PurchaseResult purchaseResult)
        {
            PurchaseResultCallback(purchaseID,purchaseResult);
        }

        public bool IsCanPurchase(PurchaseID purchaseID) => GetPurchaser(purchaseID).IsCanPurchase;

        public I GetInfo<I>(PurchaseID purchaseID) where I : class => PurchasersProvider[purchaseID] as I;

        private IPurchaser GetPurchaser(PurchaseID purchaseID) => PurchasersProvider[purchaseID];

        private void PurchaseResultCallback(PurchaseID purchaseID, PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                OnPurchaseSuccessfully?.Invoke(purchaseID);
            }
            else
            {
                OnPurchaseUnsuccessfully?.Invoke(purchaseID);
            }

            IsPurchasing = false;
        }
    }
}
