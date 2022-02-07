using Game.Models;
using Game.ThirdPerson.Weapon.Core.Interfaces;
using Game.ThirdPerson.Weapon.Data;
using Game.ThirdPerson.Weapon.Presentation.Interfaces;
using Game.ThirdPerson.Weapon.Settings.Interfaces;
using Gamekit3D;
using NaughtyAttributes;
using UnityEngine;

namespace Game.ThirdPerson.Weapon.Core.Implementation
{
    public class WeaponInteractor : MonoBehaviour, IWeaponInteractor
    {
        [SerializeField] private ThirdPersonToolsProvider thirdPersonToolsProvider;
        private IWeaponPresenter weaponPresenter;
        private IWeaponSettings weaponSettings;

        public bool WeaponEquiped {get;private set;}
        public bool RangedWeaponEquiped => weaponSettings.RangedWeaponEquiped;
        public bool MeleeWeaponEquiped => weaponSettings.MeleeWeaponEquiped;
        public float Damage => weaponSettings.Damage;
        public float Cooldown => weaponSettings.Cooldwon;
        public float HitZoneRadius => weaponSettings.HitZoneRadius;
        public float HitZoneLength => weaponSettings.HitZoneLength;

#if UNITY_EDITOR
        #region Testing

        [Header("Testing")]
        [SerializeField] private PlayerWeaponID id;
        [SerializeField] private ToolData data;


        [Button] void TestEquip_ByID() => Equip(id);
        [Button] void TestEquip() => Equip(data);

        #endregion
#endif

        #region MonoBehaviour
        private void Awake()
        {
            weaponPresenter = GetComponent<IWeaponPresenter>();
            weaponSettings = GetComponent<IWeaponSettings>();
        }

        #endregion

        public void Equip(PlayerWeaponID weaponID)
        {
            if (weaponID == PlayerWeaponID.None)
            {
                Equip(null);
                return;
            }

            ToolData toolData = null;
            try
            {
                toolData = thirdPersonToolsProvider[weaponID];
            }
            catch (System.Exception)
            {
                Debug.LogError($"Can`t get weapon for id: {weaponID}");
            }

            Equip(toolData);
        }

        private void Equip(ToolData tool)
        {
            PresentWeaponRequest request;
            SetSettingsRequest settings;
            if (tool == null)
            {
                request = new PresentWeaponRequest(null, true);
                settings = new SetSettingsRequest(false,0,1,1,1,3.5f,new UltimateSurvival.ExtractionSetting[0]);
                WeaponEquiped = false;
            }
            else
            {
                request = new PresentWeaponRequest(tool.Prefab, tool.RightHand);
                settings = new SetSettingsRequest(tool.IsRanged,tool.Damage,tool.AttackSpeed,tool.Cooldown,tool.HitZoneRadius,tool.HitZoneLength,tool.ExtractionSettings.ToArray());
                WeaponEquiped = true;
            }

            weaponPresenter.Present(request);
            weaponSettings.SetSettings(settings);
        }
    }
}