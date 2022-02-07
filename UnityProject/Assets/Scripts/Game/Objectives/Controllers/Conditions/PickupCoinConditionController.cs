using Core;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;
using Game.Purchases;

namespace Game.Objectives.Conditions.Controllers
{
    public class PickupCoinConditionController : BaseConditionController<PickupCoinConditionData, CountConditionDataModel>
    {
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }

        protected override void Subscribe()
        {
            PurchasesModel.OnPurchaseSuccessfully += OnPurchaseSuccessfullyHandler;
        }

        protected override void Unsubscribe()
        {
            PurchasesModel.OnPurchaseSuccessfully -= OnPurchaseSuccessfullyHandler;
        }

        private void OnPurchaseSuccessfullyHandler(PurchaseID purchaseID)
        {
            var coinReward = PurchasesModel.GetInfo<ICoinRewardInfo>(purchaseID);
            EventProcessing
            (
                data => coinReward != null
                , model => model.Progress(coinReward.CoinsAdd)
                , (data, model) => data.Value <= model.Value
            );
        }
    }
}
