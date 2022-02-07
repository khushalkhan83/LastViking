using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class PlayerDataView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _crownIcon;
        [VisibleObject]
        [SerializeField] private GameObject _crownObject;

        [SerializeField] private Text _place;
        [SerializeField] private Text _playerName;

        [SerializeField] private Image _playerIcon;
        [SerializeField] private Text _score;

        [SerializeField] private RectTransform _placeObject;
        [SerializeField] private RectTransform _scoreObject;

        [VisibleObject]
        [SerializeField] private GameObject _highlight;

#pragma warning restore 0649
        #endregion

        public void SetActiveCrown(bool on) => _crownObject.GetComponent<Image>().enabled = on;
        public void SetCrownIcon(Sprite icon) => _crownIcon.sprite = icon;
        public void SetActiveHighlight(bool on) => _highlight.SetActive(on);

        public void SetPlace(int place) => _place.text = place.ToString();
        public void SetPlaceString(string place) => _place.text = place;
        public void SetPlayerName(string name) => _playerName.text = name;
        public void SetPlayerIcon(Sprite icon) => _playerIcon.sprite = icon;
        public void SetPlayerScore(int score) => _score.text = score.ToString();
        public void SetPlaceObjectScale(float scale) => _placeObject.localScale = new Vector3(scale, scale, scale);
        public void SetScoreObjectScale(float scale) => _scoreObject.localScale = new Vector3(scale, scale, scale);

        public void SetPlayerData(PlayerData data)
        {
            SetPlayerName(data.Name);
            SetPlayerScore(data.Score);
        }
    }
}
