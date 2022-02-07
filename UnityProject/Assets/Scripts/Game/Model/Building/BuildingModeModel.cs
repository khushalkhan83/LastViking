using UnityEngine;
using System;
using System.Collections.Generic;
using static Game.Models.InventoryOperationsModel;
using UltimateSurvival;

namespace Game.Models
{
    public class BuildingModeModel : MonoBehaviour
    {
        public event Action BuildingEnabled;
        public event Action BuildingDisabled;
        public event Action OnPlacePartButtonPressed;
        public event Action<bool> OnHighlightPlaceButton;
        public event Action OnHideSwitchButtonChanged;

        private bool _buildingActive;
        private bool _hideSwitchButton;

        public Func<SavableItem,bool,IEnumerable<ItemInfo>> OnGetConstructionPrice;

        public bool BuildingActive
        {
            get { return _buildingActive; }
            set
            {
                _buildingActive = value;
                if (_buildingActive)
                    BuildingEnabled?.Invoke();
                else
                    BuildingDisabled?.Invoke();
            }
        }

        public bool HideSwitchButton
        {
            get{ return _hideSwitchButton; }
            set
            {
                _hideSwitchButton = value;
                OnHideSwitchButtonChanged?.Invoke();
            }
        }

        public void PlacePartButtonPressed() => OnPlacePartButtonPressed?.Invoke();

        public IEnumerable<ItemInfo> GetConstructionPrice(SavableItem construction, bool useTutorialPriceModificator = false)
        {
            return OnGetConstructionPrice?.Invoke(construction,useTutorialPriceModificator);
        }

        public void HighlightPlaceButton(bool highlight) => OnHighlightPlaceButton?.Invoke(highlight);
    }
}
