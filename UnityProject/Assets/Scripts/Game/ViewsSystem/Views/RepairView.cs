using Core.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class RepairView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _titleRepair;
        [SerializeField] private Text _buttonRepairTitle;
        [SerializeField] private Text _buttonRepairInactiveTitle;
        [SerializeField] private Text _buttonRepairGoldTitle;
        [SerializeField] private Text _titlePart;
        [SerializeField] private Text _infoTitle;
        [SerializeField] private Text _countCoinsText;
        [SerializeField] private ResourceCellView[] _resourceCellViews;
        [SerializeField] private GameObject _repairButton;
        [SerializeField] private GameObject _repairButtonDisabled;

        [SerializeField] private Image _watchIcon;
        [SerializeField] private Image _goldIcon;

        [SerializeField] private GameObject _repairResourcesCellsObject;
        [SerializeField] private GameObject _repairResourcesLineObject;
        [SerializeField] private GameObject _repairResourcesButtonObject;

#pragma warning restore 0649
        #endregion

        public GameObject RepairButton => _repairButton;
        public GameObject RepairButtonDisabled => _repairButtonDisabled;
        public ResourceCellView[] ResourceCells => _resourceCellViews;

        public void SetTextTitleRepair(string text) => _titleRepair.text = text;
        public void SetTextTitlePart(string text) => _titlePart.text = text;
        public void SetTextButtonReapirTitle(string text) => _buttonRepairTitle.text = text;
        public void SetTextButtonReapirInactiveTitle(string text) => _buttonRepairInactiveTitle.text = text;
        public void SetTextButtonReapirGoldTitle(string text) => _buttonRepairGoldTitle.text = text;
        public void SetInfo(string text) => _infoTitle.text = text;
        public void SetCountText(string text) => _countCoinsText.text = text;
        public void SetCountVisible(bool visible) => _countCoinsText.enabled = visible;
        public void SetWatchIconVisible(bool visible) => _watchIcon.enabled = visible;
        public void SetGoldIconVisible(bool visible) => _goldIcon.enabled = visible;
        
        public void SetRepairResourcesPartVisible(bool visible)
        {
            _repairResourcesCellsObject.SetActive(visible);
            _repairResourcesButtonObject.SetActive(visible);
            _repairResourcesLineObject.SetActive(visible);
        }

        //UI
        public event Action OnClose;
        public void ActionClose() => OnClose?.Invoke();

        //UI
        public event Action OnRepairWatch;
        public void ActionRepair() => OnRepairWatch?.Invoke();

        //UI
        public event Action OnRepairGold;
        public void ActionRepairGold() => OnRepairGold?.Invoke();
        
        //UI
        public event Action OnAddCoins;
        public void ActionAddCoins() => OnAddCoins?.Invoke();
    }
}
