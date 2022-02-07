using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class DeathPlayerView : ViewAnimateBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _activeTextImage;
        [SerializeField] private Slider _handleSlider;

        [SerializeField] private Text _playerName;
        [SerializeField] private Text _playerScore;

        [SerializeField] private Text _resurrectGoldCostText;

        [VisibleObject] [SerializeField] private GameObject _watchButtonObj;
        [VisibleObject] [SerializeField] private GameObject _goldButtonObj;
        [SerializeField] private GameObject _goldBonusObj;

        [SerializeField] private Image _bonusItemIcon;

        [Header("Localization")]
        [SerializeField] private Text _title;
        [SerializeField] private Text _scoreTitle;
        [SerializeField] private Text _descriptionResurectBack;
        [SerializeField] private Text _descriptionResurectFront;
        [SerializeField] private Text _watchButtonText;
        [SerializeField] private Text _goldButtonText;

#pragma warning restore 0649
        #endregion

        public void SetPlayerName(string name) => _playerName.text = name;
        public void SetPlayerScore(int score) => _playerScore.text = score.ToString();

        public void SetResurrectGoldCostText(int cost) => _resurrectGoldCostText.text = cost.ToString();

        public void ShowGoldBonusObj(bool on) => _goldBonusObj.SetActive(on);

        public void SetBonusImage(Sprite icon) => _bonusItemIcon.sprite = icon;

        public void SetTextTitle(string text) => _title.text = text;
        public void SetTextScoreTitle(string text) => _scoreTitle.text = text;
        public void SetTextDescriptionResurectBack(string text) => _descriptionResurectBack.text = text;
        public void SetTextDescriptionResurectFront(string text) => _descriptionResurectFront.text = text;
        public void SetTextWatchButtonText(string text) => _watchButtonText.text = text;
        public void SetTextGoldButtonText(string text) => _goldButtonText.text = text;

        public void SelectResurrectButton(bool isGold)
        {
            _goldButtonObj.SetActive(isGold);
            _watchButtonObj.SetActive(!isGold);
        }

        public void SetResurrectTimeProgress(float value)
        {
            _activeTextImage.fillAmount = value;
            _handleSlider.value = value;
        }

        public bool IsGoldButtonActive() => _goldButtonObj.activeSelf;

        //

        //UI
        public event Action OnGold;
        public void Gold() => OnGold?.Invoke();

        //UI
        public event Action OnWatch;
        public void Watch() => OnWatch?.Invoke();

        //UI
        public event Action OnEndShow;
        public void EndShow() => OnEndShow?.Invoke();
    }
}
