using Core;
using Core.Controllers;
using Game.Models;
using Game.Purchases;
using System;
// using Analytics = Firebase.Analytics.FirebaseAnalytics;
// using Firebase.Analytics;

namespace Game.Controllers
{
    public class AnaliticsFirebaseController : IAnaliticsFirebaseController, IController
    {
        // [Inject] public FirebaseModel FirebaseModel { get; private set; }
        // [Inject] public AnaliticsModel AnaliticsModel { get; private set; }
        // [Inject] public PurchasesModel PurchasesModel { get; private set; }
        // [Inject] public GameTimeModel GameTimeModel { get; private set; }
        // [Inject] public TutorialProgressModel TutorialProgressModel { get; private set; }

        void IController.Enable()
        {
            // if(!AnaliticsModel.EnableAnalitics) return;

            // FirebaseModel.OnFirebaseReady += OnFirebaseReadyHandler;
            // TutorialProgressModel.OnTutorialStart += OnTutorialStartHandler;
            // TutorialProgressModel.OnTutorialCompleted += OnTutorialCompleteHandler;
            // TutorialProgressModel.OnTutorialStep += OnTutorialStepHandler;
        }

        void IController.Start()
        {
        }

        void IController.Disable()
        {
            // if(!AnaliticsModel.EnableAnalitics) return;
            
            // FirebaseModel.OnFirebaseReady -= OnFirebaseReadyHandler;

            // UnsubscribeOnAnalyticsEvents();
        }

        // private void OnFirebaseReadyHandler() => SubscribeOnAnalyticsEvents();

        // private void SubscribeOnAnalyticsEvents()
        // {
        //     PurchasesModel.OnPurchaseSuccessfully += OnPurchaseSuccessfullyHandler;
        //     GameTimeModel.OnChangeDay += OnChangeDayHandler;
        // }

        // private void UnsubscribeOnAnalyticsEvents()
        // {
        //     PurchasesModel.OnPurchaseSuccessfully -= OnPurchaseSuccessfullyHandler;
        //     GameTimeModel.OnChangeDay -= OnChangeDayHandler;
        //     TutorialProgressModel.OnTutorialStart -= OnTutorialStartHandler;
        //     TutorialProgressModel.OnTutorialCompleted -= OnTutorialCompleteHandler;
        //     TutorialProgressModel.OnTutorialStep -= OnTutorialStepHandler;
        // }

        // private bool IsStorePurchase(ICoinRewardInfo coinReward) => coinReward is IPurchaseStoreInfo;
        // private bool IsStoreRewardedPurchase(ICoinRewardInfo coinReward) => IsStorePurchase(coinReward) || coinReward is IWatchPurchase;

        // private void SendCustom(AnaliticEventID eventID) => Analytics.LogEvent(AnaliticsModel.GetEventName(eventID));
        // private void SendCustom(AnaliticEventID eventID, params Parameter[] parameters) => Analytics.LogEvent(AnaliticsModel.GetEventName(eventID), parameters);

        // private void SendEvent(string name) => Analytics.LogEvent(name);
        // private void SendEvent(string name, params Parameter[] parameters) => Analytics.LogEvent(name, parameters);
        // private void SendEvent(string name, string paramName, int paramValue) => Analytics.LogEvent(name, paramName, paramValue);
        // private void SendEventWithPostfix(string name, string postfix) => Analytics.LogEvent(name + postfix);

        // private void OnTutorialCompleteHandler()
        // {
        //     if (!FirebaseModel.IsFirebaseReady) return;

        //     SendEvent(Analytics.EventTutorialComplete);
        // }

        // private void OnTutorialStartHandler()
        // {
        //     if (!FirebaseModel.IsFirebaseReady) return;

        //     SendEvent(Analytics.EventTutorialBegin);
        // }

        // private void OnTutorialStepHandler(int step)
        // {
        //     if (!FirebaseModel.IsFirebaseReady) return;

        //     SendEventWithPostfix(AnaliticEventID.TutorialStep.ToString(), $"{step:00}");
        // }

        // private void OnPurchaseSuccessfullyHandler(PurchaseID purchaseID)
        // {
        //     if (!FirebaseModel.IsFirebaseReady) return;

        //     var coinCost = PurchasesModel.GetInfo<IPurchaseCoinInfo>(purchaseID);
        //     if (coinCost != null)
        //     {
        //         Parameter[] parameters = new Parameter[]
        //         {
        //             new Parameter(Analytics.ParameterItemName, purchaseID.ToString()),
        //             new Parameter(Analytics.ParameterVirtualCurrencyName, AnaliticsModel.CoinCurrency),
        //             new Parameter(Analytics.ParameterValue, coinCost.CoinCost)
        //         };
        //         SendEvent(Analytics.EventSpendVirtualCurrency, parameters);
        //         return;
        //     }

        //     var coinReward = PurchasesModel.GetInfo<ICoinRewardInfo>(purchaseID);
        //     if (coinReward != null)
        //     {
        //         if (!IsStorePurchase(coinReward))
        //         {
        //             Parameter[] parameters = new Parameter[]
        //             {
        //                 new Parameter(Analytics.ParameterVirtualCurrencyName, AnaliticsModel.CoinCurrency),
        //                 new Parameter(Analytics.ParameterValue, coinReward.CoinsAdd)
        //             };
        //             SendEvent(Analytics.EventEarnVirtualCurrency, parameters);
        //         }
        //         return;
        //     }
        // }

        // private void OnChangeDayHandler()
        // {
        //     if (!FirebaseModel.IsFirebaseReady) return;

        //     int level = Convert.ToInt32(GameTimeModel.Days) + 1;
        //     Parameter parameters = new Parameter(Analytics.ParameterLevel, level);
        //     SendEvent(Analytics.EventLevelUp, parameters);
        // }
    }
}
