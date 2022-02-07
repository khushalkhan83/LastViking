using System;

namespace Game.Purchases.Purchasers
{
    public abstract class WatchPurchaseBase : PurchaserBase, IWatchPurchase
    {
        public abstract event Action<bool> OnReady;

        public abstract void Cancel();
        public abstract void Prepere();
    }
}
