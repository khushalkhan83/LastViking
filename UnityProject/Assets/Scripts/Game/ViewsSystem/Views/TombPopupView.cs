using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class TombPopupView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _playerNameText;
        [SerializeField] private Text _timeAliveText;
        [SerializeField] private Text _timeAliveValueText;
        [SerializeField] private Text _killedText;
        [SerializeField] private Text _killedValueText;
        [SerializeField] private Text _shelterText;
        [SerializeField] private Text _shelterValueText;
        [SerializeField] private Text _shelterNoneText;
        [SerializeField] private Text _takeWatchButtonText;
        [SerializeField] private Text _takeGoldButtonText;
        [SerializeField] private Text _takeGoldValueButtonText;
        [SerializeField] private Text _bonusText;
        [SerializeField] private Text _otherItemsCountValueText;

        [SerializeField] private Image _bonusItemIcon;
        [SerializeField] private Image _playerImage;
        [SerializeField] private Text _playerName;

        [SerializeField] private TombItemCellView[] _tombItemSellViews;
        [SerializeField] private GameObject _objectToScale;

        [VisibleObject] [SerializeField] private GameObject _bonusObject;
        [VisibleObject] [SerializeField] private GameObject _takeGoldObject;
        [VisibleObject] [SerializeField] private GameObject _otherItemsInfoObject;
        [VisibleObject] [SerializeField] private GameObject _shelterInfoObject;
        [VisibleObject] [SerializeField] private GameObject _shelterNoneObject;

#pragma warning restore 0649
        #endregion

        public TombItemCellView[] TombItemCellViews => _tombItemSellViews;

        public void SetTextPlayerName(string text) => _playerNameText.text = text;
        public void SetTextTimeAlive(string text) => _timeAliveText.text = text;
        public void SetTextTimeAliveValue(string text) => _timeAliveValueText.text = text;
        public void SetTextKilled(string text) => _killedText.text = text;
        public void SetTextKilledValue(string text) => _killedValueText.text = text;
        public void SetTextShelter(string text) => _shelterText.text = text;
        public void SetTextShelterValue(string text) => _shelterValueText.text = text;
        public void SetTextShelterNone(string text) => _shelterNoneText.text = text;
        public void SetTextTakeWatchButton(string text) => _takeWatchButtonText.text = text;

        public void SetTextTakeGoldButton(string text) => _takeGoldButtonText.text = text;
        public void SetTextTakeGoldButtonValue(string text) => _takeGoldValueButtonText.text = text;
        public void SetTextBonus(string text) => _bonusText.text = text;
        public void SetTextOtherItemsCountValue(string text) => _otherItemsCountValueText.text = text;

        public void SetImageBonusItem(Sprite sprite) => _bonusItemIcon.sprite = sprite;
        public void SetImagePlayer(Sprite sprite) => _playerImage.sprite = sprite;
        public void SetPlayerName(string text) => _playerName.text = text;

        public void SetIsVisibleBonus(bool isVisible) => _bonusObject.SetActive(isVisible);
        public void SetIsVisibleTakeGoldObject(bool isVisible) => _takeGoldObject.SetActive(isVisible);
        public void SetIsVisibleOtherItemsInfo(bool isVisible) => _otherItemsInfoObject.SetActive(isVisible);
        public void SetIsVisibleShelterInfo(bool isVisible) => _shelterInfoObject.SetActive(isVisible);
        public void SetIsVisibleShelterNone(bool isVisible) => _shelterNoneObject.SetActive(isVisible);

        public void SetLessScale(Vector3 scale) => _objectToScale.transform.localScale = scale;
        //

        //UI
        public event Action OnTakeWatch;
        public void ActionTakeWatch() => OnTakeWatch?.Invoke();

        //UI
        public event Action OnTakeGold;
        public void ActionTakeGold() => OnTakeGold?.Invoke();

        //UI
        public event Action OnClose;
        public void ActionClose() => OnClose?.Invoke();
    }
}
