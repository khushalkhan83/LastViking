using Game.Views;
using Core.Controllers;
using Game.Models;
using Core;
using UltimateSurvival;
using UltimateSurvival.Building;
using UnityEngine.EventSystems;
using Game.Purchases;
using Game.Purchases.Purchasers;
using Game.VillageBuilding;
using EasyBuildSystem.Runtimes.Internal.Part;

namespace Game.Controllers
{
    public class ReplaceButtonViewController : ViewControllerBase<ReplaceButtonView>
    {
        [Inject] public ReplaceBuildingModel ReplaceBuildingModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        protected override void Show() 
        {
            View.OnClick += OnClick;
            SetCoinsPrice();
        }

        protected override void Hide() 
        {
            View.OnClick -= OnClick;

        }

        private void SetCoinsPrice()
        {
            var info = PurchasesModel.GetInfo<ReplaceBuildingGold>(PurchaseID.ReplaceBuildingGold);
            View.SetPriceText(info.CoinCost.ToString());
        }

        private void OnClick()
        {
            var building = PlayerEventHandler.RaycastData.Value?.GameObject.GetComponentInParent<BuildingPiece>();
            if(building != null)
            {
                PurchasesModel.Purchase(PurchaseID.ReplaceBuildingGold, (result) => 
                    {
                        if (result == PurchaseResult.Successfully)
                        {
                            ReplaceBuildingModel.ReplaceBuilding(building);
                        }
                        else
                        {
                            ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
                        }
                    });
                return;
            }

            var partBehaviour = PlayerEventHandler.RaycastData.Value?.GameObject.GetComponentInParent<PartBehaviour>();
            if(partBehaviour != null)
            {
                PurchasesModel.Purchase(PurchaseID.ReplaceBuildingGold, (result) => 
                    {
                        if (result == PurchaseResult.Successfully)
                        {
                            ReplaceBuildingModel.ReplaceConstruction(partBehaviour);
                        }
                        else
                        {
                            ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
                        }
                    });
                return;
            }

        }

    }
}
