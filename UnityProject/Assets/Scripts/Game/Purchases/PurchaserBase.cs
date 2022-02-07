using UnityEngine;

namespace Game.Purchases.Purchasers
{
    abstract public class PurchaserBase : MonoBehaviour, IPurchaser
    {
        public abstract bool IsCanPurchase { get; }
        public abstract void Purchase(PurchaseCallback callback);
    }
}
