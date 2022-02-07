using Core.StateMachine;
using Game.Models;
using Game.Purchases;
using UnityEngine;

namespace Game.StateMachine.Effects
{
    public class PurchaseEffect : EffectBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private PurchaseID _purchaseID;

#pragma warning restore 0649
        #endregion

        private PurchaseID PurchaseID => _purchaseID;
        private PurchasesModel PurchasesModel => ModelsSystem.Instance._purchasesModel;

        public override void Apply()
        {
            PurchasesModel.Purchase(PurchaseID);
        }
    }
}
