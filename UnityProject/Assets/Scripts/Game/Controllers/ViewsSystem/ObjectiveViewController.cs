using Core;
using Core.Controllers;
using Game.Models;
using Game.Objectives;
using Game.Objectives.Actions;
using Game.Providers;
using Game.Purchases;
using Game.Purchases.Purchasers;
using Game.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Controllers
{
    public class ObjectiveViewControllerData : IDataViewController
    {
        public byte Id { get; }
        public ObjectiveModel ObjectiveModel { get; }
        public PurchaserBase Purchaser { get; }
        public int PurchasePrice { get; }

        public ObjectiveViewControllerData(byte id, ObjectiveModel objectiveModel, PurchaserBase purchaser, int purchasePrice)
        {
            Id = id;
            ObjectiveModel = objectiveModel;
            Purchaser = purchaser;
            PurchasePrice = purchasePrice;
        }
    }

    public class ObjectiveViewController : ViewControllerBase<ObjectiveView, ObjectiveViewControllerData>
    {
        public Color Grey { get; } = new Color(0.1607843f, 0.1647059f, 0.1803922f, 1f);
        public Color Green { get; } = new Color(0.5607843f, 0.7215686f, 0.1921569f, 1f);
        public Color White { get; } = new Color(1f, 0.9254903f, 0.7725491f, 1f);

        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public ObjectivesModel ObjectivesModel { get; private set; }
        [Inject] public SpritesProvider SpritesProvider { get; private set; }
        [Inject] public PlayerObjectivesModel PlayerObjectivesModel { get; private set; }
        [Inject] public ObjectivesWindowModel ObjectivesWindowModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public ObjectivesViewModel ObjectivesViewModel { get; private set; }


        private QuestionPopupView QuestionPopupView { get; set; }


        protected override void Show()
        {
            View.OnGetReward += OnGetRewardHandler;
            View.OnReroll += OnRerollHandler;

            Data.ObjectiveModel.OnProgress += OnProgressHandler;

            if(ObjectivesViewModel.GetAnimationMustPlay(Data.Id))
            {
                ObjectivesViewModel.StartCoroutine(DoActionAfterOneFrame(() => View.PlayFadeOutAnimation()));
                ObjectivesViewModel.SetAnimationPlayed(Data.Id);
            }

            UpdateObjective();
        }

        IEnumerator DoActionAfterOneFrame(Action action)
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }

        protected override void Hide()
        {
            Data.ObjectiveModel.OnProgress -= OnProgressHandler;
            View.OnGetReward -= OnGetRewardHandler;
            View.OnReroll -= OnRerollHandler;
            HideRerollPopup();
        }

        private void OnGetRewardHandler()
        {
            if (Data.ObjectiveModel.IsComplete)
            {
                Data.ObjectiveModel.Reward();
                ObjectivesWindowModel.Complete(Data.ObjectiveModel);
                PlayerObjectivesModel.End(Data.Id);
            }
        }

        private void OnRerollHandler()
        {
            if(Data.ObjectiveModel.IsComplete) return;
            if(!Data.ObjectiveModel.CanReroll) return;

            if(!Data.Purchaser.IsCanPurchase)
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
                return;
            }

            ShowRerollPopup();
        }

        private void ShowRerollPopup()
        {
            QuestionPopupView = ViewsSystem.Show<QuestionPopupView>(ViewConfigID.QuestionPopupConfig);
            QuestionPopupView.OnClose += OnCloseRerollPopupHandler;
            QuestionPopupView.OnApply += OnApplyRerollPopupHandler;
            SetRerollPopupLocalization();
        }

        private void HideRerollPopup()
        {
            if(QuestionPopupView != null)
            {
                QuestionPopupView.OnClose -= OnCloseRerollPopupHandler;
                QuestionPopupView.OnApply -= OnApplyRerollPopupHandler;
                ViewsSystem.Hide(QuestionPopupView);
                QuestionPopupView = null;
            }
        }

        private void SetRerollPopupLocalization()
        {
            if(QuestionPopupView != null)
            {
                QuestionPopupView.SetTextTitle(LocalizationModel.GetString(LocalizationKeyID.RerollObjectivePopup_Title));
                QuestionPopupView.SetTextDescription(LocalizationModel.GetString(LocalizationKeyID.RerollObjectivePopup_RerollQuestion));
                QuestionPopupView.SetTextOkButton(LocalizationModel.GetString(LocalizationKeyID.ResetWarning_OkBtn));
                QuestionPopupView.SetTextBackButton(LocalizationModel.GetString(LocalizationKeyID.NotEnoughSpacePopUp_BackBtn));
            }
        }

        private void OnCloseRerollPopupHandler() => HideRerollPopup();

        private void OnApplyRerollPopupHandler()
        {
            if(Data.Purchaser.IsCanPurchase)
            {
                Data.Purchaser.Purchase((result) => {
                    if(result != PurchaseResult.Successfully) return;
                    
                    ObjectivesViewModel.SetAnimationMustPlay(Data.Id);

                    PlayerObjectivesModel.End(Data.Id);
                    PlayerObjectivesModel.RerollObjective(Data.ObjectiveModel, Data.Id);
                });
            }
            HideRerollPopup();
        }

        private void UpdateObjective()
        {
            var progress = Mathf.Clamp01(Data.ObjectiveModel.Progress);
            var isVisibleSliderHandle = progress > 0 && progress < 1;

            SetBackgroundColor(GetBackgroundColor());

            UpdateRewardViews(Data.ObjectiveModel.ObjectiveData.Rewards);
            View.RewardButton.SetIsVisible(Data.ObjectiveModel.IsComplete);
            View.RerollButton.SetIsVisible(Data.ObjectiveModel.CanReroll);
            View.RerollButton.SetText(Data.PurchasePrice.ToString());
            View.RewardButton.SetText(LocalizationModel.GetString(LocalizationKeyID.LootBoxMenu_TakeBtn));
            View.SetColorDescriptionText(GetDescriptionTextColor());
            View.SetPColorProgressText(GetDescriptionTextColor());

            View.SetIsVisibleSliderHandle(isVisibleSliderHandle);
            View.SetFillAmountSlider(progress);
            View.SetTextObjectiveProgress(GetProgressText());
            View.SetTextObjectiveDescription(LocalizationModel.GetString(GetDescriptionKeyID()));
        }

        private void UpdateRewardViews(IEnumerable<ActionBaseData> rewards)
        {
            var it = rewards.Where(x => x.ActionID == ActionID.RewardGold || x.ActionID == ActionID.BluePrintsReward).GetEnumerator();

            ObjectiveRewardView rewardView;
            int i = 0;

            for (; i < View.RewardViews.Length; i++)
            {
                if (!it.MoveNext())
                {
                    break;
                }

                rewardView = View.RewardViews[i];
                rewardView.SetIsVisible(true);
                switch (it.Current)
                {
                    case RewardGoldActionData data:
                        rewardView.SetImageItem(SpritesProvider[SpriteID.CoinIcon]);
                        rewardView.SetTextCountItems(data.GoldReward.ToString());
                        break;
                    case RewardBluePrintsActionData data:
                        rewardView.SetImageItem(SpritesProvider[SpriteID.BluePrint]);
                        rewardView.SetTextCountItems(data.BluePrintsReward.ToString());
                        break;
                }
            }

            for (; i < View.RewardViews.Length; i++)
            {
                View.RewardViews[i].SetIsVisible(false);
            }
        }

        public string GetProgressText()
        {
            var all = Data.ObjectiveModel.GetAllInt;
            if (all == 0)
            {
                return string.Empty;
            }
            else
            {
                return $"{Data.ObjectiveModel.GetCurrentInt}/{all}";
            }
        }

        public Color GetBackgroundColor()
        {
            if (Data.ObjectiveModel.IsComplete)
            {
                return Green;
            }

            return Grey;
        }

        public Color GetDescriptionTextColor()
        {
            if (Data.ObjectiveModel.IsComplete)
            {
                return Grey;
            }

            return White;
        }

        public LocalizationKeyID GetDescriptionKeyID()
        {
            if (Data.ObjectiveModel.IsComplete)
            {
                return Data.ObjectiveModel.ObjectiveData.DescriptionCompleteKeyID;
            }

            return Data.ObjectiveModel.ObjectiveData.DescriptionKeyID;
        }

        private void OnProgressHandler(ObjectiveModel objectiveModel) => UpdateObjective();

        private void SetBackgroundColor(Color color)
        {
            View.SetColorBackground(color);
            foreach (var item in View.RewardViews)
            {
                item.SetBackgroundColor(color);
            }
        }
    }
}
