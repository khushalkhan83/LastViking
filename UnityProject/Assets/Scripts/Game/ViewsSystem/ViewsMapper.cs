using Core.Mapper;
using Game.Controllers;
using UnityEngine;

namespace Game.Views
{
    public class ViewsMapper : MonoBehaviour, IMap<ViewConfigID, ViewConfigData>
    {
        protected ViewConfigData[] Datas { get; } =
        {
            /* 0001*/
            new ViewConfigData(ViewConfigID.Touchpad, ViewID.Touchpad, typeof(TouchpadView), typeof(TouchpadViewController), LayerID.Base),
            /* 0002*/
            new ViewConfigData(ViewConfigID.Joystick, ViewID.Joystick, typeof(JoystickView), typeof(JoystickViewController), LayerID.Base),
            /* 0003*/
            new ViewConfigData(ViewConfigID.JumpButton, ViewID.JumpButton, typeof(JumpButtonView), typeof(JumpButtonViewController), LayerID.Base),
            /* 0004*/
            new ViewConfigData(ViewConfigID.RunButton, ViewID.RunButton, typeof(RunButtonView), typeof(RunButtonViewController), LayerID.Base),
            /* 0005*/
            new ViewConfigData(ViewConfigID.CraftInteract, ViewID.CraftInteract, typeof(CraftInteractView), typeof(CraftInteractViewController), LayerID.Base),
            /* 0006*/
            new ViewConfigData(ViewConfigID.InventoryInteract, ViewID.InventoryInteract, typeof(InventoryInteractView), typeof(InventoryInteractViewController), LayerID.Base),
            /* 0007*/
            new ViewConfigData(ViewConfigID.AimButton, ViewID.AimButton, typeof(AimButtonView), typeof(AimButtonViewController), LayerID.Base),
            /* 0008*/
            new ViewConfigData(ViewConfigID.AttackButton, ViewID.AttackButton, typeof(AttackButtonView), typeof(AttackButtonViewController), LayerID.Base),
            /* 0009*/
            new ViewConfigData(ViewConfigID.PlaceButton, ViewID.PlaceButton, typeof(PlaceButtonView), typeof(PlaceButtonViewController), LayerID.Base),
            /* 0010*/
            new ViewConfigData(ViewConfigID.DeathFader, ViewID.DeathFader, typeof(DeathFaderView), null, LayerID.Base),
            /* 0011*/
            new ViewConfigData(ViewConfigID.RotateButton, ViewID.RotateButton, typeof(RotateButtonView), typeof(RotateButtonViewController), LayerID.Base),
            /* 0012*/
            new ViewConfigData(ViewConfigID.DebugTime, ViewID.DebugTime, typeof(DebugTimeView), typeof(DebugTimeViewController), LayerID.Base),
            /* 0013*/
            new ViewConfigData(ViewConfigID.DebugTimeInteract, ViewID.DebugTimeInteract, typeof(DebugTimeInteractView), typeof(DebugTimeInteractViewController), LayerID.Base),
            /* 0014*/
            new ViewConfigData(ViewConfigID.InventoryPlayer, ViewID.InventoryPlayer, typeof(InventoryPlayerView), typeof(InventoryPlayerViewController), LayerID.Window),
            /* 0015*/
            new ViewConfigData(ViewConfigID.HotBar, ViewID.HotBar, typeof(HotBarView), typeof(HotBarViewController), LayerID.Base),
            /* 0016*/
            new ViewConfigData(ViewConfigID.DividePopup, ViewID.DividePopup, typeof(DividePopupView), null, LayerID.Popup),
            /* 0017*/
            new ViewConfigData(ViewConfigID.InventoryIsFillPopup, ViewID.InventoryIsFillPopup, typeof(InventoryIsFillPopupView), typeof(InventoryIsFillPopupViewController), LayerID.Popup),
            /* 0018*/
            new ViewConfigData(ViewConfigID.InventoryLoot, ViewID.InventoryLoot, typeof(InventoryLootView), typeof(InventoryLootViewController), LayerID.Window),
            /* 0019*/
            new ViewConfigData(ViewConfigID.Craft, ViewID.Craft, typeof(CraftView), typeof(CraftViewController), LayerID.Window),
            /* 0020*/
            new ViewConfigData(ViewConfigID.CraftCell, ViewID.CraftCell, typeof(CraftCellView), null),
            /* 0021*/
            new ViewConfigData(ViewConfigID.HealthBar, ViewID.HealthBar, typeof(HealthBarView), typeof(HealthBarViewController), LayerID.Base),
            /* 0022*/
            new ViewConfigData(ViewConfigID.WaterBar, ViewID.WaterBar, typeof(WaterBarView), typeof(WaterBarViewController), LayerID.Base),
            /* 0023*/
            new ViewConfigData(ViewConfigID.FoodBar, ViewID.FoodBar, typeof(FoodBarView), typeof(FoodBarViewController), LayerID.Base),
            /* 0024*/
            new ViewConfigData(ViewConfigID.StaminaBar, ViewID.StaminaBar, typeof(StaminaBarView), typeof(StaminaBarViewController), LayerID.Base),
            /* 0025*/
            new ViewConfigData(ViewConfigID.DotCrosshair, ViewID.DotCrosshair, typeof(DotCrosshairView), typeof(DotCrosshairViewController), LayerID.Base),
            /* 0026*/
            new ViewConfigData(ViewConfigID.OpenLootCursor, ViewID.OpenLootCursor, typeof(OpenLootCursorView), typeof(OpenLootCursorViewController), LayerID.Base),
            /* 0027*/
            new ViewConfigData(ViewConfigID.PickUpCursor, ViewID.PickUpCursor, typeof(PickUpCursorView), typeof(PickUpCursorViewController), LayerID.Base),
            /* 0028*/
            new ViewConfigData(ViewConfigID.CampFireCursor, ViewID.CampFireCursor, typeof(CampFireCursorView), typeof(CampFireCursorViewController), LayerID.Base),
            /* 0029*/
            new ViewConfigData(ViewConfigID.RestorableCursor, ViewID.RestorableCursor, typeof(RestorableCursorView), typeof(RestorableCursorViewController), LayerID.Base),
            /* 0030*/
            new ViewConfigData(ViewConfigID.SettingsButton, ViewID.SettingsButton, typeof(SettingsButtonView), typeof(SettingsButtonViewController), LayerID.Base),
            /* 0031*/
            new ViewConfigData(ViewConfigID.HouseAttackedInfo, ViewID.HouseAttackedInfo, typeof(HouseAttackedInfoView), typeof(HouseAttackedInfoViewController), LayerID.Base),
            /* 0032*/
            new ViewConfigData(ViewConfigID.ShelterHealthBar, ViewID.ShelterHealthBar, typeof(ShelterHealthBarView), typeof(ShelterHealthBarViewController), LayerID.Base),
            /* 0033*/
            new ViewConfigData(ViewConfigID.ShelterCursorBuy, ViewID.ShelterCursor, typeof(ShelterCursorBuyView), null, LayerID.Base),
            /* 0034*/
            new ViewConfigData(ViewConfigID.CampFire, ViewID.CampFire, typeof(CampFireView), typeof(CampFireViewController), LayerID.Window),
            /* 0035*/
            new ViewConfigData(ViewConfigID.FurnaceCursor, ViewID.FurnaceCursor, typeof(FurnaceCursorView), typeof(FurnaceCursorViewController), LayerID.Base),
            /* 0036*/
            new ViewConfigData(ViewConfigID.Furnace, ViewID.Furnace, typeof(FurnaceView), typeof(FurnaceViewController), LayerID.Window),
            /* 0037*/
            new ViewConfigData(ViewConfigID.Settings, ViewID.Settings, typeof(SettingsView), typeof(SettingsViewController), LayerID.Popup),
            /* 0038*/
            new ViewConfigData(ViewConfigID.ShelterPopup, ViewID.ShelterPopup, typeof(ShelterPopupView), typeof(ShelterPopupViewController), LayerID.Window),
            /* 0039*/
            new ViewConfigData(ViewConfigID.ResourceCost, ViewID.ResourceCost, typeof(ResourceCostView), null),
            /* 0040*/
            new ViewConfigData(ViewConfigID.ShelterCursorUpgrade, ViewID.ShelterCursorUpgrade, typeof(ShelterCursorUpgradeView), typeof(ShelterCursorUpgradeViewController), LayerID.Base),
            /* 0041*/
            new ViewConfigData(ViewConfigID.ShelterCursorActivate, ViewID.ShelterCursorActivate, typeof(ShelterCursorActivateView), typeof(ShelterCursorActivateViewController), LayerID.Base),
            /* 0042*/
            new ViewConfigData(ViewConfigID.Lose, ViewID.Lose, typeof(LoseView), typeof(LoseViewController), LayerID.Popup),
            /* 0043*/
            new ViewConfigData(ViewConfigID.Blood, ViewID.Blood, typeof(BloodEffectView), typeof(BloodEffectViewController), LayerID.First),
            /* 0044*/
            new ViewConfigData(ViewConfigID.AddStatEffect, ViewID.AddStatEffect, typeof(AddStatEffectView), null),
            /* 0045*/
            new ViewConfigData(ViewConfigID.AddStatsEffect, ViewID.AddStatsEffect, typeof(AddStatsEffectView), null),
            /* 0046*/
            new ViewConfigData(ViewConfigID.DestroyableCursor, ViewID.DestroyableCursor, typeof(DestroyableCursorView), typeof(DestroyableCursorViewController), LayerID.Base),
            /* 0047*/
            new ViewConfigData(ViewConfigID.CreatureHealth, ViewID.CreatureHealth, typeof(CreatureHealthView), null, LayerID.Base),
            /* 0048*/
            new ViewConfigData(ViewConfigID.Purchases, ViewID.Purchases, typeof(PurchasesView), typeof(PurchasesViewController), LayerID.Popup),
            /* 0049*/
            new ViewConfigData(ViewConfigID.CoinsInfo, ViewID.CoinsInfo, typeof(CoinsInfoView), typeof(CoinsInfoViewController), LayerID.Base),
            /* 0050*/
            new ViewConfigData(ViewConfigID.VersionControl, ViewID.VersionControl, typeof(VersionControlPopupView), typeof(VersionControlViewController), LayerID.Popup),
            /* 0051*/
            new ViewConfigData(ViewConfigID.DiscordButton, ViewID.DiscordButton, typeof(DiscordButtonView), typeof(DiscordButtonViewController), LayerID.Popup),
            /* 0052*/
            new ViewConfigData(ViewConfigID.ReceiveDamage, ViewID.ReceiveDamage, typeof(ReceiveDamageView), null, LayerID.Popup),
            /* 0053*/
            new ViewConfigData(ViewConfigID.AddCellGold, ViewID.AddCellGold, typeof(AddCellGoldView), null),
            /* 0054*/
            new ViewConfigData(ViewConfigID.AddCellWatch, ViewID.AddCellWatch, typeof(AddCellWatchView), null),
            /* 0055*/
            new ViewConfigData(ViewConfigID.UnlockSlotsGold, ViewID.UnlockSlotsGold, typeof(UnlockSlotsGoldView), null),
            /* 0056*/
            new ViewConfigData(ViewConfigID.UnlockSlotsWatch, ViewID.UnlockSlotsWatch, typeof(UnlockSlotsWatchView), null),
            /* 0057*/
            new ViewConfigData(ViewConfigID.BoostGoldButton, ViewID.BoostGoldButton, typeof(BoostButtonGoldView), null),
            /* 0058*/
            new ViewConfigData(ViewConfigID.BoostWatchButton, ViewID.BoostWatchButton, typeof(BoostButtonWatchView), null),
            /* 0059*/
            new ViewConfigData(ViewConfigID.TombCursor, ViewID.TombCursor, typeof(TombCursorView), typeof(TombCursorViewController), LayerID.Base),
            /* 0060*/
            new ViewConfigData(ViewConfigID.Black, ViewID.Black, typeof(BlackView), null, LayerID.Over),
            /* 0061*/
            new ViewConfigData(ViewConfigID.TombPopup, ViewID.TombPopup, typeof(TombPopupView), typeof(TombPopupViewController), LayerID.Window),
            /* 0062*/
            new ViewConfigData(ViewConfigID.RestartGamePopup, ViewID.RestartGamePopup, typeof(RestartGamePopupView), typeof(RestartGamePopupViewController), LayerID.Popup),
            /* 0063*/
            new ViewConfigData(ViewConfigID.PickUpTimeDelayCursor, ViewID.PickUpTimeDelayCursor, typeof(PickUpTimeDelayCursorView), typeof(PickUpTimeDelayCursorViewController), LayerID.Base),
            /* 0064*/
            new ViewConfigData(ViewConfigID.PickUpTimeDelayTimerCursor, ViewID.PickUpTimeDelayTimerCursor, typeof(PickUpTimeDelayTimerCursorView), typeof(PickUpTimeDelayTimerCursorViewController), LayerID.Base),
            /* 0065*/
            new ViewConfigData(ViewConfigID.LoomCursor, ViewID.LoomCursor, typeof(LoomCursorView), typeof(LoomCursorViewController), LayerID.Base),
            /* 0066*/
            new ViewConfigData(ViewConfigID.Loom, ViewID.Loom, typeof(LoomView), typeof(LoomViewController), LayerID.Window),
            /* 0067*/
            new ViewConfigData(ViewConfigID.MessageCraftResource, ViewID.MessageCraftResource, typeof(MessageCraftResourceView), null),
            /* 0068*/
            new ViewConfigData(ViewConfigID.MessageAppendResource, ViewID.MessageAppendResource, typeof(MessageAppendResourceView), null),
            /* 0069*/
            new ViewConfigData(ViewConfigID.MessageAttentionResource, ViewID.MessageAttentionResource, typeof(MessageAttentionResourceView), null),
            /* 0070*/
            new ViewConfigData(ViewConfigID.MessageDestroyResource, ViewID.MessageDestroyResource, typeof(MessageDestroyResourceView), null),
            /* 0071*/
            new ViewConfigData(ViewConfigID.ResourceMessages, ViewID.ResourceMessages, typeof(ResourceMessagesView), typeof(ResourceMessagesViewController), LayerID.Base),
            /* 0072*/
            new ViewConfigData(ViewConfigID.MessagesFloatingText, ViewID.MessagesFloatingText, typeof(MessagesFloatingTextView), null),
            /* 0073*/
            new ViewConfigData(ViewConfigID.MineableCursor, ViewID.MineableCursor, typeof(MineableCursorView), typeof(MineableCursorViewController), LayerID.Base),
            /* 0074*/
            new ViewConfigData(ViewConfigID.RatePopup, ViewID.RatePopup, typeof(RateMeView), typeof(RateMeViewController), LayerID.Popup),
            /* 0075*/
            new ViewConfigData(ViewConfigID.GameSparksStart, ViewID.GameSparksStart, typeof(GameSparksStartView), typeof(GameSparksStartViewController), LayerID.Popup),
            /* 0076*/
            new ViewConfigData(ViewConfigID.GameSparksAccount, ViewID.GameSparksAccount, typeof(GameSparksAccountView), typeof(GameSparksAccountViewController), LayerID.Popup),
            /* 0077*/
            new ViewConfigData(ViewConfigID.MessageAppendCoin, ViewID.MessageAppendCoin, typeof(MessageAppendCoinView), null),
            /* 0078*/
            new ViewConfigData(ViewConfigID.CampFireHealthCursor, ViewID.CampFireHealthCursor, typeof(CampFireHealthCursorView), typeof(CampFireHealthCursorViewController), LayerID.Base),
            /* 0079*/
            new ViewConfigData(ViewConfigID.FurnaceHealthCursor, ViewID.FurnaceHealthCursor, typeof(FurnaceHealthCursorView), typeof(FurnaceHealthCursorViewController), LayerID.Base),
            /* 0080*/
            new ViewConfigData(ViewConfigID.LoomHealthCursor, ViewID.LoomHealthCursor, typeof(LoomHealthCursorView), typeof(LoomHealthCursorViewController), LayerID.Base),
            /* 0081*/
            new ViewConfigData(ViewConfigID.GodMode, ViewID.GodMode, typeof(GodModeView), null, LayerID.First),
            /* 0082*/
            new ViewConfigData(ViewConfigID.PickUpTimeDelayHealthCursor, ViewID.PickUpTimeDelayHealthCursor, typeof(PickUpTimeDelayHealthCursorView), typeof(PickUpTimeDelayHealthCursorViewController), LayerID.Base),
            /* 0083*/
            new ViewConfigData(ViewConfigID.PickUpTimeDelayTimerHealthCursor, ViewID.PickUpTimeDelayTimerHealthCursor, typeof(PickUpTimeDelayTimerHealthCursorView), typeof(PickUpTimeDelayTimerHealthCursorViewController), LayerID.Base),
            /* 0084*/
            new ViewConfigData(ViewConfigID.DeathPlayer, ViewID.DeathPlayer, typeof(DeathPlayerView), typeof(DeathPlayerViewController), LayerID.Popup),
            /* 0085*/
            new ViewConfigData(ViewConfigID.Repair, ViewID.Repair, typeof(RepairView), typeof(RepairViewController), LayerID.Popup),
            /* 0086*/
            new ViewConfigData(ViewConfigID.ColdStatus, ViewID.ColdStatus, typeof(ColdStatusView), typeof(ColdStatusViewController), LayerID.Base),
            /* 0087*/
            new ViewConfigData(ViewConfigID.InfoPanel, ViewID.InfoPanel, typeof(InfoPanelView), typeof(InfoPanelViewController)),
            /* 0088*/
            new ViewConfigData(ViewConfigID.InfoPanelDefence, ViewID.InfoPanelDefence, typeof(InfoPanelDefenceView), typeof(InfoPanelDefenceViewController)),
            /* 0089*/
            new ViewConfigData(ViewConfigID.InfoPanelFood, ViewID.InfoPanelFood, typeof(InfoPanelFoodView), typeof(InfoPanelFoodViewController)),
            /* 0090*/
            new ViewConfigData(ViewConfigID.InfoPanelMedicine, ViewID.InfoPanelMedicine, typeof(InfoPanelMedicineView), typeof(InfoPanelMedicineViewController)),
            /* 0091*/
            new ViewConfigData(ViewConfigID.InfoPanelTool, ViewID.InfoPanelTool, typeof(InfoPanelToolView), typeof(InfoPanelToolViewController)),
            /* 0092*/
            new ViewConfigData(ViewConfigID.InfoPanelWeapon, ViewID.InfoPanelWeapon, typeof(InfoPanelWeaponView), typeof(InfoPanelWeaponViewController)),
            /* 0093*/
            new ViewConfigData(ViewConfigID.PlayerStatus, ViewID.PlayerStatus, typeof(PlayerStatusView), typeof(PlayerStatusViewController), LayerID.Base),
            /* 0094*/
            new ViewConfigData(ViewConfigID.ThirstStatus, ViewID.ThirstStatus, typeof(ThirstStatusView), typeof(ThirstStatusViewController), LayerID.Base),
            /* 0095*/
            new ViewConfigData(ViewConfigID.HungerStatus, ViewID.HungerStatus, typeof(HungerStatusView), typeof(HungerStatusViewController), LayerID.Base),
            /* 0096*/
            new ViewConfigData(ViewConfigID.NotificationContainer, ViewID.NotificationContainer, typeof(NotificationContainerView), typeof(NotificationContainerViewController), LayerID.Over),
            /* 0097*/
            new ViewConfigData(ViewConfigID.LoadingCircle, ViewID.LoadingCircle, typeof(LoadingCircleView), typeof(LoadingCircleViewController), LayerID.Popup),
            /* 0098*/
            new ViewConfigData(ViewConfigID.ObjectiveAttackProcess, ViewID.ObjectiveAttackProcess, typeof(ObjectiveAttackProcessView), typeof(ObjectiveAttackProcessViewController), LayerID.Base),
            /* 0099*/
            new ViewConfigData(ViewConfigID.ObjectiveTimeDelay, ViewID.ObjectiveTimeDelay, typeof(ObjectiveTimeDelayView), typeof(ObjectiveTimeDelayViewController)),
            /* 0100*/
            new ViewConfigData(ViewConfigID.ObjectiveButton, ViewID.ObjectiveButton, typeof(ObjectivesButtonView), typeof(ObjectivesButtonViewController), LayerID.Base),
            /* 0101*/
            new ViewConfigData(ViewConfigID.AutoSaveInfo, ViewID.AutoSaveInfo, typeof(AutoSaveInfoView), typeof(AutoSaveInfoViewController), LayerID.Popup),
            /* 0102*/
            new ViewConfigData(ViewConfigID.TutorialDark, ViewID.TutorialDark, typeof(TutorialDarkView), typeof(TutorialDarkViewController), LayerID.Window),
            /* 0103*/
            new ViewConfigData(ViewConfigID.Objectives, ViewID.Objectives, typeof(ObjectivesView), typeof(ObjectivesViewController), LayerID.Window),
            /* 0104*/
            new ViewConfigData(ViewConfigID.TutorialDeathPlayer, ViewID.TutorialDeathPlayer, typeof(TutorialDeathPlayerView), typeof(TutorialDeathPlayerViewController), LayerID.Popup),
            /* 0105*/
            new ViewConfigData(ViewConfigID.NoSpaceForSavePopup, ViewID.NoSpaceForSavePopup, typeof(NoSpaceForSavePopupView), typeof(NoSpaceForSaveGamePopupViewController), LayerID.Popup),
            /* 0106*/
            new ViewConfigData(ViewConfigID.SpawnWaveInfo, ViewID.ObjectiveInfo, typeof(ObjectiveInfoView), typeof(ObjectiveInfoViewController)),
            /* 0107*/
            new ViewConfigData(ViewConfigID.TutorialObjective, ViewID.ObjectiveProcess, typeof(ObjectiveProcessView), typeof(TutorialObjectiveProcessViewController)),
            /* 0108*/
            new ViewConfigData(ViewConfigID.MessageAppendBlueprint, ViewID.MessageAppendBlueprint, typeof(MessageAppendBlueprintView), null),
            /* 0109*/
            new ViewConfigData(ViewConfigID.TutorialCompleted, ViewID.TutorialComplete, typeof(TutorialCompleteView), typeof(TutorialCompletedController)),
            /* 0110*/
            new ViewConfigData(ViewConfigID.Bleeding, ViewID.Bleeding, typeof(BleedingView), typeof(BleedingViewController), LayerID.Base),
            /* 0111*/
            new ViewConfigData(ViewConfigID.Poisoning, ViewID.Poisoning, typeof(PoisoningView), typeof(PoisoningViewController), LayerID.Base),
            /* 0112*/
            new ViewConfigData(ViewConfigID.AttackSessionCompleted, ViewID.AttackSessionCompleted, typeof(AttackSessionCompleteView), typeof(AttackSessionCompletedController)),
            /* 0113*/
            new ViewConfigData(ViewConfigID.AttackDelayStatus, ViewID.AttackDelayStatus, typeof(AttackDelayStatusView), typeof(AttackDelayStatusViewController), LayerID.Base),
            /* 0114*/
            new ViewConfigData(ViewConfigID.VersionInfo, ViewID.VersionInfo, typeof(VersionInfoView), typeof(VersionInfoViewController), LayerID.Popup),
            /* 0115*/
            new ViewConfigData(ViewConfigID.SaveInfo, ViewID.SaveInfo, typeof(SaveInfoView), typeof(SaveInfoViewController), LayerID.Popup),
            /* 0116*/
            new ViewConfigData(ViewConfigID.ObjectiveView, ViewID.ObjectiveView, typeof(ObjectiveView), typeof(ObjectiveViewController)),
            /* 117*/
            new ViewConfigData(ViewConfigID.NotificationObjectiveView, ViewID.ObjectiveProcess, typeof(ObjectiveProcessView), typeof(NotificationObjectiveViewController)),
            /* 118*/
            new ViewConfigData(ViewConfigID.OpenLootHealthCursor, ViewID.OpenLootHealthCursor, typeof(OpenLootHealthCursorView), typeof(OpenLootCursorHealthViewController), LayerID.Base),
            /* 119*/
            new ViewConfigData(ViewConfigID.OpenDoorCursor, ViewID.OpenDoorCursor, typeof(OpenDoorCursorView), typeof(OpenDoorCursorViewController), LayerID.Base),
            /* 120*/
            new ViewConfigData(ViewConfigID.OpenDoorHealthCursor, ViewID.OpenDoorHealthCursor, typeof(OpenDoorHealthCursorView), typeof(OpenDoorHealthCursorViewController), LayerID.Base),
            /* 121*/
            new ViewConfigData(ViewConfigID.HotBarRepairingContainerView, ViewID.HotBarRepairingContainerView, typeof(HotBarRepairingContainerView), typeof(HotBarRepairingContainerViewController), LayerID.Base),
            /* 122*/
            new ViewConfigData(ViewConfigID.HotRepairingView, ViewID.HotRepairingView, typeof(HotRepairingView), null),
            /* 123*/
            new ViewConfigData(ViewConfigID.FishingInit, ViewID.FishingInit, typeof(FishingInitView), typeof(FishingInitViewController), LayerID.Base),
            /* 124*/
            new ViewConfigData(ViewConfigID.FishingBiting, ViewID.FishingBiting, typeof(FishingBitingView), typeof(FishingBitingViewController), LayerID.Base),
            /* 125*/
            new ViewConfigData(ViewConfigID.FishingPulling, ViewID.FishingPulling, typeof(FishingPullView), typeof(FishingPullViewController), LayerID.Base),
            /* 126*/
            new ViewConfigData(ViewConfigID.MiniGameExit, ViewID.MiniGameExit, typeof(MiniGameExitView), typeof(MiniGameExitViewController), LayerID.Base),
            /* 126*/
            new ViewConfigData(ViewConfigID.Tokens, ViewID.Tokens, typeof(TokensView), typeof(TokensViewController), LayerID.Base),
            /* 128*/
            new ViewConfigData(ViewConfigID.EventObjectiveView, ViewID.ObjectiveProcess, typeof(ObjectiveProcessView), typeof(EventObjectiveProcessViewController)),
            /* 129*/
            new ViewConfigData(ViewConfigID.StarterPackPopupView, ViewID.StarterPackPopupView, typeof(StarterPackPopupView), typeof(StarterPackPopupViewController), LayerID.Popup),
            /* 130*/
            new ViewConfigData(ViewConfigID.StarterPackInsidePurchases, ViewID.StarterPackView, typeof(StarterPackView), typeof(StarterPackPopupViewController), LayerID.Popup),
            /* 131*/
            new ViewConfigData(ViewConfigID.StarterPackIconView, ViewID.StarterPackIconView, typeof(StarterPackIconView), typeof(StarterPackIconViewController), LayerID.Base),
            /* 132*/
            new ViewConfigData(ViewConfigID.OpenCoastLootCursor, ViewID.OpenTreasureLootCursor, typeof(OpenTreasureLootCursorView), typeof(OpenCoastLootCursorViewController), LayerID.Base),
            /* 133*/
            new ViewConfigData(ViewConfigID.CoastLootPopup, ViewID.TreasureLootPopup, typeof(TreasureLootPopupView), typeof(CoastLootPopupViewController), LayerID.Window),
            /* 134*/
            new ViewConfigData(ViewConfigID.TreasureHuntInit, ViewID.TreasureHuntInit, typeof(TrasureHuntInitView), typeof(TrasureHuntInitViewController), LayerID.Base),
            /* 135*/
            new ViewConfigData(ViewConfigID.TreasureShovel, ViewID.TreasureShovel, typeof(TreasureHuntShovelIndicatorView),null, LayerID.Base),
            /* 136*/
            new ViewConfigData(ViewConfigID.InternerError, ViewID.InternerError, typeof(InternetErrorView),null, LayerID.Base),
            /* 137*/
            new ViewConfigData(ViewConfigID.HuntDebug, ViewID.HuntDebug, typeof(TreasureHuntDebugView),null, LayerID.Base),
            /* 0138*/
            new ViewConfigData(ViewConfigID.ActivateObjectsCursor, ViewID.ActivateObjectsCursor, typeof(ActivateObjectsCursorView), typeof(ActivateObjectsCursorViewController), LayerID.Base),
            /* 0139*/
            new ViewConfigData(ViewConfigID.ScenesTransition, ViewID.ScenesTransition, typeof(EnvironmentLoadingView), typeof(EnvironmentLoadingViewController), LayerID.Over),
            /* 0140*/
            new ViewConfigData(ViewConfigID.ProgressCursor, ViewID.ProgressCursor, typeof(ProgressCursorView), typeof(ProgressCursorViewController), LayerID.Base),
            /* 141*/
            new ViewConfigData(ViewConfigID.TreasureLootTimerCursor, ViewID.TreasureLootTimerCursor, typeof(TreasureLootTimerCursorView), typeof(TreasureLootTimerCursorViewController), LayerID.Base),
            /* 142*/
            new ViewConfigData(ViewConfigID.DeadManLootPopup, ViewID.TreasureLootPopup, typeof(TreasureLootPopupView), typeof(DeadManLootPopupViewController), LayerID.Window),
            /* 143*/
            new ViewConfigData(ViewConfigID.DeadManLootCursor, ViewID.OpenTreasureLootCursor, typeof(OpenTreasureLootCursorView), typeof(OpenDeadManLootCursorViewController), LayerID.Base),
            /* 144*/
            new ViewConfigData(ViewConfigID.TreasureHuntLootPopup, ViewID.TreasureLootPopup, typeof(TreasureLootPopupView), typeof(TreasureHuntLootPopupViewController), LayerID.Base),
            /* 0145*/
            new ViewConfigData(ViewConfigID.BombDestroyCursor, ViewID.BombDestroyCursor, typeof(BombDestroyCursorView), typeof(BombDestroyCursorViewController), LayerID.Base),
            /* 0146*/
            new ViewConfigData(ViewConfigID.ToggleInteractableCursor, ViewID.ToggleInteractableCursor, typeof(ToggleInteractableCursorView), typeof(ToggleInteractableCursorViewController), LayerID.Base),
            /* 0147*/
            new ViewConfigData(ViewConfigID.BuildingSwitchButton, ViewID.BuildingSwitchButton, typeof(BuildingSwitchButtonView), typeof(BuildingSwitchButtonViewController), LayerID.Base),
            /* 0148*/
            new ViewConfigData(ViewConfigID.BuildingHotBar, ViewID.BuildingHotBar, typeof(BuildingHotBarView), typeof(BuildingHotBarViewController), LayerID.Base),
            /* 0149*/
            new ViewConfigData(ViewConfigID.PlacePartButton, ViewID.PlaceButton, typeof(PlaceButtonView), typeof(PlacePartButtonViewController), LayerID.Base),
            /* 0150*/
            new ViewConfigData(ViewConfigID.RotatePartButton, ViewID.RotateButton, typeof(RotateButtonView), typeof(RotatePartButtonViewController), LayerID.Base),
            /* 0151*/
            new ViewConfigData(ViewConfigID.ConstructionInfoPopupView, ViewID.ConstructionInfoPopupView, typeof(ConstructionInfoPopupView), typeof(ConstructionInfoPopupViewController), LayerID.Window), // TODO: remove unused setting
            /* 152*/
            new ViewConfigData(ViewConfigID.NoSpaceForSaveExitConfig, ViewID.NoSpaceForSaveExit, typeof(NoSpaceForSaveExitPopupView), typeof(NoSpaceForSaveGameExitPopupViewController), LayerID.Popup),
            /* 153*/
            new ViewConfigData(ViewConfigID.ProtectShelterPopupConfig, ViewID.ProtectShelterPopup, typeof(ProtectShelterPopupView), typeof(ProtectShelterPopupViewController), LayerID.Popup),
            /* 154*/
            new ViewConfigData(ViewConfigID.ShelterAttackModeConfig, ViewID.ObjectiveAttackProcess, typeof(ObjectiveAttackProcessView), typeof(ShelterAttackModeViewController), LayerID.Base),
            /* 155*/
            new ViewConfigData(ViewConfigID.QuestObjective, ViewID.QuestObjective, typeof(QuestObjectiveView), typeof(QuestObjectiveViewController), LayerID.Base),
            /* 156*/
            new ViewConfigData(ViewConfigID.AttackWarningPopup, ViewID.AttackWarningPopup, typeof(AttackWarningPopupView), null, LayerID.Popup),
            /* 157*/
            new ViewConfigData(ViewConfigID.TutorialObjectivesDarkScreenConfig, ViewID.TutorialObjectivesDarkScreen, typeof(TutorialObjectivesDarkScreenView), typeof(TutorialObjectivesDarkScreenViewController), LayerID.Window),
            /* 158*/
            new ViewConfigData(ViewConfigID.RequiredItemsCursorViewConfig, ViewID.RequiredItems, typeof(ItemsRelatedInteractableCursorView), typeof(ItemsRelatedInteractableCursorViewController), LayerID.Base),
            /* 159*/
            new ViewConfigData(ViewConfigID.OpenConstructionButtonConfig, ViewID.ConstructionInfoPopupView, typeof(ConstructionInfoPopupView), typeof(OpenConstructionViewController), LayerID.Window),
            /* 160*/
            new ViewConfigData(ViewConfigID.RecivedQuestItemConfig, ViewID.SpecialMessage, typeof(SpecialMessageView), typeof(RecivedQuestItemMessageViewController), LayerID.Base),
            /* 161*/
            new ViewConfigData(ViewConfigID.ConstructionTutorialConfig, ViewID.ConstructionTutorial, typeof(ConstructionTutorialView), typeof(ConstructionTutorialViewController), LayerID.Window),
            /* 162*/
            new ViewConfigData(ViewConfigID.TeleporHomeConfig, ViewID.TeleporHome, typeof(TeleporHomeView), typeof(TeleporHomeViewController), LayerID.Popup),
            /* 163*/
            new ViewConfigData(ViewConfigID.QuestRequiredItemsCursorViewConfig, ViewID.RequiredItems, typeof(ItemsRelatedInteractableCursorView), typeof(QuestItemRelatedInteractableCursorViewController), LayerID.Base),
            /* 164*/
            new ViewConfigData(ViewConfigID.TutorialFoundationViewConfig, ViewID.TutorialFoundationView, typeof(TutorialFoundationView), typeof(TutorialFoundationViewController), LayerID.Popup),
            /* 165*/
            new ViewConfigData(ViewConfigID.QuestionPopupConfig, ViewID.QuestionPopup, typeof(QuestionPopupView), null, LayerID.Popup),
            /* 166*/
            new ViewConfigData(ViewConfigID.TutorialSimpleDarkConfig, ViewID.TutorialSimpleDark, typeof(TutorialSimpleDarkView), typeof(TutorialSimpleDarkViewController), LayerID.Window),
            /* 167*/
            new ViewConfigData(ViewConfigID.LowPerformancePopupConfig, ViewID.QuestionPopup, typeof(QuestionPopupView), typeof(LowPerformanceViewController), LayerID.Popup),
            /* 168*/
            new ViewConfigData(ViewConfigID.UserIDQuestionPopupConfig, ViewID.QuestionPopup, typeof(QuestionPopupView), typeof(UserIDQuestionPopupViewController), LayerID.Popup),
            /* 169*/
            new ViewConfigData(ViewConfigID.FishHealthConfig, ViewID.FishHealth, typeof(FishHealthView), typeof(FishHealthViewController), LayerID.Base),
            /* 170*/
            new ViewConfigData(ViewConfigID.FishingLinkConfig, ViewID.FishingLink, typeof(FishingLinkView), typeof(FishingLinkViewController), LayerID.Popup),
            /* 171*/
            new ViewConfigData(ViewConfigID.ReplaceButtonConfig, ViewID.ReplaceButton, typeof(ReplaceButtonView), typeof(ReplaceButtonViewController), LayerID.Base),
            /* 172*/
            new ViewConfigData(ViewConfigID.PlaceReplacedButtonConfig, ViewID.PlaceButton, typeof(PlaceButtonView), typeof(PlaceReplacedButtonViewController), LayerID.Base),
            /* 173*/
            new ViewConfigData(ViewConfigID.EncountersDebugConfig, ViewID.EncountersDebug, typeof(EncountersDebugView), typeof(EncountersDebugViewController), LayerID.Over),
            /* 174*/
            new ViewConfigData(ViewConfigID.ActivitiesLogConfig, ViewID.ActivitiesLog, typeof(ActivitiesLogView), typeof(ActivitiesLogViewController), LayerID.Popup),
            /* 175*/
            new ViewConfigData(ViewConfigID.ActivitiesLogButtonConfig, ViewID.ActivitiesLogButton, typeof(ActivitiesLogButtonView), typeof(ActivitiesLogButtonViewController), LayerID.Base),
            /* 176*/
            new ViewConfigData(ViewConfigID.InfoPanelEquipmentConfig, ViewID.InfoPanelEquipment, typeof(InfoPanelEquipmentView), typeof(InfoPanelEquipmentViewController)),
            /* 177*/
            new ViewConfigData(ViewConfigID.HouseBuildingCursorConfig, ViewID.HouseBuildingCursor, typeof(HouseBuildingCursorView), typeof(HouseBuildingCursorViewController), LayerID.Base),
            /* 178*/
            new ViewConfigData(ViewConfigID.HouseBuildingConfig, ViewID.HouseBuilding, typeof(HouseBuildingView), typeof(HouseBuildingViewController), LayerID.Window),
            /* 179*/
            new ViewConfigData(ViewConfigID.BuildingTokenConfig, ViewID.BuildingToken, typeof(BuildingTokenView), null),
            /* 180*/
            new ViewConfigData(ViewConfigID.WarmBarViewConfig, ViewID.WarmBar, typeof(WarmBarView), typeof(WarmBarViewController), LayerID.Base),
            /* 181*/
            new ViewConfigData(ViewConfigID.CitizensInfoViewConfig, ViewID.CitizensInfo, typeof(CitizensInfoView), typeof(CitizensInfoViewController), LayerID.Base),
            /* 182*/
            new ViewConfigData(ViewConfigID.ZoneMessageConfig, ViewID.ZoneMessage, typeof(ZoneMessageView), typeof(ZoneMessageViewController), LayerID.Base),
            /* 183*/
            new ViewConfigData(ViewConfigID.TeleportEffectConfig, ViewID.TeleportEffect, typeof(TeleportEffectView), typeof(TeleportEffectViewController), LayerID.Base),
            /* 184*/
            new ViewConfigData(ViewConfigID.DragItemConfig, ViewID.DragItem, typeof(DragItemView), typeof(DragItemViewController), LayerID.Over),
            /* 185*/
            new ViewConfigData(ViewConfigID.TaskViewConfig, ViewID.TaskView, typeof(TaskView), typeof(TaskViewController), LayerID.Over),
            /* 186*/
            new ViewConfigData(ViewConfigID.LearnInputConfig, ViewID.LearnInput, typeof(LearnInputView), typeof(LearnInputViewController), LayerID.Window),
            ///CODE_GENERATION_VIEWS_MAPPER
        };

        public ViewConfigData this[ViewConfigID key] => Datas[(int)key - 1];
    }
}
