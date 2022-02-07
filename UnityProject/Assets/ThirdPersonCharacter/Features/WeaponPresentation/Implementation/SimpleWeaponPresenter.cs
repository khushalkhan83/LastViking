using Game.ThirdPerson.Weapon.Presentation.Interfaces;
using Gamekit3D;
using UnityEngine;
using UnityEngine.Events;

namespace Game.ThirdPerson.Weapon.Presentation.Implementation
{
    public class SimpleWeaponPresenter : MonoBehaviour, IWeaponPresenter
    {
        [SerializeField] private Transform toolRoot;

        [SerializeField] private Transform rightHand;
        [SerializeField] private Transform leftHand;
        [SerializeField] private UpdateFollow updateFollow;

        [SerializeField] private UnityEvent onPresent;

        private GameObject weaponInstance;

        public void Present(PresentWeaponRequest data)
        {
            if(weaponInstance != null) Destroy(weaponInstance);

            var weaponPrefab = data.Prefab;
            if(weaponPrefab == null) return;
            
            weaponInstance = Instantiate(weaponPrefab,toolRoot);
            updateFollow.toFollow = data.RightHand ? rightHand: leftHand;

            onPresent?.Invoke();
        }
    }
}