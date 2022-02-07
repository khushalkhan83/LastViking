using UnityEngine;

namespace Game.ThirdPerson.Weapon.Presentation.Interfaces
{
    public interface IWeaponPresenter
    {
        void Present(PresentWeaponRequest request);
    }

    public class PresentWeaponRequest
    {
        public PresentWeaponRequest(GameObject prefab, bool rightHand)
        {
            Prefab = prefab;
            RightHand = rightHand;
        }

        public GameObject Prefab { get; }
        public bool RightHand { get; }
    }
}