using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class LeaderboardView : ViewBase
    {

        #region Data
#pragma warning disable 0649

        [SerializeField] private PlayerDataView[] _dataViews;

        [SerializeField] private Text _playerName;
        [SerializeField] private Text _playerScore;

        [SerializeField] private RectTransform _playerScoreObject;

        [SerializeField] private Image _usernameIcon;
        [SerializeField] private Button _logoutButton;

        [SerializeField] private Sprite _goldCrown;
        [SerializeField] private Sprite _silverCrown;
        [SerializeField] private Sprite _bronzeCrown;

        [SerializeField] private Button _submitButton;

        [SerializeField] private Text _playerScoreText;
        [SerializeField] private Text _submitButtonText;

#pragma warning restore 0649
        #endregion

        //

        public PlayerDataView[] DataViews => _dataViews;

        public void SetPlayerName(string name) => _playerName.text = name;
        public void SetPlayerScore(int score) => _playerScore.text = score.ToString();

        public void SetScoreObjectScale(float scale) => _playerScoreObject.localScale = new Vector3(scale, scale, scale);
        public void ShowUsernameIcon(bool on) => _usernameIcon.gameObject.SetActive(on);
        public void ShowLogoutButton(bool on) => _logoutButton.gameObject.SetActive(on);

        public void ShowSubmitButton(bool on) => _submitButton.gameObject.SetActive(on);

        public void SetTextPlayerScore(string text) => _playerScoreText.text = text;
        public void SetTextSubmitButton(string text) => _submitButtonText.text = text;

        public Sprite GoldCrown => _goldCrown;
        public Sprite SilverCrown => _silverCrown;
        public Sprite BronzeCrown => _bronzeCrown;

        //UI
        public event Action OnShowLeaderboard;
        public void ShowLeaderboard() => OnShowLeaderboard?.Invoke();

        public event Action OnGameSparksRegister;
        public void GameSparksRegister() => OnGameSparksRegister?.Invoke();
    }
}