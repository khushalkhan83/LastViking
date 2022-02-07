using Core;
using Core.Controllers;
using Extensions;
using Game.Interactables;
using Game.Models;
using Game.Providers;
using Game.QuestSystem.Map.Extra.Environment;
using Game.Views;
using System;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    /*
     * Убрать лишние проверки
     * Оптимизировать на отдельные курсоры (чтоб логика курсоров построек с уничтожением не была в RestorableCursor)
     * 
     * (Сделать один курсор для построек с уничтожением ?)
     */

    public class CursorController : ICursorController, IController
    {
        [Inject] public ShelterModelsProvider ShelterModelsProvider { get; private set; }
        [Inject] public MineableObjectsModel MineableObjectsModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public SheltersModel SheltersModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public LoseViewModel LoseViewModel { get; private set; }
        [Inject] public CoastLootChestModel CoastLootChestModel { get; private set; }
        [Inject] public DeadManLootChestModel DeadManLootChestModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public InputModel InputModel { get; private set; }
        [Inject] public ViewsStateModel ViewsStateModel { get; private set; }

        private GameObject GameObjectLast { get; set; }
        private GameObject GameObject { get; set; }
        private CursorViewBase CursorView { get; set; }

        private Action[] Cursors { get; set; }


        // [Refactor].
        private ResourceMessagesView resourceMessagesView;
        private ResourceMessagesView ResourceMessagesView
        {
            get
            {
                if(resourceMessagesView == null) 
                    resourceMessagesView = GetResourceMessagesView();
                return resourceMessagesView;
            }
        }
        private ResourceMessagesView GetResourceMessagesView()
        {
            ResourceMessagesView resourceMessagesView = null;
                if (ViewsSystem.ActiveViews.TryGetValue(ViewConfigID.ResourceMessages, out var views)) {
                    foreach (var view in views) {
                        resourceMessagesView = view as ResourceMessagesView;
                    }
                }

            return resourceMessagesView;
        }
        // [Refactor]

        void IController.Enable()
        {
            Cursors = new Action[]
            {
                DefaultCorsor,
                ItemPickupCursor,
                TableUpgradeCurcor,
                RestorableCursor,
                DestroyableCursor,
                LootObjectCursor,
                TombCursor,
                MinableCursor,
                FurnaceCursor,
                LoomCursor,
                PickupTimeDelayCursor,
                OpenDoorCursor,
                CoastLootCursor,
                ActivateCursor,
                ProgressCursor,
                DeadManLootCursor,
                BombDestroyCursor,
                ToggleInteractableCursor,
                ItemsRelatedInteractableCursor,
                QuestItemRelatedInteractableCursor,
                HouseBuildingCursor,
            };
            PlayerEventHandler.RaycastData.OnChange += OnChangeRaycastDataHandler;
            ViewsSystem.OnHideAll += OnHideAllHandler;
            OnChangeRaycastDataHandler();

            LoseViewModel.OnRessurect += OnRessurectHandler;

            InputModel.OnInput.AddListener(PlayerActions.Interact,OnInteractionInput);
            InputModel.OnInput.AddListener(PlayerActions.InteractAlternative,OnInteractionAlternativeInput);
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            PlayerEventHandler.RaycastData.OnChange -= OnChangeRaycastDataHandler;
            ViewsSystem.OnHideAll -= OnHideAllHandler;
            Hide();

            LoseViewModel.OnRessurect -= OnRessurectHandler;

            InputModel.OnInput.RemoveListener(PlayerActions.Interact,OnInteractionInput);
            InputModel.OnInput.RemoveListener(PlayerActions.InteractAlternative,OnInteractionAlternativeInput);
        }

        private void OnInteractionInput()
        {
            if(ViewsStateModel.WindowOpened()) return;
            if(CursorView == null) return;

            CursorView.Interact();
        }
        private void OnInteractionAlternativeInput()
        {
            if(ViewsStateModel.WindowOpened()) return;
            if(CursorView == null) return;

            CursorView.InteractAlternative();
        }

        private void OnRessurectHandler() => OnChangeRaycastDataHandler();

        private void OnHideAllHandler()
        {
            CursorView = null;
        }

        private void ItemPickupCursor()
        {
            if (IsHas<ItemPickup>(GameObject))
            {
                Show<PickUpCursorView>(ViewConfigID.PickUpCursor);
                return;
            }

            DefaultCorsor();
        }

        private void TableUpgradeCurcor()
        {
            var tableUpgradeShalterView = GameObject?.GetComponent<TableUpgradeShelterView>();
            if (tableUpgradeShalterView)
            {
                var shelterID = tableUpgradeShalterView.ShelterModelID;
                var shelterModel = ShelterModelsProvider[shelterID];
                var isMaxLevel = shelterModel.IsMaxLevel;
                var isBuyed = SheltersModel.IsBuyed(shelterID);
                var isActive = SheltersModel.ShelterActive == shelterID;
                var isCanActivateShelter = isBuyed && !isActive;

                // if (isCanActivateShelter)
                // {
                //     Show<ShelterCursorActivateView>(ViewConfigID.ShelterCursorActivate);
                //     return;
                // }

                // var isCanUpgradeShelter = isBuyed && !isMaxLevel;
                var isCanUpgradeShelter = !isMaxLevel;
                if (isCanUpgradeShelter)
                {
                    Show<ShelterCursorUpgradeView>(ViewConfigID.ShelterCursorUpgrade);
                    return;
                }
            }

            DefaultCorsor();
        }

        private void RestorableFurnace()
        {
            var restorable = GameObject?.GetComponent<RestorableObject>();
            if (restorable)
            {
                if (restorable.Health.Health < restorable.Health.HealthMax)
                {
                    if (IsHas<FurnaceObjectView>(GameObject))
                    {
                        Show<FurnaceHealthCursorView>(ViewConfigID.FurnaceHealthCursor);
                        return;
                    }
                }

                FurnaceCursor();
                return;
            }

            DefaultCorsor();
        }

        private void RestorableCampfire()
        {
            var restorable = GameObject?.GetComponent<RestorableObject>();
            if (restorable)
            {
                if (restorable.Health.Health < restorable.Health.HealthMax)
                {
                    if (IsHas<CampFireObjectView>(GameObject))
                    {
                        Show<CampFireHealthCursorView>(ViewConfigID.CampFireHealthCursor);
                        return;
                    }
                }

                CampfireCursor();
                return;
            }

            DefaultCorsor();
        }

        private void RestorableLoom()
        {
            var restorable = GameObject?.GetComponent<RestorableObject>();
            if (restorable)
            {
                if (restorable.Health.Health < restorable.Health.HealthMax)
                {
                    if (IsHas<LoomObjectView>(GameObject))
                    {
                        Show<LoomHealthCursorView>(ViewConfigID.LoomHealthCursor);
                        return;
                    }
                }

                LoomCursor();
                return;
            }

            DefaultCorsor();
        }

        private void RestorableWaterCatcher()
        {
            var restorable = GameObject?.GetComponent<RestorableObject>();
            if (restorable)
            {
                if (restorable.Health.Health < restorable.Health.HealthMax)
                {
                    if (IsHas<ItemPickUpTimeDelayController>(GameObject))
                    {
                        if (GameObject.GetComponentInParent<ItemPickUpTimeDelayModel>().IsHasItem)
                        {
                            Show<PickUpTimeDelayHealthCursorView>(ViewConfigID.PickUpTimeDelayHealthCursor);
                            return;
                        }

                        Show<PickUpTimeDelayTimerHealthCursorView>(ViewConfigID.PickUpTimeDelayTimerHealthCursor);
                        return;
                    }
                }

                PickupTimeDelayCursor();
                return;
            }

            DefaultCorsor();
        }

        private void RestorableLoot()
        {
            var restorable = GameObject?.GetComponent<RestorableObject>();
            if (restorable)
            {
                if (restorable.Health.Health < restorable.Health.HealthMax)
                {
                    if (IsHas<LootObject>(GameObject))
                    {
                        Show<OpenLootHealthCursorView>(ViewConfigID.OpenLootHealthCursor);
                        return;
                    }
                }

                LootObjectCursor();
                return;
            }

            DefaultCorsor();
        }

        private void RestorableDoor(RestorableObject restorable)
        {
            if (restorable.Health.Health < restorable.Health.HealthMax)
            {
                Show<OpenDoorHealthCursorView>(ViewConfigID.OpenDoorHealthCursor);
            }
            else
            {
                OpenDoorCursor();
            }
        }

        private void Restorable()
        {
            var restorable = GameObject?.GetComponent<RestorableObject>();
            if (restorable)
            {
                if (restorable.Health.Health < restorable.Health.HealthMax)
                {
                    Show<RestorableCursorView>(ViewConfigID.RestorableCursor);
                    return;
                }
            }

            DefaultCorsor();
        }

        private void RestorableCursor()
        {
            var restorable = GameObject?.GetComponent<RestorableObject>();
            if (restorable)
            {
                if (IsHas<CampFireObjectView>(GameObject))
                {
                    RestorableCampfire();
                    return;
                }
                else if (IsHas<FurnaceObjectView>(GameObject))
                {
                    RestorableFurnace();
                    return;
                }
                else if (IsHas<LoomObjectView>(GameObject))
                {
                    RestorableLoom();
                    return;
                }
                else if (IsHas<ItemPickUpTimeDelayController>(GameObject))
                {
                    RestorableWaterCatcher();
                    return;
                }
                else if (IsHas<LootObject>(GameObject))
                {
                    RestorableLoot();
                    return;
                }
                else if (IsHas<ManualDoorOpener>(GameObject))
                {
                    RestorableDoor(restorable);
                    return;
                }

                Restorable();
                return;
            }

            DefaultCorsor();
        }

        private void DestroyableCursor()
        {
            if (IsHas<DestroyableObject>(GameObject))
            {
                Show<DestroyableCursorView>(ViewConfigID.DestroyableCursor);
                return;
            }

            DefaultCorsor();
        }

        private void LootObjectCursor()
        {
            if (GameObject?.GetComponent<LootObject>()?.IsCanOpen ?? false)
            {
                Show<OpenLootCursorView>(ViewConfigID.OpenLootCursor);
                return;
            }

            DefaultCorsor();
        }

        private void CampfireCursor()
        {
            if (IsHas<CampFireObjectView>(GameObject))
            {
                Show<CampFireCursorView>(ViewConfigID.CampFireCursor);
                return;
            }

            DefaultCorsor();
        }

        private void FurnaceCursor()
        {
            if (IsHas<FurnaceObjectView>(GameObject))
            {
                Show<FurnaceCursorView>(ViewConfigID.FurnaceCursor);
                return;
            }

            DefaultCorsor();
        }

        private void LoomCursor()
        {
            if (IsHas<LoomObjectView>(GameObject))
            {
                Show<LoomCursorView>(ViewConfigID.LoomCursor);
                return;
            }

            DefaultCorsor();
        }

        private void PickupTimeDelayCursor()
        {
            if (IsHas<ItemPickUpTimeDelayController>(GameObject))
            {
                if (GameObject.GetComponentInParent<ItemPickUpTimeDelayModel>().IsHasItem)
                {
                    Show<PickUpTimeDelayCursorView>(ViewConfigID.PickUpTimeDelayCursor);
                    return;
                }

                Show<PickUpTimeDelayTimerCursorView>(ViewConfigID.PickUpTimeDelayTimerCursor);
                return;
            }

            DefaultCorsor();
        }

        private void TombCursor()
        {
            if (IsHas<TombInteractable>(GameObject))
            {
                Show<TombCursorView>(ViewConfigID.TombCursor);
                return;
            }

            DefaultCorsor();
        }

        private void MinableCursor()
        {
            var mineable = GameObject.CheckNull()?.GetComponent<MineableObject>();
            if (mineable != null)
            {
                var isWood = mineable.RequiredToolPurpose == FPTool.ToolPurpose.CutWood && !MineableObjectsModel.HasWoodTutorialShown;
                if (isWood)
                {
                    Show<MineableCursorView>(ViewConfigID.MineableCursor, ResourceMessagesView.ContainerContent);
                    return;
                }

                var isStone = mineable.RequiredToolPurpose == FPTool.ToolPurpose.BreakRocks && !MineableObjectsModel.HasStoneTutorialShown;
                if (isStone)
                {
                    Show<MineableCursorView>(ViewConfigID.MineableCursor, ResourceMessagesView.ContainerContent);
                    return;
                }

                var isDig = mineable.RequiredToolPurpose == FPTool.ToolPurpose.Dig && !MineableObjectsModel.HasStoneTutorialShown;
                if (isDig)
                {
                    Show<MineableCursorView>(ViewConfigID.MineableCursor, ResourceMessagesView.ContainerContent);
                    return;
                }
            }
            DefaultCorsor();
        }

        private void OpenDoorCursor()
        {
            if (IsHas<ManualDoorOpener>(GameObject))
            {
                Show<OpenDoorCursorView>(ViewConfigID.OpenDoorCursor);
                return;
            }

            DefaultCorsor();
        }

        private void CoastLootCursor() {
            if(GameObject != null && IsHas<CoastLootObject>(GameObject))
            {
                TreasureLootObject lootObject = GameObject.GetComponent<TreasureLootObject>();
                if (lootObject != null)
                {
                    if (lootObject.TimeSpawnTicks < GameTimeModel.RealTimeNowTick)
                    {
                        Show<OpenTreasureLootCursorView>(ViewConfigID.OpenCoastLootCursor);
                    }
                    else
                    {
                        Show<TreasureLootTimerCursorView>(ViewConfigID.TreasureLootTimerCursor);
                    }
                    return;
                }
            }

            DefaultCorsor();
        }

        private void DeadManLootCursor() 
        {
            if (GameObject != null && IsHas<TreasureLootObject>(GameObject))
            {
                if (DeadManLootChestModel.Unlock)
                {
                    Show<OpenTreasureLootCursorView>(ViewConfigID.DeadManLootCursor);
                    return;
                }
            }

            DefaultCorsor();
        }

        private void BombDestroyCursor() {
            if (IsHasParent<BombDestroyEnter>(GameObject))
            {
                if(ResourceMessagesView != null)
                {
                    Show<BombDestroyCursorView>(ViewConfigID.BombDestroyCursor,ResourceMessagesView.ContainerContent);
                    return;
                }
            }
            DefaultCorsor();
        }

        private void ToggleInteractableCursor() {
            if (IsHas<IToggleInteractable>(GameObject))
            {
                Show<ToggleInteractableCursorView>(ViewConfigID.ToggleInteractableCursor);
                return;
            }

            DefaultCorsor();
        }

        private void ItemsRelatedInteractableCursor() {
            ItemsRelatedInteractableBase itemsRelatedInteractable = GameObject?.GetComponent<ItemsRelatedInteractableBase>();
            if (itemsRelatedInteractable != null && itemsRelatedInteractable.CanUse())
            {
                Show<ItemsRelatedInteractableCursorView>(ViewConfigID.RequiredItemsCursorViewConfig);
                return;
            }

            DefaultCorsor();
        }
        private void QuestItemRelatedInteractableCursor() {
            QuestDependantInteractable interactable = GameObject?.GetComponent<QuestDependantInteractable>();
            if (interactable != null && interactable.Avaliable())
            {
                Show<ItemsRelatedInteractableCursorView>(ViewConfigID.QuestRequiredItemsCursorViewConfig);
                return;
            }

            DefaultCorsor();
        }

        private void HouseBuildingCursor()
        {
            Show<HouseBuildingCursorView>(ViewConfigID.HouseBuildingCursorConfig);
        }


        private void ActivateCursor()
        {
            if (IsHas<ObjectsActivator>(GameObject))
            {
                Show<ActivateObjectsCursorView>(ViewConfigID.ActivateObjectsCursor);
                return;
            }
        }

        private void ProgressCursor()
        {
            if (IsHasParent<DungeonEnter>(GameObject))
            {
                Show<ProgressCursorView>(ViewConfigID.ProgressCursor);
                return;
            }
        }

        private void OnChangeRaycastDataHandler()
        {
            GameObjectLast = GameObject;
            GameObject = PlayerEventHandler.RaycastData.Value?.GameObject;

            ProcessingCursor(GameObject.CheckNull()?.GetComponent<CursorIdentifier>()?.CursorID ?? CursorID.None);
        }

        private void ProcessingCursor(CursorID cursorID) => Cursors[(int)cursorID]();

        private void DefaultCorsor() => Show<DotCrosshairView>(ViewConfigID.DotCrosshair);

        public bool IsHas<T>(GameObject obj) where T : class => obj?.GetComponent<T>() != null;
        public bool IsHasParent<T>(GameObject obj) where T : class => obj?.GetComponentInParent<T>() != null;

        public void Show<T>(ViewConfigID viewConfigID) where T : CursorViewBase
        {
            if (CursorView != null)
            {
                if (CursorView is T && GameObject == GameObjectLast)
                {
                    return;
                }

                ViewsSystem.Hide(CursorView);
            }
            CursorView = ViewsSystem.Show<T>(viewConfigID);
            CursorView.gameObject.transform.localScale = Vector3.one;
        }
        public void Show<T>(ViewConfigID viewConfigID, Transform container) where T : CursorViewBase
        {
            if (CursorView != null)
            {
                if (CursorView is T && GameObject == GameObjectLast)
                {
                    return;
                }

                ViewsSystem.Hide(CursorView);
            }
            CursorView = ViewsSystem.Show<T>(viewConfigID,container);
            CursorView.gameObject.transform.localScale = Vector3.one;
        }

        public void Hide()
        {
            if (CursorView != null)
            {
                ViewsSystem.Hide(CursorView);
                CursorView = null;
            }
        }
    }
}
