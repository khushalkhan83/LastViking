using AddressablesRelated;
using Game.AI;
using Game.Controllers;
using Game.Models;
using Game.Providers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UltimateSurvival
{
    public class FPManager : PlayerBehaviour
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private Camera m_WorldCamera;
        [SerializeField] private Camera m_FPCamera;
        [SerializeField] private Camera m_TokenCamera;
        [SerializeField] private HitInfo _hitInfo;

        [Header("Aiming")]

        [Range(0f, 100f)] [SerializeField] private float m_NormalFOV = 75f;
        [Range(0f, 100f)] [SerializeField] private float m_AimFOV = 45f;
        [Clamp(0f, 9999f)] [SerializeField] private float m_FOVSetSpeed = 30f;
        [Range(0.1f, 1f)] [SerializeField] private float m_AimSpeedMultiplier = 0.6f;

        [Header("Equipping")]

        [Range(0f, 3f)] [SerializeField] private float m_DrawTime = 0.7f;
        [Range(0f, 3f)] [SerializeField] private float m_HolsterTime = 0.5f;

        [Header("Pool")]
        [SerializeField] private int weaponPoolLimit = 0;

        // TODO: Debug. Remove
        public void SetPoolLimit(int weaponPoolLimit)
        {
            this.weaponPoolLimit = weaponPoolLimit;
        }

#pragma warning restore 0649
        #endregion

        public Camera WorldCamera => m_WorldCamera;
        private HitInfo HitInfo => _hitInfo;

        private PlayerWeaponProvider PlayerWeaponProvider => ModelsSystem.Instance.playerWeaponProvider;
        private PlayerRunModel PlayerRunModel => ModelsSystem.Instance._playerRunModel;
        private PlayerMovementModel PlayerMovementModel => ModelsSystem.Instance._playerMovementModel;
        private WeaponAssetsLoadingModel WeaponAssetsLoadingModel => ModelsSystem.Instance._weaponAssetsLoadingModel;
        private AddressablePool NewWeaponPool => WeaponAssetsLoadingModel.WeaponPool;
        private StaticPlayerWeaponProvider StaticPlayerWeaponProvider => WeaponAssetsLoadingModel.StaticPlayerWeaponProvider;

        private FPObject ObjectCurrent { get; set; }
        private FPWeaponBase m_EquippedWeapon { get; set; }
        public FPWeaponBase currentWeapon => m_EquippedWeapon;

        private float m_NextTimeCanEquip;
        private bool m_WaitingToEquip;
        private bool m_WaitingToDisable;
        private float m_NextTimeCanDisable;

        private Coroutine m_FOVSetter;

        public SavableItem ItemCurrent { get; private set; }
        public SavableItem ItemTo { get; private set; }

        public bool IsPlayerInWater => PlayerMovementModel.MovementID == PlayerMovementID.Water;

        public Dictionary<PlayerWeaponID, FPObject> WeaponPool { get; } = new Dictionary<PlayerWeaponID, FPObject>();

        public event Action<FPObject> onChangeEquipedItem;
        public event Action<PlayerWeaponID> onPreEquipeWeapon;

        private Action<FPObject> _processWeapon;

        private void LoadOrGetWeaponFromPool(PlayerWeaponID playerWeaponID, Action<FPObject> processWeapon)
        {
            _processWeapon = processWeapon;

            // handle hook as static weapon (not loaded by asset reference)
            if(StaticPlayerWeaponProvider.Contains(playerWeaponID))
            {
                bool weaponInPool = WeaponPool.TryGetValue(playerWeaponID, out var staticWeaponFromPool);

                if(weaponInPool)
                    WeaponLoadedHandler(staticWeaponFromPool.gameObject);
                else
                {
                    var staticWeapon = StaticPlayerWeaponProvider[playerWeaponID];
                    var instance = Instantiate(staticWeapon,transform);
                    WeaponLoadedHandler(instance.gameObject);
                }
                return;
            }

            var assetReference = PlayerWeaponProvider[playerWeaponID];
            NewWeaponPool.InstantiateAsync(assetReference, transform, WeaponLoadedHandler);
        }

        private void WeaponLoadedHandler(GameObject instance)
        {
            instance.TryGetComponent<FPObject>(out var weaponInstance);

            if (weaponInstance is FPWeaponBase fPWeaponBase)
            {
                fPWeaponBase.SetHitInfo(HitInfo);
            }
            _processWeapon?.Invoke(weaponInstance);
        }

        //TODO: remove old pool. Weapon managed with NewWeaponPool
        private void ReleaseWeapon(FPObject fPObject)
        {
            if (fPObject == null) return;

            bool createdByAddressablesPool = NewWeaponPool.ContainsInstance(fPObject.gameObject);

            bool objectInRegularPool = WeaponPool.TryGetValue(GetPlayerWeaponID(ItemCurrent), out var instance);

            if (objectInRegularPool) HandleExistingObject();
            else HandleNewObject();

            #region Mehtods

            void HandleExistingObject()
            {
                fPObject.gameObject.SetActive(false);
            }

            void HandleNewObject()
            {
                if (createdByAddressablesPool)
                {
                    NewWeaponPool.ReleaseInstance(fPObject.gameObject);
                }
                else
                {
                    WeaponPool[GetPlayerWeaponID(ItemCurrent)] = fPObject;
                    fPObject.gameObject.SetActive(false);
                }
            }
            #endregion
        }

        private void Awake()
        {
            Player.ChangeEquippedItem.SetTryer(Try_ChangeEquippedItem);
            Player.AttackOnce.SetTryer(() => OnTry_Attack(false));
            Player.AttackContinuously.SetTryer(() => OnTry_Attack(true));
            Player.Aim.AddStartTryer(TryStart_Aim);
            Player.Aim.AddStopListener(OnStop_Aim);
        }

        private void Update()
        {
            if (m_WaitingToDisable && Time.time > m_NextTimeCanDisable)
            {
                m_WaitingToDisable = false;
            }

            if (m_WaitingToEquip && Time.time > m_NextTimeCanEquip)
            {
                TryEquipItem();
                m_WaitingToEquip = false;
            }
        }

        private bool Try_ChangeEquippedItem(SavableItem item, bool instantly)
        {
            if (item?.ItemData != null && item.IsBroken())
            {
                A(null, instantly);
                Player.Aim.ForceStop();
                return false;
            }

            if (item == ItemCurrent && !m_WaitingToEquip)
            {
                return true;
            }
            A(item, instantly);

            return true;
        }

        private void A(SavableItem item, bool instantly)
        {
            // Register the current equipped object for disabling.
            if (!m_WaitingToEquip && ItemCurrent?.ItemData != null)
            {
                ObjectCurrent?.On_Holster();

                m_WaitingToDisable = true;
                m_NextTimeCanDisable = Time.time;

                if (!instantly)
                {
                    m_NextTimeCanDisable += m_HolsterTime;
                }
            }

            // Register the object for equipping.
            m_WaitingToEquip = true;
            m_NextTimeCanEquip = Time.time;

            if (!instantly && ItemCurrent != null)
            {
                m_NextTimeCanEquip += m_HolsterTime;
            }

            Player.EquippedItem.Set(item);
            ItemTo = item;
        }

        private float equipStartTime;

        private void DisplayTimeSinseEquipStarted()
        {
            var interval = Time.timeSinceLevelLoad - equipStartTime;

            Debug.Log("equip time: " + interval.ToString());
        }

        private void TryEquipItem()
        {
            equipStartTime = Time.timeSinceLevelLoad;

            ReleaseWeapon(ObjectCurrent);

            ItemCurrent = ItemTo;

            ItemTo = null;
            ObjectCurrent = null;
            m_EquippedWeapon = null;
            if (ItemCurrent == null || ItemCurrent.ItemData == null)
                return;

            var weaponId = GetPlayerWeaponID(ItemCurrent);

            onPreEquipeWeapon?.Invoke(weaponId);

            LoadAndProcessWeapon(weaponId, FPOjbectLodedHandle);
        }



        private void FPOjbectLodedHandle(FPObject obj)
        {
            obj.gameObject.SetActive(true);

            obj.On_Draw(ItemCurrent);

            ObjectCurrent = obj;
            m_EquippedWeapon = ObjectCurrent as FPWeaponBase;

            m_FPCamera.fieldOfView = ObjectCurrent.TargetFOV;

            onChangeEquipedItem?.Invoke(ObjectCurrent);

            DisplayTimeSinseEquipStarted();
        }

        private PlayerWeaponID GetPlayerWeaponID(SavableItem item)
        {
            /* ��������: ����� ����� ������ � ����, �� �������� �� ��� ������, ������� � ���� ������ � �����, � 
             * tool_hook_water (�� ������ ����). � �.�. ������ ������� ��� ������, ������� ���������� ����� 
             * ����� ������ ������, �� � ������� ������ tool_hook_water. ��������, � ����� ����� ����� �������� ������ 
             */
            /*if (IsPlayerInWater)
            {
                return PlayerWeaponID.tool_hook_water;
            }*/

            if
            (
                item != null
                && item.ItemData != null
                && item.ItemData.TryGetProperty("PlayerWeaponID", out var property)
                && property.PlayerWeaponID != PlayerWeaponID.None
            )
            {
                return property.PlayerWeaponID;
            }

            return PlayerWeaponID.None;
        }

        private void LoadAndProcessWeapon(PlayerWeaponID playerWeaponID, Action<FPObject> processLoadedWeapon)
        {
            if(playerWeaponID == PlayerWeaponID.None ) return;

            LoadOrGetWeaponFromPool(playerWeaponID, (fPObject) => processLoadedWeapon?.Invoke(fPObject));
        }

        private bool TryStart_Aim()
        {
            bool canStartAiming =
                ItemCurrent?.ItemData != null
                && ObjectCurrent
                && Time.time > m_NextTimeCanEquip + m_DrawTime
                && !PlayerRunModel.IsRun;

            if (canStartAiming && ObjectCurrent as FPHitscan)
            {
                if (m_FOVSetter != null)
                {
                    StopCoroutine(m_FOVSetter);
                }

                m_FOVSetter = StartCoroutine(C_SetFOV(m_AimFOV));
            }

            if (canStartAiming)
            {
                Player.SetMovementSpeedFactor(m_AimSpeedMultiplier);
            }

            return canStartAiming;
        }

        private void OnStop_Aim()
        {
            if (m_FOVSetter != null)
                StopCoroutine(m_FOVSetter);

            m_FOVSetter = StartCoroutine(C_SetFOV(m_NormalFOV));

            Player.SetMovementSpeedFactor(1f);
        }

        private IEnumerator C_SetFOV(float targetFOV)
        {
            while (Mathf.Abs(m_WorldCamera.fieldOfView - targetFOV) > Mathf.Epsilon)
            {
                m_WorldCamera.fieldOfView = Mathf.MoveTowards(m_WorldCamera.fieldOfView, targetFOV, Time.deltaTime * m_FOVSetSpeed);
                yield return null;
            }
        }

        private bool OnTry_Attack(bool continuously)
        {
            if
            (
                ItemCurrent?.ItemData == null
                || m_EquippedWeapon == null
                || IsPlayerInWater
                || Time.time < m_NextTimeCanEquip + m_DrawTime
            )
            {
                return false;
            }

            if (Time.time > m_EquippedWeapon.LastDrawTime + m_DrawTime)
            {
                bool attackWasSuccessful;

                if (continuously)
                {
                    attackWasSuccessful = m_EquippedWeapon.TryAttackContinuously(m_WorldCamera);
                }
                else
                {
                    attackWasSuccessful = m_EquippedWeapon.TryAttackOnce(m_WorldCamera);
                }

                if (attackWasSuccessful && PlayerRunModel.IsRun)
                {
                    PlayerRunModel.RunStop();
                    PlayerRunModel.RunTogglePassive();
                }

                return attackWasSuccessful;
            }

            return false;
        }
    }
}


