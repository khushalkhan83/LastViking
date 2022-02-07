using static Game.Models.StarterPackModel;

namespace Game.Purchases.Purchasers
{
    public class DominationPackStore : PackPurchaserStoreBase
    {
        protected override ItemSettings[] Items => StarterPackModel.DominationItems;
    }
}
