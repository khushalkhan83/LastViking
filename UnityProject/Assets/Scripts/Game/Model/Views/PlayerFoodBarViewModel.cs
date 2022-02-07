using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class PlayerFoodBarViewModel : MonoBehaviour
    {
        public enum ButtonAddonID
        {
            None = 0,
            WatchVideo = 1,
            Buy = 2,
        }

        [Serializable]
        public class Data : DataBase
        {
            [ObscuredID(typeof(ButtonAddonID))] public ObscuredInt ButtonAddonIDCurrent;
            public ObscuredInt AddonLevel;

            public void SetButtonAddonIDCurrent(int buttonAddonIDCurrent)
            {
                ButtonAddonIDCurrent = buttonAddonIDCurrent;
                ChangeData();
            }

            public void SetAddonLevel(int addonLevel)
            {
                AddonLevel = addonLevel;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private ObscuredFloat[] _foodToShowButtonAddons;
        [SerializeField] private StorageModel _storageModel;

#pragma warning restore 0649
        #endregion

        public ObscuredFloat FoodToShowButtonAddon => GetFoodToShowButtonAddon(AddonLevel);
        public ObscuredFloat[] FoodToShowButtonAddons => _foodToShowButtonAddons;

        public ObscuredInt AddonLevel
        {
            get
            {
                return _data.AddonLevel;
            }
            protected set
            {
                _data.SetAddonLevel(value);
            }
        }

        public ButtonAddonID ButtonAddonIDCurrent
        {
            get
            {
                return (ButtonAddonID)(int)_data.ButtonAddonIDCurrent;
            }
            protected set
            {
                _data.SetButtonAddonIDCurrent((int)value);
            }
        }

        public StorageModel StorageModel => _storageModel;

        public event Action OnChangeAddonButtonType;
        public event Action OnChangeAddonLevel;

        public ObscuredFloat GetFoodToShowButtonAddon(int level)
        {
            if (AddonLevel < FoodToShowButtonAddons.Length)
            {
                return FoodToShowButtonAddons[level];
            }

            return FoodToShowButtonAddons[FoodToShowButtonAddons.Length - 1];
        }

        public void SetAddonLevel(int level)
        {
            AddonLevel = level;
            OnChangeAddonLevel?.Invoke();
        }

        public void SetButtonAddonType(ButtonAddonID buttonAddonID)
        {
            ButtonAddonIDCurrent = buttonAddonID;
            OnChangeAddonButtonType?.Invoke();
        }

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
        }
    }
}
