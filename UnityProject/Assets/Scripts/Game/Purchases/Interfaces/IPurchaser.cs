namespace Game.Purchases
{

    public interface IPurchaser
    {
        void Purchase(PurchaseCallback callback);
        bool IsCanPurchase { get; }
    }
}
