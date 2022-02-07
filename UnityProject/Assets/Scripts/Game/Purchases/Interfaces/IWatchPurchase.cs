using System;

namespace Game.Purchases
{
    public interface IWatchPurchase
    {
        bool IsCanPurchase { get; }

        event Action<bool> OnReady;

        void Prepere();
        void Cancel();
    }
}
