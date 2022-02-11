using Gamekit3D;
using UnityEngine;
using UnityEngine.UI;
using Game.ThirdPerson.Weapon.Core.Interfaces;
using Game.ThirdPerson.RangedCombat;
using System.Collections;
using UltimateSurvival;
using Game.Models;
using UnityEngine.EventSystems;

namespace Game.ThirdPerson
{
    public class PlayerMovementFacade : MonoBehaviour
    {
        [SerializeField] private AimMovementController aimingController;
        [SerializeField] private GameObject aimButton;
        [SerializeField] private CharacterLocomotion locomotion;


        private IWeaponInteractor weapon;

        private bool aim;

        private bool run;

        private bool attackPressed;

        private bool isRunningOrAiming;

        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
        private WorldCameraModel WorldCameraModel => ModelsSystem.Instance._worldCameraModel;

        #region MonoBehaviour
        private void Awake()
        {
            weapon = GetComponentInChildren<IWeaponInteractor>();
        }

        private void OnEnable()
        {
            SetActiveAimingController();
        }


        private void Update()
        {
            if (weapon.RangedWeaponEquiped)
            {
                if (PlayerInput.Instance.AimInput) aim = !aim;

                aimButton.gameObject.SetActive(true);
                if (aim && PlayerEventHandler.CanUseWeapon)
                {
                    SetActiveAimingController();
                }
                else if (!aim)
                {
                    StartCoroutine(DeactivateAim());
                }
            }
            else
            {
                aimButton.gameObject.SetActive(false);
                aim = false;
            }

            if (PlayerInput.Instance.RunInput) run = !run;

            isRunningOrAiming = run || aim;

            if (isRunningOrAiming)
            {
                Debug.LogError("SET RUN");
                locomotion.SetPreset(aim ? CharacterLocomotion.Preset.Aiming : CharacterLocomotion.Preset.Run);
            }
            else
            {
                locomotion.SetPreset(CharacterLocomotion.Preset.Walk);
            }
        }
        #endregion

        public void Reset()
        {
            aim = false;
            SetActiveAimingController();
        }

        public bool CanShowUI { get; set; }

        private void SwapMovementType()
        {
            // aim = !aim;
            SetActiveAimingController();
        }

        private void SetActiveAimingController()
        {
            aimingController.enabled = aim;
        }

        private void UpdateAimButton()
        {
            aimButton.gameObject.SetActive(CanShowUI && weapon.RangedWeaponEquiped);
        }

        private IEnumerator DeactivateAim()
        {
            yield return null;
            // aim = false;
            SetActiveAimingController();
        }
    }
}