using Core;
using Core.Controllers;
using Game.Models;
using Game.Purchases;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class PurchaseComplitedController : IPurchaseComplitedController, IController
    {
        [Inject] public PurchaseComplitedModel PurchaseComplitedModel { get; set; }
        [Inject] public PurchasersStoreModel PurchasersStoreModel { get; set; }

        public void Enable()
        {
            PurchasersStoreModel.OnAnyPurchaseSuccess += OnAnyPurchaseSuccess;
        }

        public void Disable()
        {
            PurchasersStoreModel.OnAnyPurchaseSuccess -= OnAnyPurchaseSuccess;
        }

        public void Start()
        {
            
        }

        private void OnAnyPurchaseSuccess() 
        {
            PurchaseComplitedModel.AnyPurchaseComplited = true;
        }
    }
}
