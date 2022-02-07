using System;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Purchases;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class FishingLinkController : ViewEnableController<FishingLinkView>, IFishingLinkController
    {
        [Inject] public MoreGamesModel MoreGamesModel {get;set;}
        [Inject] public ApplicationCallbacksModel ApplicationCallbacksModel {get;set;}
        [Inject] public CoinsModel CoinsModel {get;set;}
        [Inject] public PurchasesModel PurchasesModel {get;set;}

        public override ViewConfigID ViewConfigID => ViewConfigID.FishingLinkConfig;

        public override bool IsCanShow => false;


        private bool isFishingLinkClicked = false;
        private DateTime pauseDateTime;

        public override void Enable()
        {
            ViewsSystem.OnEndShow.AddListener(ViewConfigID.Settings, OnShowSettings);
            ViewsSystem.OnEndHide.AddListener(ViewConfigID.Settings, OnHideSettings);
            MoreGamesModel.OnLastFishLinkClicked += OnFishingLinkClicked;
            ApplicationCallbacksModel.ApplicationPause += OnApplicationPause;
            ApplicationCallbacksModel.ApplicationFocus += OnApplicationFocus;
            UpdateViewVisible();
        }

        public override void Disable()
        {
            ViewsSystem.OnEndShow.RemoveListener(ViewConfigID.Settings, OnShowSettings);
            ViewsSystem.OnEndHide.RemoveListener(ViewConfigID.Settings, OnHideSettings);
            MoreGamesModel.OnLastFishLinkClicked -= OnFishingLinkClicked;
            ApplicationCallbacksModel.ApplicationPause -= OnApplicationPause;
            ApplicationCallbacksModel.ApplicationFocus -= OnApplicationFocus;

            Hide();
        }

        public override void Start()
        {

        }

        private void OnShowSettings()
        {
            UpdateViewVisible();
        }

        private void OnHideSettings()
        {
            UpdateViewVisible();
        }

        private void OnFishingLinkClicked()
        {
            isFishingLinkClicked = true;
            pauseDateTime = DateTime.Now;
            Application.OpenURL(MoreGamesModel.LastFishLink);
        }

        private void OnApplicationPause(bool pause) => ApplicationPause(pause);

        private void OnApplicationFocus(bool focus) => ApplicationPause(!focus);

        private void ApplicationPause(bool pause)
        {
            if(!pause)
            {
                if(isFishingLinkClicked && !MoreGamesModel.IsLastFishRewardReceived)
                {
                    float pauseTimeSec = (float)DateTime.Now.Subtract(pauseDateTime).TotalSeconds;
                    if(pauseTimeSec >= MoreGamesModel.RewardPauseTime)
                    {
                        PurchasesModel.Purchase(PurchaseID.OpenLastFishLink, (result) => {
                                if(result == PurchaseResult.Successfully)
                                {
                                    MoreGamesModel.SetLastFishRewardRecieved(true);
                                }
                            });
                    }
                }
                isFishingLinkClicked = false;
            }
        }


    }
}
