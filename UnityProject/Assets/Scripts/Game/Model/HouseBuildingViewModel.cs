using System;
using Game.VillageBuilding;
using UnityEngine;

namespace Game.Models
{
    public class HouseBuildingViewModel : MonoBehaviour
    {
        [SerializeField] private Sprite _citizensIconSprite;
        public Sprite CitizensIconSprite => _citizensIconSprite;
        public bool IsShow { get; private set; } = false;
        public event Action OnShowChanged;
        public void SetShow(bool value)
        {
            IsShow = value;
            OnShowChanged?.Invoke();
        }

        public HouseBuilding Target { get; private set; }
        public event Action OnTargetChanged;

        public void SetTarget(HouseBuilding houseBuilding)
        {
            this.Target = houseBuilding;
            OnTargetChanged?.Invoke();
        }


        public bool IsSkipButtonHilight { get; private set; }
        public event Action OnSkipButtonHilightChanged;

        public void SetSkipButtonHilight(bool value)
        {
            IsSkipButtonHilight = value;
            OnSkipButtonHilightChanged?.Invoke();
        }

        public bool IsBuildButtonHilight { get; private set; }
        public event Action OnIsBuildButtonHilightChanged;

        public void SetBuildButtonHilight(bool value)
        {
            IsBuildButtonHilight = value;
            OnIsBuildButtonHilightChanged?.Invoke();
        }

        public void RemoveAllHilight()
        {
            SetBuildButtonHilight(false);
            SetSkipButtonHilight(false);
        }

        public event Action OnUpgradeClicked;

        public void BuildClick()
        {
            OnUpgradeClicked?.Invoke();
        }

        public event Action OnSkipClicked;

        public void SkipClick()
        {
            OnSkipClicked?.Invoke();
        }
    }
}
