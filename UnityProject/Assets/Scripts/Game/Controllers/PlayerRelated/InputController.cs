using System.Linq;
using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Views;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class InputController : IInputController, IController
    {
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }

        [Inject] public JoystickModel JoystickModel  { get; private set; }
        [Inject] public TouchpadModel TouchpadModel  { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler  { get; private set; }
        [Inject] public PlayerRunModel PlayerRunModel  { get; private set; }
        [Inject] public AimModel AimModel  { get; private set; }
        [Inject] public InventoryModel InventoryModel  { get; private set; }
        [Inject] public HotBarModel HotBarModel  { get; private set; }
        [Inject] public PlacementModel PlacementModel  { get; private set; }
        [Inject] public AudioSystem AudioSystem  { get; private set; }
        [Inject] public ActionsLogModel ActionsLogModel  { get; private set; }
        [Inject] public ItemsDB ItemsDB  { get; private set; }
        [Inject] public InventoryViewModel InventoryViewModel  { get; private set; }
        [Inject] public PlayerStaminaModel PlayerStaminaModel  { get; private set; }
        [Inject] public SheltersModel SheltersModel  { get; private set; }
        [Inject] public GameTimeModel GameTimeModel  { get; private set; }
        [Inject] public StorageModel StorageModel  { get; private set; }
        [Inject] public WorldObjectCreator WorldObjectCreator  { get; private set; }
        [Inject] public InputModel InputModel { get; private set; }
        [Inject] public ViewsStateModel ViewsStateModel { get; private set; }
        

        private InventoryPlayerView InventoryPlayerView { get; set; }
        private CraftView CraftView { get; set; }
        private PurchasesView PurchasesView { get; set; }
        private SettingsView SettingsView { get; set; }
        private ShelterPopupView ShelterPopupView { get; set; }
        private TombPopupView TombPopupView { get; set; }
        private GameObject GameObject { get; set; }
        private FurnaceView FurnaceView { get; set; }
        private CampFireView CampFireView { get; set; }
        private LoomView LoomView { get; set; }

        void IController.Enable()
        {
            // Disabled keyboard/gamepad input to editor only mode.
            #if !UNITY_EDITOR
            return;
            #endif

            GameUpdateModel.OnUpdate += OnUpdate;

            // movement basic
            InputModel.OnInput.AddListener(PlayerActions.Movement, OnInputMovementHandler);
            InputModel.OnInput.AddListener(PlayerActions.View, OnInputViewHandler);

            // attack
            InputModel.OnInput.AddListener(PlayerActions.Attack, OnInputAttackHandler);
            InputModel.OnInput.AddListener(PlayerActions.StopAttack, OnInputStopAttackHandler);

            // attack legacy
            InputModel.OnInput.AddListener(PlayerActions.Aim, OnInputAimHandler);

            // movement basic
            InputModel.OnInput.AddListener(PlayerActions.Jump, OnInputJumpHandler);
            InputModel.OnInput.AddListener(PlayerActions.Run, OnInputRunHandler);

            // UI. elements
            InputModel.OnInput.AddListener(PlayerActions.Inventory, OnInputInventoryHandler);
            InputModel.OnInput.AddListener(PlayerActions.Craft, OnInputCraftHandler);
            InputModel.OnInput.AddListener(PlayerActions.Purchases, OnInputPurchasesHandler);
            InputModel.OnInput.AddListener(PlayerActions.Settings, OnInputSettingsHandler);

            // UI. hotbar
            InputModel.OnInput.AddListener(PlayerActions.HotbarSwitch1, OnInputHotBarSwitch1Handler);
            InputModel.OnInput.AddListener(PlayerActions.HotbarSwitch2, OnInputHotBarSwitch2Handler);
            InputModel.OnInput.AddListener(PlayerActions.HotbarSwitch3, OnInputHotBarSwitch3Handler);
            InputModel.OnInput.AddListener(PlayerActions.HotbarSwitch4, OnInputHotBarSwitch4Handler);
            InputModel.OnInput.AddListener(PlayerActions.HotbarSwitch5, OnInputHotBarSwitch5Handler);
            InputModel.OnInput.AddListener(PlayerActions.HotbarSwitch6, OnInputHotBarSwitch6Handler);
            InputModel.OnInput.AddListener(PlayerActions.HotbarSwitchRight, OnInputHotBarSwitchRight);
            InputModel.OnInput.AddListener(PlayerActions.HotbarSwitchLeft, OnInputHotBarSwitchLeft);

            // UI. contextual actions
            InputModel.OnInput.AddListener(PlayerActions.Loot, OnInputLootHandler);
            // _inputModel.OnInput.AddListener(PlayerActions.ItemPickUp, OnInputItemPickUpHandler);
            InputModel.OnInput.AddListener(PlayerActions.Tomb, OnInputTombHandler);
            InputModel.OnInput.AddListener(PlayerActions.Furnace, OnInputFurnaceHandler);
            InputModel.OnInput.AddListener(PlayerActions.Campfire, OnInputCampfireHandler);
            InputModel.OnInput.AddListener(PlayerActions.Loom, OnInputLoomHandler);
            InputModel.OnInput.AddListener(PlayerActions.Shelter, OnInputShelterHandler);
            InputModel.OnInput.AddListener(PlayerActions.WaterPickUp, OnInputWaterPickUpHandler);
            InputModel.OnInput.AddListener(PlayerActions.Build, OnInputBuildHandler);
        }

        void IController.Start()
        {
        }

        void IController.Disable()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;

            InputModel.OnInput.RemoveListener(PlayerActions.Movement, OnInputMovementHandler);
            InputModel.OnInput.RemoveListener(PlayerActions.View, OnInputViewHandler);
            InputModel.OnInput.RemoveListener(PlayerActions.Attack, OnInputAttackHandler);
            InputModel.OnInput.RemoveListener(PlayerActions.StopAttack, OnInputStopAttackHandler);
            InputModel.OnInput.RemoveListener(PlayerActions.Aim, OnInputAimHandler);
            InputModel.OnInput.RemoveListener(PlayerActions.Jump, OnInputJumpHandler);
            InputModel.OnInput.RemoveListener(PlayerActions.Run, OnInputRunHandler);
            InputModel.OnInput.RemoveListener(PlayerActions.Inventory, OnInputInventoryHandler);
            InputModel.OnInput.RemoveListener(PlayerActions.Craft, OnInputCraftHandler);
            InputModel.OnInput.RemoveListener(PlayerActions.Purchases, OnInputPurchasesHandler);
            InputModel.OnInput.RemoveListener(PlayerActions.Settings, OnInputSettingsHandler);
            InputModel.OnInput.RemoveListener(PlayerActions.HotbarSwitch1, OnInputHotBarSwitch1Handler);
            InputModel.OnInput.RemoveListener(PlayerActions.HotbarSwitch2, OnInputHotBarSwitch2Handler);
            InputModel.OnInput.RemoveListener(PlayerActions.HotbarSwitch3, OnInputHotBarSwitch3Handler);
            InputModel.OnInput.RemoveListener(PlayerActions.HotbarSwitch4, OnInputHotBarSwitch4Handler);
            InputModel.OnInput.RemoveListener(PlayerActions.HotbarSwitch5, OnInputHotBarSwitch5Handler);
            InputModel.OnInput.RemoveListener(PlayerActions.HotbarSwitch6, OnInputHotBarSwitch6Handler);
            InputModel.OnInput.RemoveListener(PlayerActions.HotbarSwitchRight, OnInputHotBarSwitchRight);
            InputModel.OnInput.RemoveListener(PlayerActions.HotbarSwitchLeft, OnInputHotBarSwitchLeft);

            InputModel.OnInput.RemoveListener(PlayerActions.Loot, OnInputLootHandler);
            // _inputModel.OnInput.RemoveListener(PlayerActions.ItemPickUp, OnInputItemPickUpHandler);
            InputModel.OnInput.RemoveListener(PlayerActions.Tomb, OnInputTombHandler);
            InputModel.OnInput.RemoveListener(PlayerActions.Furnace, OnInputFurnaceHandler);
            InputModel.OnInput.RemoveListener(PlayerActions.Campfire, OnInputCampfireHandler);
            InputModel.OnInput.RemoveListener(PlayerActions.Loom, OnInputLoomHandler);
            InputModel.OnInput.RemoveListener(PlayerActions.Shelter, OnInputShelterHandler);
            InputModel.OnInput.RemoveListener(PlayerActions.WaterPickUp, OnInputWaterPickUpHandler);
            InputModel.OnInput.RemoveListener(PlayerActions.Build, OnInputBuildHandler);
        }


        public CursorLockMode CursorLock
        {
            get
            {
                return Cursor.lockState;
            }
            private set
            {
                Cursor.lockState = value;
                Cursor.visible = Cursor.lockState != CursorLockMode.Locked;
            }
        }

        public InventoryIsFillPopupView InventoryIsFillPopupView { get; private set; }

        public float sensitivity = 2;
        public float smoothing = 2;

        private void OnInputMovementHandler()
        {
            var verticalInput = Input.GetAxis("Vertical");
            var horizontalInput = Input.GetAxis("Horizontal");

            var joystickAxes = Vector2.zero;
            joystickAxes.y = verticalInput;
            joystickAxes.x = horizontalInput;

            JoystickModel.SetAxes(joystickAxes.normalized * JoystickModel.Radius);
        }

        private void OnInputViewHandler()
        {
            var touchpadAxes = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            touchpadAxes = Vector2.Scale(touchpadAxes, new Vector2(sensitivity * smoothing, sensitivity * smoothing));

            TouchpadModel.SetAxes(touchpadAxes);
        }

        private void OnInputAttackHandler()
        {
            GameUpdateModel.OnUpdate += AttackContinuouslyProcess;
            PlayerEventHandler.AttackOnce.Try();
        }

        private void OnInputStopAttackHandler()
        {
            GameUpdateModel.OnUpdate -= AttackContinuouslyProcess;
        }

        private void AttackContinuouslyProcess() => PlayerEventHandler.AttackContinuously.Try();

        private void OnInputAimHandler()
        {
            AimModel.SetActive(!AimModel.IsActive);

            if (!PlayerEventHandler.Aim.Active)
            {
                PlayerEventHandler.Aim.TryStart();
            }
            else
            {
                PlayerEventHandler.Aim.ForceStop();
            }
        }

        private void OnInputJumpHandler()
        {
            PlayerEventHandler.Jump.TryStart();
        }

        private void OnInputRunHandler()
        {
            PlayerRunModel.RunStart();
        }

        private void OnInputInventoryHandler()
        {
            if (InventoryPlayerView == null)
            {
                InventoryPlayerView = ViewsSystem.Show<InventoryPlayerView>(ViewConfigID.InventoryPlayer);
            }
            else
            {
                ViewsSystem.Hide(InventoryPlayerView);

                InventoryPlayerView = null;
            }
        }

        private void OnInputCraftHandler()
        {
            if (CraftView == null)
            {
                CraftView = ViewsSystem.Show<CraftView>(ViewConfigID.Craft);
            }
            else
            {
                ViewsSystem.Hide(CraftView);

                CraftView = null;
            }
        }

        private void OnInputPurchasesHandler()
        {
            if (PurchasesView == null)
            {
                PurchasesView = ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
            else
            {
                ViewsSystem.Hide(PurchasesView);

                PurchasesView = null;
            }
        }

        private void OnInputSettingsHandler()
        {
            if (SettingsView == null)
            {
                SettingsView = ViewsSystem.Show<SettingsView>(ViewConfigID.Settings);
            }
            else
            {
                ViewsSystem.Hide(SettingsView);

                SettingsView = null;
            }
        }

        private void OnInputHotBarSwitch1Handler()
        {
            SetHotBarCell(0);
        }

        private void OnInputHotBarSwitch2Handler()
        {
            SetHotBarCell(1);
        }

        private void OnInputHotBarSwitch3Handler()
        {
            SetHotBarCell(2);
        }

        private void OnInputHotBarSwitch4Handler()
        {
            SetHotBarCell(3);
        }

        private void OnInputHotBarSwitch5Handler()
        {
            SetHotBarCell(4);
        }

        private void OnInputHotBarSwitch6Handler()
        {
            SetHotBarCell(5);
        }

        private void OnInputHotBarSwitchLeft()
        {
            var id = HotBarModel.EquipCellId - 1;
            Debug.Log("Left" + id);
            SetHotBarCell(id);
        }
        private void OnInputHotBarSwitchRight()
        {
            var id = HotBarModel.EquipCellId + 1;
            Debug.Log("Right" + id);
            SetHotBarCell(id);
        }

        private void SetHotBarCell(int id)
        {
            if (id > 5 || id < 0) return;

            HotBarModel.Equp(id);
            PlayerEventHandler.ChangeEquippedItem.Try(HotBarModel.ItemsContainer.GetCell(id).Item, false);
        }

        private void OnInputLootHandler()
        {
            PlayerEventHandler.RaycastData.Value.GameObject.GetComponent<LootObject>().Open();
        }


        // TODO: refactor
        private void OnInputTombHandler()
        {
            if (TombPopupView == null)
            {
                TombPopupView = ViewsSystem.Show<TombPopupView>(ViewConfigID.TombPopup);
            }
            else
            {
                ViewsSystem.Hide(TombPopupView);

                TombPopupView = null;
            }
        }
        // TODO: refactor
        private void OnInputFurnaceHandler()
        {
            if (FurnaceView == null)
            {
                FurnaceView = ViewsSystem.Show<FurnaceView>(ViewConfigID.Furnace);
            }
            else
            {
                ViewsSystem.Hide(FurnaceView);

                FurnaceView = null;
            }
        }
        // TODO: refactor
        private void OnInputCampfireHandler()
        {
            if (CampFireView == null)
            {
                CampFireView = ViewsSystem.Show<CampFireView>(ViewConfigID.CampFire);
            }
            else
            {
                ViewsSystem.Hide(CampFireView);

                CampFireView = null;
            }
        }
        // TODO: refactor
        private void OnInputLoomHandler()
        {
            if (LoomView == null)
            {
                LoomView = ViewsSystem.Show<LoomView>(ViewConfigID.Loom);
            }
            else
            {
                ViewsSystem.Hide(LoomView);

                LoomView = null;
            }
        }
        // TODO: refactor
        private void OnInputShelterHandler()
        {
            if (ShelterPopupView == null)
            {
                ShelterPopupView = ViewsSystem.Show<ShelterPopupView>(ViewConfigID.ShelterPopup);
            }
            else
            {
                ViewsSystem.Hide(ShelterPopupView);

                ShelterPopupView = null;
            }
        }
        // TODO: refactor
        private void OnInputWaterPickUpHandler()
        {
            var itemPickUpTimeDelayModel = PlayerEventHandler.RaycastData.Value.GameObject.GetComponentInParent<ItemPickUpTimeDelayModel>();

            var itemFrom = GetItem(itemPickUpTimeDelayModel.ItemName, itemPickUpTimeDelayModel.ItemCount);
            var left = itemFrom.Count;

            if (itemFrom.IsCanStackable)
            {
                left = InventoryModel.ItemsContainer.AddItems(itemFrom.Id, itemFrom.Count);
                if (left > 0)
                {
                    left = HotBarModel.ItemsContainer.AddItems(itemFrom.Id, itemFrom.Count);
                }
            }
            else
            {
                var cellTo = GetEmptyCell(InventoryModel.ItemsContainer);
                if (cellTo == null)
                {
                    cellTo = GetEmptyCell(HotBarModel.ItemsContainer);
                }

                if (cellTo != null)
                {
                    left = 0;
                    cellTo.Item = itemFrom;
                }
            }

            var added = itemFrom.Count - left;

            if (added > 0)
            {
                var raycastData = PlayerEventHandler.RaycastData.Value;
                var audioIdentifier = raycastData.GameObject.GetComponent<AudioIdentifier>();

                if (audioIdentifier)
                {
                    AudioSystem.PlayOnce(audioIdentifier.AudioID[Random.Range(0, audioIdentifier.AudioID.Length)], raycastData.HitInfo.point);
                }

                var item = new SavableItem(itemFrom)
                {
                    Count = added
                };

                var itemData = ItemsDB.GetItem(item.Name);
                ActionsLogModel.SendMessage(new MessageInventoryGatheredData(item.Count, itemData));

                itemPickUpTimeDelayModel.StartSpawn();
            }

            if (left > 0)
            {
                itemFrom.Count = left;
                DropItem(itemFrom);

                if (!InventoryViewModel.IsMaxExpandLevel)
                {
                    ShowInventoryFillPopup();
                }
            }
        }
        // TODO: refactor
        private void OnInputBuildHandler()
        {
            PlacementModel.PlaceItem(HotBarModel.EquipCell.Item.ItemData.Id);
            PlayerEventHandler.PlaceObject.Try();
            HotBarModel.ItemsContainer.RemoveItemsFromCell(HotBarModel.EquipCellId, 1);
        }

        private CellModel GetEmptyCell(ItemsContainer itemsContainer) => itemsContainer.Cells.FirstOrDefault(x => !x.IsHasItem);

        private void ShowInventoryFillPopup()
        {
            InventoryIsFillPopupView = ViewsSystem.Show<InventoryIsFillPopupView>(ViewConfigID.InventoryIsFillPopup);
        }

        private SavableItem GetItem(string name, int count) => new SavableItem(ItemsDB.GetItem(name), count);

        private void DropItem(SavableItem item)
        {
            var itemPickup = WorldObjectCreator.Create(item.ItemData.WorldObjectID, PlayerEventHandler.transform.position, Quaternion.identity).GetComponentInChildren<ItemPickup>();
            itemPickup.SetItemToAdd(item);
        }

        private bool dPadYInUse;
        private bool dPadXInUse;

        private void OnUpdate()
        {
            UpdateBaseInput();

            if(ViewsStateModel.WindowOpened())
            {
                UpdateUIInput();
            }
            else
            {
                UpdateDefaultInput();
            }
        }

        private void UpdateBaseInput()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (CursorLock == CursorLockMode.Locked)
                {
                    CursorLock = CursorLockMode.None;
                }
                else
                {
                    CursorLock = CursorLockMode.Locked;
                }
            }
        }

        private void UpdateUIInput()
        {
            if (Input.GetButtonUp("X_Button"))
            {
                InputModel.Input(PlayerActions.UIMenu_X);
            }
            if (Input.GetButtonUp("A_Button"))
            {
                InputModel.Input(PlayerActions.UIMenu_A);
            }
            if (Input.GetButtonUp("B_Button"))
            {
                InputModel.Input(PlayerActions.UIMenu_B);
            }
            if (Input.GetButtonUp("Y_Button"))
            {
                InputModel.Input(PlayerActions.UIMenu_Y);
            }


            if (Input.GetButtonUp("UIMenu_CategoryRight"))
            {
                InputModel.Input(PlayerActions.UIMenu_CategoryRight);
            }
            if (Input.GetButtonUp("UIMenu_CategoryLeft"))
            {
                InputModel.Input(PlayerActions.UIMenu_CategoryLeft);
            }
        }
        private void UpdateDefaultInput()
        {
            //Mouse
            if (Time.timeScale != 0 && CursorLock == CursorLockMode.Locked)
            {
                InputModel.Input(PlayerActions.View);
            }

            ////Movement
            var verticalInput = Input.GetAxis("Vertical");
            var horizontalInput = Input.GetAxis("Horizontal");
            if (verticalInput != 0 || horizontalInput != 0)
            {
                InputModel.Input(PlayerActions.Movement);
            }

            //Attack
            if (Input.GetButtonDown("Fire1") && CursorLock == CursorLockMode.Locked)
            // if (Input.GetMouseButtonDown(0) && CursorLock == CursorLockMode.Locked)
            {
                InputModel.Input(PlayerActions.Attack);
            }

            if (Input.GetButtonUp("Fire1"))
            {
                InputModel.Input(PlayerActions.StopAttack);
            }

            //Aim
            if (Input.GetMouseButtonDown(1))
            {
                InputModel.Input(PlayerActions.Aim);
            }

            //Jump
            if (Input.GetButtonDown("Jump"))
            {
                InputModel.Input(PlayerActions.Jump);
            }

            //Run
            if (Input.GetButtonDown("Run"))
            {
                if (PlayerStaminaModel.Stamina > 0)
                {
                    InputModel.Input(PlayerActions.Run);
                }
            }

            //Inventory
            if (Input.GetButtonDown("Inventory"))
            {
                InputModel.Input(PlayerActions.Inventory);
            }



            // handle d pad buttons
            var dpadX = Input.GetAxis("DPadX");
            if (dpadX != 0 && !dPadXInUse)
            {
                dPadXInUse = true;

                if (dpadX == 1)
                {
                    InputModel.Input(PlayerActions.Craft);
                }
                else if (dpadX == -1)
                {
                    InputModel.Input(PlayerActions.Inventory);
                }
            }
            if (dpadX == 0)
            {
                dPadXInUse = false;
            }

            var dpadY = Input.GetAxis("DPadY");
            if (dpadY != 0 && !dPadYInUse)
            {
                dPadYInUse = true;

                if (dpadY == 1)
                {
                    InputModel.Input(PlayerActions.ConsturctionMode);
                }
                else if (dpadY == -1)
                {
                    InputModel.Input(PlayerActions.Purchases);
                }
            }
            if (dpadY == 0)
            {
                dPadYInUse = false;
            }

            //Craft
            if (Input.GetButtonDown("Craft"))
            {
                InputModel.Input(PlayerActions.Craft);
            }

            //Purchase
            // if (Input.GetKeyDown(KeyCode.P))
            if (Input.GetButtonDown("Purchases")) // righh button to p (removed p because of conflict with editor shortcuts)
            {
                InputModel.Input(PlayerActions.Purchases);
            }

            //Settings
            // if (Input.GetKeyDown(KeyCode.Escape))
            // if (Input.GetKeyDown(KeyCode.Tilde))
            if (Input.GetButtonDown("Settings"))
            {
                InputModel.Input(PlayerActions.Settings);
            }

            //HotBar
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                InputModel.Input(PlayerActions.HotbarSwitch1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                InputModel.Input(PlayerActions.HotbarSwitch2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                InputModel.Input(PlayerActions.HotbarSwitch3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                InputModel.Input(PlayerActions.HotbarSwitch4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                InputModel.Input(PlayerActions.HotbarSwitch5);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                InputModel.Input(PlayerActions.HotbarSwitch6);
            }
            else if (Input.GetButtonDown("JoystickRightButton"))
            {
                InputModel.Input(PlayerActions.HotbarSwitchRight);
            }
            else if (Input.GetButtonDown("JoystickLeftButton"))
            {
                InputModel.Input(PlayerActions.HotbarSwitchLeft);
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                if (Cursor.visible == true)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }

            //Views
            if (Input.GetButtonDown("Action"))
            {
                InputModel.Input(PlayerActions.Interact);
            }
            if (Input.GetButtonDown("ActionAlternative"))
            {
                InputModel.Input(PlayerActions.InteractAlternative);
            }

            // cancel
            if (Input.GetButtonDown("Cancel"))
            {
                InputModel.Input(PlayerActions.Cancel);
            }

            //Build
            if (Input.GetKeyDown(KeyCode.F))
            {
                // InputModel.Input(PlayerActions.Build);
            }
        }

    }
}
