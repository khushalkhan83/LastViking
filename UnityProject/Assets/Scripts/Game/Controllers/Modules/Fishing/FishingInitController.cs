using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Controllers;
using Core;
using Game.Models;
using Game.Views;
using Core.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class FishingInitController : IController, IFishingInitController
    {
        [Inject] public FishingModel fishingModel { get; private set; }
        [Inject] public PlayerMovementModel PlayerMovementModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public FPManager FPManager { get; private set; }
        [Inject] public BuildingModeModel BuildingModeModel { get; private set; }

        [Inject] public PlayerCameras PlayerCameras { get; private set; }


        public IView View { get; private set; }

        void IController.Enable()
        {
            // fishingModel.OnThrowStart += ThrowStartHandler;
            // fishingModel.OnThrowAnimationEnd += ThowCheck;
            // fishingModel.OnStartFishing += OnStartFishingHandler;
            // PlayerMovementModel.OnChangeMovementID += OnChangePlayerMovement;

            // fishingModel.OnStateChange += CheckMinigameMode;

            // FPManager.onChangeEquipedItem += OnChangeEquipedItem;
            // FPManager.onPreEquipeWeapon += OnPreEquipeWeapon;
            // BuildingModeModel.BuildingEnabled += OnBuildingModeChanged;
            // BuildingModeModel.BuildingDisabled += OnBuildingModeChanged;
        }

        void IController.Disable()
        {
            // fishingModel.OnThrowStart -= ThrowStartHandler;
            // fishingModel.OnThrowAnimationEnd -= ThowCheck;
            // fishingModel.OnStartFishing -= OnStartFishingHandler;
            // PlayerMovementModel.OnChangeMovementID -= OnChangePlayerMovement;
            // fishingModel.OnStateChange -= CheckMinigameMode;
            // FPManager.onChangeEquipedItem -= OnChangeEquipedItem;
            // FPManager.onPreEquipeWeapon -= OnPreEquipeWeapon;
            // BuildingModeModel.BuildingEnabled -= OnBuildingModeChanged;
            // BuildingModeModel.BuildingDisabled -= OnBuildingModeChanged;
        }

        private void OnChangePlayerMovement() => CheckFishingAvaible();
        
        private void OnBuildingModeChanged() => CheckFishingAvaible();
 
        void IController.Start()
        {
        }

        void OnStartFishingHandler(bool isFishing)
        {
            //CheckFishingAvaible();
        }

        void OnChangeEquipedItem(FPObject itemObject)
        {
            // if (FPManager.ItemCurrent.TryGetProperty("PlayerWeaponID", out var weaponId)
            //     && weaponId.PlayerWeaponID == PlayerWeaponID.tool_fishing_rod)
            // {
            //     itemObject.gameObject.GetComponent<FishingMinigameItems>()?.SetFishingModel(fishingModel);
            //     CheckFishingAvaible();
            // }
            // else
            // {
            //     fishingModel.Fishing(false, Vector3.zero, null);
            //     CloseView();
            // }
        }

        void OnPreEquipeWeapon(PlayerWeaponID weaponID)
        {
            // if(weaponID == PlayerWeaponID.None)
            // {
            //     fishingModel.Fishing(false, Vector3.zero, null);
            //     CloseView();
            // }
        }

        void CheckFishingAvaible()
        {
            // if
            // (
            //     PlayerMovementModel.MovementID == PlayerMovementID.Ground
            //     && fishingModel.state == FishingModel.FishingState.none
            //     && HotBarModel.EquipCell.IsHasItem
            //     && HotBarModel.EquipCell.Item.TryGetProperty("PlayerWeaponID", out var weaponId2)
            //     && weaponId2.PlayerWeaponID == PlayerWeaponID.tool_fishing_rod
            //     && HotBarModel.EquipCell.Item.TryGetProperty("Durability", out var durab)
            //     && durab.Float.Current > 0
            //     && !BuildingModeModel.BuildingActive
            // )
            // {
            //     OpenView();
            // }
            // else
            // {
            //     CloseView();
            // }
        }

        private void OpenView()
        {
            // if (View == null)
            // {
            //     View = ViewsSystem.Show<AimButtonView>(ViewConfigID.FishingInit);
            //     View.OnHide += OnHideHandler;
            // }
        }

        private void CloseView()
        {
            // if (View != null)
            // {
            //     ViewsSystem.Hide(View);
            // }
        }

        private void OnHideHandler(IView view)
        {
            // view.OnHide -= OnHideHandler;
            // View = null;
        }

        
        void CheckMinigameMode()
        {
            // if (fishingModel.state == FishingModel.FishingState.none)
            // {
            //     CheckFishingAvaible();
            // }
        }

        void ThrowStartHandler()
        {
            // RaycastHit hit;
            // Ray r = GetRay();
            // if (Physics.Raycast(r, out hit,fishingModel.startFishingDistance, fishingModel.FishingLayerMask))
            // {
            //     fishingModel.ThrowInited( hit.point);
            // }
            // else
            //     fishingModel.ThrowInited(r.origin + r.direction * fishingModel.startFishingDistance);
        }

        // Ray GetRay()
        // {
        //     // Vector3 pos = PlayerCameras.CameraWorld.transform.position;
        //     // Vector3 direction = PlayerCameras.CameraWorld.transform.TransformDirection(Vector3.forward);
        //     // Ray r = new Ray(pos, direction);
        //     // return r;
        // }

        void ThowCheck()
        {
            // RaycastHit hit;
            // Ray r = GetRay();

            // if (HotBarModel.EquipCell.Item.TryGetProperty("PlayerWeaponID", out var weaponId)
            //     && weaponId.PlayerWeaponID == PlayerWeaponID.tool_fishing_rod 
            //     && Physics.Raycast(r ,out hit, fishingModel.startFishingDistance, fishingModel.FishingLayerMask)
            //     && CheckDepth(hit.point))
            // {
            //     FishingZone zone = hit.collider.GetComponent<FishingZone>();
            //     if (zone && (zone.fishCount > 0))
            //     {
            //         Vector3 fishingPoint = new Vector3(hit.point.x, zone.transform.position.y, hit.point.z);
            //         fishingModel.Fishing(true, fishingPoint, zone);
            //         return;
            //     }
            // }

            // fishingModel.Fishing(false,Vector3.zero,null);
        }

        private bool CheckDepth(Vector3 point)
        {
            // Ray r = new Ray(point, Vector3.down);
            // RaycastHit hit;
            // if(Physics.Raycast(r ,out hit, fishingModel.FishingMinDepth, fishingModel.FishingBottomMask))
            // {
            //    return false;
            // }
            return true;
        }
    }
}