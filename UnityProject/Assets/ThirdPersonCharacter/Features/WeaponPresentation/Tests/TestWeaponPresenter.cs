using Game.ThirdPerson.Weapon.Presentation.Interfaces;
using NaughtyAttributes;
using UnityEngine;

namespace Game.ThirdPerson.Weapon.Presentation.Implementation
{
    public class TestWeaponPresenter : MonoBehaviour
    {
        [SerializeField] private IWeaponPresenter weaponPresenter;
        [SerializeField] private bool holdInRightHand;
        [SerializeField] private GameObject prefab;

        [Space]
        [SerializeField] private bool testOnStart;

        private void Start()
        {
            weaponPresenter = GetComponent<IWeaponPresenter>();

            if (testOnStart) Test();
        }

        [Button]
        private void Test()
        {
            var request = new PresentWeaponRequest(prefab,holdInRightHand);
            weaponPresenter.Present(request);
        }
    }
}