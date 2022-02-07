using Core.Storage;
using Game.Models;
using System;
using UnityEngine;

namespace UltimateSurvival
{
    public class MineableObjectsModel : MonoBehaviour
    {
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        [Serializable]
        public class Data : DataBase
        {
            public bool _hasWoodTutorialShown = true;
            public bool _hasStoneTutorialShown = true;

            public void SetHasWoodTutorialShown(bool hasWoodTutorialShown)
            {
                _hasWoodTutorialShown = hasWoodTutorialShown;
                ChangeData();
            }

            public void SetHasStoneTutorialShown(bool hasStoneTutorialShown)
            {
                _hasStoneTutorialShown = hasStoneTutorialShown;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;

#pragma warning restore 0649
        #endregion

        public delegate void OnMineItem(ItemData itemData, int count);

        public event OnMineItem OnMine;

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
        }

        public bool HasWoodTutorialShown
        {
            set => _data.SetHasWoodTutorialShown(value);
            get => _data._hasWoodTutorialShown;
        }

        public bool HasStoneTutorialShown
        {
            set => _data.SetHasStoneTutorialShown(value);
            get => _data._hasStoneTutorialShown;
        }

        public bool HasAllTutorialsShown => HasWoodTutorialShown && HasStoneTutorialShown;

        public void Mine(ItemData itemData, int count) => OnMine?.Invoke(itemData, count);
    }
}