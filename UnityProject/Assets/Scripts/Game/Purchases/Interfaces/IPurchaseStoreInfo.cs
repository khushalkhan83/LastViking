namespace Game.Purchases
{
    public interface IPurchaseStoreInfo
    {
        string PriceString { get; }
        string Title { get; }
        string Description { get; }
        decimal Price { get; }
    }
}
