using Core;
using Core.Controllers;
using Core.Views;
using Game.Audio;
using Game.Models;
using Game.Objectives;
using Game.Purchases;
using Game.Purchases.Purchasers;
using Game.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;

namespace Game.Controllers
{
    public class ObjectivesViewController : ViewControllerBase<ObjectivesView>
    {
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public BluePrintsModel BluePrintsModel { get; private set; }
        [Inject] public CoinsModel CoinsModel { get; private set; }
        [Inject] public ObjectivesViewModel ObjectivesViewModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public PlayerObjectivesModel PlayerObjectivesModel { get; private set; }
        [Inject] public ObjectivesModel ObjectivesModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public RealTimeModel RealTimeModel { get; private set; }
        [Inject] public SheltersModel SheltersModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public ShelterUpgradeModel ShelterUpgradeModel { get; private set; }
        [Inject] public QuestsModel QuestsModel { get; private set; }
        [Inject] public ObjectivesWindowModel ObjectivesWindowModel { get; private set; }

        private Dictionary<byte, IView> ObjectiveViews { get; } = new Dictionary<byte, IView>();

        private DailyTaskRespinSlotGold DailyTaskRespinSlotGold => PurchasesModel.GetInfo<DailyTaskRespinSlotGold>(PurchaseID.DailyTaskRespinSlotGold);

        private bool IsVisibleTaskPanel => ObjectivesViewModel.IsHasAny || PlayerObjectivesModel.Pool.Any(x => x.IsHasValue);
        private bool IsReadyRealtime => RealTimeModel.isReady;
        private bool IsCanShowRemainingTimeTimer => ObjectivesWindowModel.RequiredObjectivesCompleated;

        protected override void Show()
        {
            ObjectivesViewModel.CloseGlobalObjectives();

            ForceCloseGlobalObjectives();
            UpdateBluePrints();
            UpdateCoins();
            SetLocalization();
            UpdatePanels();
            UpdateLayer2();
            UpdateGetButton();
            UpdateGoldButtonValue();
            SetupUpgradeResources();

            AudioSystem.PlayOnce(AudioID.WindowOpen);

            View.OnClose += OnCloseHandler;
            View.OnPickUpFirst += OnPickUpFirstHandler;
            View.OnPickUpSecond += OnPickUpSecondHandler;
            View.OnPickUpThird += OnPickUpThirdHandler;
            View.OnPickUpFourth += OnPickUpFourthHandler;
            View.OnPickUpFifth += OnPickUpFifthHandler;
            View.OnPickUpSixth += OnPickUpSixthHandler;
            View.OnGetFree += OnGetFreeHandler;
            View.OnGetWatch += OnGetWatchHandler;
            View.OnGetGold += OnGetGoldHandler;
            View.OnClickGlobalObjectives += OnClickGlobalObjectivesHandler;

            PlayerObjectivesModel.OnPreEndTask += OnEndTaskHandler;
            PlayerObjectivesModel.OnPostEndTask += OnPostEndTaskHandler;
            PlayerObjectivesModel.OnCreate += OnCreateHandler;

            ObjectivesViewModel.OnPickUpTutorialStep1 += OnChangeIsHasTutorialObjectiveHandler;
            ObjectivesViewModel.OnPickUpTutorialStep2 += OnChangeIsHasTutorialObjectiveHandler;
            ObjectivesViewModel.OnPickUpTutorialStep3 += OnChangeIsHasTutorialObjectiveHandler;
            ObjectivesViewModel.OnPickUpTutorialStep4 += OnChangeIsHasTutorialObjectiveHandler;
            ObjectivesViewModel.OnPickUpTutorialStep5 += OnChangeIsHasTutorialObjectiveHandler;
            ObjectivesViewModel.OnPickUpTutorialStep6 += OnChangeIsHasTutorialObjectiveHandler;
            ObjectivesViewModel.OnChangeIsOpenGlobalObjectives += OnChangeIsOpenGlobalObjectivesHandler;

            ObjectivesViewModel.OnShowTutorialButtonsOnTop += OnShowTutorialButtonsOnTop;

            RealTimeModel.OnTimeReady += OnReadyRealtimeHandler;
            RealTimeModel.OnTimeError += OnErrorRealtimeHandler;
            RealTimeModel.DropTime();

            GameUpdateModel.OnUpdate += OnUpdateHandler;

            LocalizationModel.OnChangeLanguage += OnChangeLanguageHandler;

            CoinsModel.OnChange += OnChangeCoinsHandler;

            BluePrintsModel.OnChange += OnChangeBluePrintsHandler;

            NetworkModel.OnCheckConnection += OnCheckConnectionHandler;

            ShowObjectives();

            var isHasStartedTutorial = TutorialModel.IsStart;

            View.SetIsVisibleObject1(ObjectivesViewModel.IsHas1 && isHasStartedTutorial);
            View.SetIsVisibleObject2(ObjectivesViewModel.IsHas2 && isHasStartedTutorial);
            View.SetIsVisibleObject3(ObjectivesViewModel.IsHas3 && isHasStartedTutorial);
            View.SetIsVisibleObject4(ObjectivesViewModel.IsHas4 && isHasStartedTutorial);
            View.SetIsVisibleObject5(ObjectivesViewModel.IsHas5 && isHasStartedTutorial);
            View.SetIsVisibleObject6(ObjectivesViewModel.IsHas6 && isHasStartedTutorial);

            SetShowTutorialButtonsOnTop(false);

            View.ScrollTasksTo(1);
        }

        protected override void Hide()
        {
            PlayerObjectivesModel.OnPostEndTask -= OnPostEndTaskHandler;
            PlayerObjectivesModel.OnPreEndTask -= OnEndTaskHandler;
            PlayerObjectivesModel.OnCreate -= OnCreateHandler;

            View.OnClose -= OnCloseHandler;
            View.OnPickUpFirst -= OnPickUpFirstHandler;
            View.OnPickUpSecond -= OnPickUpSecondHandler;
            View.OnPickUpThird -= OnPickUpThirdHandler;
            View.OnPickUpFourth -= OnPickUpFourthHandler;
            View.OnPickUpFifth -= OnPickUpFifthHandler;
            View.OnPickUpSixth -= OnPickUpSixthHandler;
            View.OnGetFree -= OnGetFreeHandler;
            View.OnGetWatch -= OnGetWatchHandler;
            View.OnGetGold -= OnGetGoldHandler;
            View.OnClickGlobalObjectives -= OnClickGlobalObjectivesHandler;

            ObjectivesViewModel.OnPickUpTutorialStep1 -= OnChangeIsHasTutorialObjectiveHandler;
            ObjectivesViewModel.OnPickUpTutorialStep2 -= OnChangeIsHasTutorialObjectiveHandler;
            ObjectivesViewModel.OnPickUpTutorialStep3 -= OnChangeIsHasTutorialObjectiveHandler;
            ObjectivesViewModel.OnPickUpTutorialStep4 -= OnChangeIsHasTutorialObjectiveHandler;
            ObjectivesViewModel.OnPickUpTutorialStep5 -= OnChangeIsHasTutorialObjectiveHandler;
            ObjectivesViewModel.OnPickUpTutorialStep6 -= OnChangeIsHasTutorialObjectiveHandler;

            ObjectivesViewModel.OnShowTutorialButtonsOnTop -= OnShowTutorialButtonsOnTop;

            RealTimeModel.OnTimeReady -= OnReadyRealtimeHandler;
            RealTimeModel.OnTimeError -= OnErrorRealtimeHandler;

            ObjectivesViewModel.OnChangeIsOpenGlobalObjectives -= OnChangeIsOpenGlobalObjectivesHandler;

            LocalizationModel.OnChangeLanguage -= OnChangeLanguageHandler;

            NetworkModel.OnCheckConnection -= OnCheckConnectionHandler;

            CoinsModel.OnChange -= OnChangeCoinsHandler;
            BluePrintsModel.OnChange -= OnChangeBluePrintsHandler;
            SetShowTutorialButtonsOnTop(false);
            HideObjectives();

            GameUpdateModel.OnUpdate -= OnUpdateHandler;
        }

        private void SetupUpgradeResources()
        {
            ShelterCost[] costs = null;
            if (SheltersModel.ShelterActive == ShelterModelID.None)
            {
                costs = SheltersModel.GetComponentsInChildren<ShelterModel>().First(x => x.ShelterID == ShelterModelID.Ship).CostBuy.Costs;
            }
            else if (!SheltersModel.ShelterModel.IsMaxLevel)
            {
                costs = SheltersModel.ShelterModel.CostUpgradeCurrent.Costs;
            }

            int i = 0;
            if(costs != null)
            {
                for(; i < costs.Length && i < View.ResourceCells.Length; i++)
                {
                    ShelterCost item = costs[i];
                    var itemData = ItemsDB.GetItem(item.Name);
                    var countCurrent = InventoryModel.ItemsContainer.GetItemsCount(itemData.Id) + HotBarModel.ItemsContainer.GetItemsCount(itemData.Id);

                    var cellData = GetCellData(item, itemData, countCurrent);
                    View.ResourceCells[i].gameObject.SetActive(true);
                    View.ResourceCells[i].SetData(cellData);
                }
            }

            for(; i < View.ResourceCells.Length; i++)
            {
                 View.ResourceCells[i].gameObject.SetActive(false);
            }

            if(ShelterUpgradeModel.NeedQuestItem)
            {
                View.QuestItemCells.gameObject.SetActive(true);
                View.QuestItemCells.SetData(GetQuestCellData());
            }
            else
            {
                View.QuestItemCells.gameObject.SetActive(false);
            }
        }

        private ResourceCellData GetCellData(ShelterCost itemCost, ItemData itemData, int countCurrent)
        {
            return new ResourceCellData
            {
                Icon = itemData.Icon,
                Message = countCurrent + "/" + itemCost.Count,
                IsActive = countCurrent >= itemCost.Count,
                ItemRarity = itemData.ItemRarity,
                IsComponent = itemData.Category == "Components",
            };
        }

        private ResourceCellData GetQuestCellData()
        {
            bool isCanBuy = ShelterUpgradeModel.CanBeUpgraded;
            return new ResourceCellData
            {
                Icon = QuestsModel.QuestItemData.ItemIcon,
                Message = isCanBuy? "1/1" : "0/1",
                IsActive = isCanBuy,
            };
        }

        private void OnCheckConnectionHandler() => UpdateGetButton();

        private void OnErrorRealtimeHandler(string obj) => UpdatePanels();
        private void OnReadyRealtimeHandler() => UpdatePanels();

        private void ShowGetFreeButton()
        {
            View.SetIsVisibleGetFreeButton(true);
            View.SetIsVisibleGetWatchButton(false);
            View.SetIsVisibleGetGoldButton(false);
        }

        private void ShowGetWatchButton()
        {
            View.SetIsVisibleGetFreeButton(false);
            View.SetIsVisibleGetWatchButton(true);
            View.SetIsVisibleGetGoldButton(false);
        }

        private void ShowGetGoldButton()
        {
            View.SetIsVisibleGetFreeButton(false);
            View.SetIsVisibleGetWatchButton(false);
            View.SetIsVisibleGetGoldButton(true);
        }

        private void OnChangeIsOpenGlobalObjectivesHandler()
        {
            if (ObjectivesViewModel.IsOpenGlobalObjectives)
            {
                ForceOpenGlobalObjectives();
            }
            else
            {
                ForceCloseGlobalObjectives();
            }
        }

        private void UpdateLayer2()
        {
            View.HeaderLayer2Object.sizeDelta = View.HeaderLayer0Object.sizeDelta;
            View.HeaderLayer2Object.position = View.HeaderLayer0Object.position;

            {
                var offsetMin = View.GlobalObjectivesLayer0Object.offsetMin;
                offsetMin.y -= View.Content0Layer0Object.rect.height;
                offsetMin.y -= View.Space0Layer0Object.rect.height;

                View.GlobalObjectivesLayer2Object.offsetMax = View.GlobalObjectivesLayer0Object.offsetMax;
                View.GlobalObjectivesLayer2Object.offsetMin = offsetMin;
            }

            {
                View.ObjectivesLayer2Object.offsetMin = View.Content0Layer0Object.offsetMin;
                View.ObjectivesLayer2Object.offsetMax = View.Content0Layer0Object.offsetMax;
                View.ObjectivesLayer2Object.position = View.Content0Layer0Object.position;
            }
        }

        private void OnClickGlobalObjectivesHandler() => ObjectivesViewModel.ChangeIsOpenGlobalObjectives();

        private void ForceOpenGlobalObjectives()
        {
            View.MoveDarkToLayer1();
            View.MoveGlobalObjectivesHeaderToLayer1();
            View.MoveGlobalObjectivesToLayer1();
            View.MoveTitleToLayer1();

            View.SetIsVisibleLayer1(true);
            View.SetIsVisibleLayer2(false);
        }

        private void ForceCloseGlobalObjectives()
        {
            View.MoveGlobalObjectivesHeaderToLayer0();
            View.MoveTitleToLayer0();

            View.SetIsVisibleLayer1(false);
            View.SetIsVisibleLayer2(false);
        }

        private void OnChangeIsHasTutorialObjectiveHandler()
        {
            if (!ObjectivesViewModel.IsHasAny)
            {
                UpdatePanels();
                UpdateObjectives();
            }
        }

        private void OnShowTutorialButtonsOnTop()
        {
           SetShowTutorialButtonsOnTop(true);
        }

        private void SetShowTutorialButtonsOnTop(bool show)
        {
            View.SetOverrideSortingObject1(show);
            View.SetOverrideSortingObject2(show);
            View.SetOverrideSortingObject3(show);
            View.SetOverrideSortingObject4(show);
            View.SetOverrideSortingObject5(show);
            View.SetOverrideSortingObject6(show);
        }

        private void UpdateObjectives()
        {
            HideObjectives();

            if (!ObjectivesViewModel.IsHasAny)
            {
                ShowObjectives();
            }
        }

        private void UpdatePanels() => SetVisibleTaskPanel(IsVisibleTaskPanel, IsReadyRealtime);

        private void OnGetGoldHandler() => PurchasesModel.Purchase(PurchaseID.GetDailyTaskGold, OnGetNextObjectivesResultHandler);

        private void OnGetWatchHandler() => PurchasesModel.Purchase(PurchaseID.GetDailyTaskWatch, OnGetNextObjectivesResultHandler);

        private void OnGetFreeHandler()
        {
            PlayerObjectivesModel.GetFree();
            NextObjectives();
        }

        private void OnGetNextObjectivesResultHandler(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                NextObjectives();
            }
            else
            {
                ViewsSystem.Show(ViewConfigID.Purchases);
            }
        }

        private void NextObjectives()
        {
            PlayerObjectivesModel.NextTier();
            UpdatePanels();
        }

        private void SetVisibleTaskPanel(bool isShowTask, bool isInet)
        {
            View.SetIsVisibleRemainingTimePanel(IsCanShowRemainingTimeTimer && isShowTask && !ObjectivesViewModel.IsHasAny && isInet);
            View.SetIsVisibleNoInternetTimePanel(isShowTask && !ObjectivesViewModel.IsHasAny && !isInet);

            View.SetIsVisibleTaskPanel(isShowTask);

            View.SetIsVisibleTimerPanel(!isShowTask && isInet);
            View.SetIsVisibleNoInternetContentPanel(!isShowTask && !isInet);
        }

        private void OnUpdateHandler()
        {
            var text = GetTimerText(TimeSpan.FromSeconds(PlayerObjectivesModel.ReminingTime));

            View.SetTextReminingTimeTask(text);
            View.SetTextReminingTimeTimer(text);
        }

        private string GetTimerText(TimeSpan time)
        {
            if (time.Hours == 0)
            {
                return string.Format(LocalizationModel.GetString(LocalizationKeyID.ObjectivesMenu_MinSec), time.Minutes, time.Seconds);
            }

            return string.Format(LocalizationModel.GetString(LocalizationKeyID.ObjectivesMenu_HourMin), time.Hours, time.Minutes);
        }

        private void ShowObjectives()
        {
            foreach (var item in PlayerObjectivesModel.Pool)
            {
                if (item.IsHasValue)
                {
                    ShowObjective(item);
                }
            }
        }

        private void HideObjectives()
        {
            foreach (var view in ObjectiveViews.Values)
            {
                ViewsSystem.Hide(view);
            }
            ObjectiveViews.Clear();
        }

        private void ShowObjective(PlayerObjectivesModel.ObjectiveDataInfo item)
        {
            var data = new ObjectiveViewControllerData(item.Id, ObjectivesModel.Get(item.Value),DailyTaskRespinSlotGold,DailyTaskRespinSlotGold.CoinCost);
            var view = ViewsSystem.Show(ViewConfigID.ObjectiveView, View.TasksContainer, data);
            ObjectiveViews[item.Id] = view;
        }

        private void OnCreateHandler(byte id)
        {
            bool validIndex = PlayerObjectivesModel.Pool.IndexIsValid(id);
            if(validIndex == true)
                ShowObjective(PlayerObjectivesModel.Pool[id]);
                
            UpdatePanels();
        }

        private void OnPostEndTaskHandler(byte id) => UpdatePanels();

        private void OnEndTaskHandler(byte id)
        {
            ViewsSystem.Hide(ObjectiveViews[id]);
            ObjectiveViews.Remove(id);
        }

        private void OnChangeCoinsHandler() => UpdateCoins();

        private void UpdateCoins() => View.SetTextCoins(CoinsModel.Coins.ToString());

        private void UpdateGoldButtonValue() => View.SetTextFreeGoldValueButton(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.GetDailyTaskGold).CoinCost.ToString());

        private void UpdateGetButton()
        {
            var isFree = false;
            var isWatch = !isFree && NetworkModel.IsHasConnection;
            var isGold = !isWatch;

            View.SetIsVisibleGetFreeButton(isFree);
            View.SetIsVisibleGetWatchButton(isWatch);
            View.SetIsVisibleGetGoldButton(isGold);
        }

        private void OnChangeBluePrintsHandler() => UpdateBluePrints();

        private void UpdateBluePrints() => View.SetTextBluePrints(BluePrintsModel.BluePrints.ToString());

        private void OnPickUpFirstHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            BluePrintsModel.Adjust(30);
            ObjectivesViewModel.PickUpTutorialStep1();

            View.SetIsVisibleObject1(false);
        }

        private void OnPickUpSecondHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            BluePrintsModel.Adjust(40);
            ObjectivesViewModel.PickUpTutorialStep2();

            View.SetIsVisibleObject2(false);
        }

        private void OnPickUpThirdHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            CoinsModel.Adjust(20);
            ObjectivesViewModel.PickUpTutorialStep3();

            View.SetIsVisibleObject3(false);
        }

        private void OnPickUpFourthHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            CoinsModel.Adjust(30);
            ObjectivesViewModel.PickUpTutorialStep4();

            View.SetIsVisibleObject4(false);
        }

        private void OnPickUpFifthHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            CoinsModel.Adjust(30);
            ObjectivesViewModel.PickUpTutorialStep5();

            View.SetIsVisibleObject5(false);
        }

        private void OnPickUpSixthHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            BluePrintsModel.Adjust(30);
            CoinsModel.Adjust(20);
            ObjectivesViewModel.PickUpTutorialStep6();

            View.SetIsVisibleObject6(false);
        }

        private void OnChangeLanguageHandler() => SetLocalization();

        private void OnCloseHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            ViewsSystem.Hide(View);
        }

        private void SetLocalization()
        {
            View.SetTextFirstTask(LocalizationModel.GetString(LocalizationKeyID.Tutorial_Task01_Start));
            View.SetTextSecondTask(LocalizationModel.GetString(LocalizationKeyID.Tutorial_Task02_Start));
            View.SetTextThirdTask(LocalizationModel.GetString(LocalizationKeyID.Tutorial_Task03_Start));
            View.SetTextFourthTask(LocalizationModel.GetString(LocalizationKeyID.Tutorial_Task04_Start));
            View.SetTextFifthTask(LocalizationModel.GetString(LocalizationKeyID.Tutorial_Task05_Start));
            View.SetTextSixthTask(LocalizationModel.GetString(LocalizationKeyID.Tutorial_Task06_Start));
            View.SetPickUpText(LocalizationModel.GetString(LocalizationKeyID.LootBoxMenu_TakeBtn));
            View.SetTitleText(LocalizationModel.GetString(LocalizationKeyID.ObjectivesMenu_Title));
            var textRemaininingDescription = LocalizationModel.GetString(LocalizationKeyID.ObjectivesMenu_NewMissionsTxt);
            View.SetTextRemainingDescription0(textRemaininingDescription);
            View.SetTextRemainingDescription1(textRemaininingDescription);
            View.SetTextGlobalObjectivesDescription(LocalizationModel.GetString(LocalizationKeyID.ObjectivesMenu_TextTemp));
            View.SetTextFreeButton(LocalizationModel.GetString(LocalizationKeyID.ObjectivesMenu_GetNowBtn));
            View.SetTextFreeWatchButton(LocalizationModel.GetString(LocalizationKeyID.ObjectivesMenu_GetNowBtn));
            View.SetTextFreeGoldButton(LocalizationModel.GetString(LocalizationKeyID.ObjectivesMenu_GetNowBtn));
            View.SetTextDescriptionQuest(QuestsModel.StageDescription);
            SetShipLevelText();
        }

        private void SetShipLevelText()
        {
            int shipLevel = 0;
            if (SheltersModel.ShelterActive != ShelterModelID.None)
            {
                shipLevel = SheltersModel.ShelterModel.Level;
            }
            View.SetShipLevelText(LocalizationModel.GetString(LocalizationKeyID.ShelterUpgradeMenu_Level) + " " + shipLevel);
        }
    }
}
