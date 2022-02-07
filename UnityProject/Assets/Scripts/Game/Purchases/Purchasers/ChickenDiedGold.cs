using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using UnityEngine;

namespace Game.Purchases.Purchasers
{
    public class ChickenDiedGold : PurchaserBase, ICoinRewardInfo
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredInt _coinsAdd;
        [SerializeField] private CoinsModel _coinsModel;

#pragma warning restore 0649
        #endregion

        public int CoinsAdd => _coinsAdd;
        public CoinsModel CoinsModel => _coinsModel;

        public override void Purchase(PurchaseCallback callback)
        {
            if (IsCanPurchase)
            {
                CoinsModel.Adjust(CoinsAdd);
                callback?.Invoke(PurchaseResult.Successfully);
            }
            else
            {
                callback?.Invoke(PurchaseResult.Failure);
            }
        }

        public override bool IsCanPurchase => true;
    }
}
