using UnityEngine;
using UltimateSurvival;
using Game.Models;
using Game.ThirdPerson.Weapon.Core.Interfaces;

namespace Game.ThirdPerson.WeaponSelection
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private PlayerEventHandler Player;

        private IWeaponInteractor weaponSelect;


        #region MonoBehaviour
        private void Awake()
        {
            Player.ChangeEquippedItem.SetTryer(Try_ChangeEquippedItem);
            weaponSelect = GetComponent<IWeaponInteractor>();
        }

        #endregion

        private bool Try_ChangeEquippedItem(SavableItem item, bool instantly)
        {
            if (item?.ItemData != null && item.IsBroken())
            {
                Equip(null, instantly);
                Player.Aim.ForceStop();
                return false;
            }

            // if (item == ItemCurrent && !m_WaitingToEquip)
            // {
            //     return true;
            // }
            Equip(item, instantly);

            return true;
        }

        private void Equip(SavableItem item, bool instantly)
        {
            var weaponID = GetPlayerWeaponID(item);

            weaponSelect.Equip(weaponID);
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
    }
}