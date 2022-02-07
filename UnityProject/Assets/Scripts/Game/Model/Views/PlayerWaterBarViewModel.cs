using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class PlayerWaterBarViewModel : MonoBehaviour
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

            public void SetButtonIDCurrent(int buttonIdCurrent)
            {
                ButtonAddonIDCurrent = buttonIdCurrent;
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
        [SerializeField] private ObscuredFloat[] _waterToShowButtonAddons;
        [SerializeField] private StorageModel _storageModel;

#pragma warning restore 0649
        #endregion

        public float WaterToShowButtonAddon => GetWaterToShowButtonAddon(AddonLevel);
        public ObscuredFloat[] WaterToShowButtonAddons => _waterToShowButtonAddons;

        public int AddonLevel
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
                _data.SetButtonIDCurrent((int)value);
            }
        }

        public StorageModel StorageModel => _storageModel;

        public event Action OnChangeAddonButtonType;
        public event Action OnChangeAddonLevel;

        public float GetWaterToShowButtonAddon(int level)
        {
            if (AddonLevel < WaterToShowButtonAddons.Length)
            {
                return WaterToShowButtonAddons[level];
            }

            return WaterToShowButtonAddons[WaterToShowButtonAddons.Length - 1];
        }

        public void SetAddonLevel(ushort level)
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
