using Game.Models;
using Game.ThirdPerson.Weapon.Core.Interfaces;
using NaughtyAttributes;
using UnityEngine;

namespace Game.ThirdPerson.WeaponSelection.Tests
{
    public class TestWeaponController : MonoBehaviour
    {
        [SerializeField] private PlayerWeaponID defaultWeapon;
        [SerializeField] private bool equipOnStart;

        private IWeaponInteractor weapon;

        #region MonoBehaviour
        private void Awake()
        {
            weapon = GetComponent<IWeaponInteractor>();
        }
        private void Start()
        {
            if (!equipOnStart) return;
            Equip();
        }
        #endregion

        [Button]
        private void Equip()
        {
            weapon.Equip(defaultWeapon);
        }
    }
}