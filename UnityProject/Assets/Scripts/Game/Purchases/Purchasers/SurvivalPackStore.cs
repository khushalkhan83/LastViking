using static Game.Models.StarterPackModel;

namespace Game.Purchases.Purchasers
{
    public class SurvivalPackStore : PackPurchaserStoreBase
    {
        protected override ItemSettings[] Items => StarterPackModel.SurvivalItems;
    }
}
