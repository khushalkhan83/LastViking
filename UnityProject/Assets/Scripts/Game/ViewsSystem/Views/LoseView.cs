using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class LoseView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _title;

        [SerializeField] private Text _playAgainButtonText;

        [SerializeField] private Image _respawnPointIcon;

        [VisibleObject]
        [SerializeField] private GameObject _descriptionTextObject;
        [SerializeField] private Text _descriptionText;

        [SerializeField] private Sprite _shelterSprite;
        [SerializeField] private Sprite _palmSprite;

        [SerializeField] private LeaderboardView _leaderboardView;
        //[SerializeField] private GameSparksAccountView _accountView;

        //Statistics popup
        [SerializeField] private Text _totalDaysOnIsland;
        [SerializeField] private Text _daysBySession;
        [SerializeField] private Text _gatharedResources;
        [SerializeField] private Text _craftedItems;
        [SerializeField] private Text _killedAnimal;
        [SerializeField] private Text _killedZombies;
        [SerializeField] private Text _killedSkeletons;
        [SerializeField] private Text _brokenBarrels;
        //Statistics popup

        [SerializeField] private GameObject _playerList;
        [SerializeField] private GameObject _statistics;

        [SerializeField] private Transform _titleTrans;
        [SerializeField] private Transform _restartTrans;
        [SerializeField] private Transform _leaderboardTrans;

#pragma warning restore 0649
        #endregion

        //Statistics popup
        public Text TotalDaysOnIsland => _totalDaysOnIsland;
        public Text DaysBySession => _daysBySession;
        public Text GatheredResources => _gatharedResources;
        public Text CraftedItems => _craftedItems;
        public Text KilledAnimal => _killedAnimal;
        public Text KilledZombies => _killedZombies;
        public Text KilledSkeletons => _killedSkeletons;
        public Text BrokenBarrels => _brokenBarrels;

        public RectTransform TotalDaysOnIslandText;
        public RectTransform DaysBySessionText;
        public RectTransform GatheredResourcesText;
        public RectTransform CraftedItemsText;
        public RectTransform KilledAnimalText;
        public RectTransform KilledZombiesText;
        public RectTransform KilledSkeletonsText;
        public RectTransform BrokenBarrelsText;
        public RectTransform CounterText;
        public Animator Animator;
        public Text Counter;

        public GameObject PlayerList => _playerList;
        public GameObject Statistics => _statistics;
        public LeaderboardView LeaderboardView => _leaderboardView;

        //Statistics popup
        public void SetTotalDaysOnIsland(int days) => _totalDaysOnIsland.text = days.ToString();
        public void SetDaysBySession(int daysSisseon) => _daysBySession.text = daysSisseon.ToString();
        public void SetGatharedResourcesCount(int gathered) => _gatharedResources.text = gathered.ToString();
        public void SetCraftedItemsCount(int crafted) => _craftedItems.text = crafted.ToString();
        public void SetKilledAnimalCount(int animals) => _killedAnimal.text = animals.ToString();
        public void SetKilledZombieCount(int zombies) => _killedZombies.text = zombies.ToString();
        public void SetKilledSkeletonsCount(int skeletons) => _killedSkeletons.text = skeletons.ToString();
        public void SetBrokenBarrelsCount(int barrels) => _brokenBarrels.text = barrels.ToString();
        public void SetCounter(int counter) => Counter.text = "x" + counter.ToString();

        public void SetTotalDaysObjectScale(float scale) => TotalDaysOnIslandText.localScale = new Vector3(scale, scale, scale);
        public void SetSessionDaysObjectScale(float scale) => DaysBySessionText.localScale = new Vector3(scale, scale, scale);
        public void SetGatheredObjectScale(float scale) => GatheredResourcesText.localScale = new Vector3(scale, scale, scale);
        public void SetCraftedObjectScale(float scale) => CraftedItemsText.localScale = new Vector3(scale, scale, scale);
        public void SetKilledAnimalObjectScale(float scale) => KilledAnimalText.localScale = new Vector3(scale, scale, scale);
        public void SetKilledZombiesObjectScale(float scale) => KilledZombiesText.localScale = new Vector3(scale, scale, scale);
        public void SetKilledSkeletonsObjectScale(float scale) => KilledSkeletonsText.localScale = new Vector3(scale, scale, scale);
        public void SetBrokenObjectScale(float scale) => BrokenBarrelsText.localScale = new Vector3(scale, scale, scale);
        public void SetCounterObjectScale(float scale) => CounterText.localScale = new Vector3(scale, scale, scale);
        //Statistics popup

        public void ShowObjects()
        {
            _totalDaysOnIsland.text = string.Empty;
            _daysBySession.text = string.Empty;
            _gatharedResources.text = string.Empty;
            _craftedItems.text = string.Empty;
            _killedAnimal.text = string.Empty;
            _killedZombies.text = string.Empty;
            _killedSkeletons.text = string.Empty;
            _brokenBarrels.text = string.Empty;
            Counter.text = string.Empty;
        }

        public Sprite ShelterSprite => _shelterSprite;
        public Sprite PalmSprite => _palmSprite;

        public void SetTextTitle(string text) => _title.text = text;
        public void SetTextDescription(string text) => _descriptionText.text = text;
        public void SetTextPlayAgainButton(string text) => _playAgainButtonText.text = text;

        public void SetSpriteRespawnPoint(Sprite sprite) => _respawnPointIcon.sprite = sprite;

        public void SetIsVisibleDescription(bool isVisible) => _descriptionTextObject.SetActive(isVisible);

        public void ShowLeaderboardView(bool on) => LeaderboardView.gameObject.SetActive(on);

        public GameSparksAccountView AccountView { set; get; }

        //UI
        public event Action OnRestart;
        public void Restart() => OnRestart?.Invoke();

        //UI
        public event Action OnSelectStatistics;
        public void SelectStatistics() => OnSelectStatistics?.Invoke();
        //UI
        public event Action OnSelectPlayer;
        public void SelectPlayer() => OnSelectPlayer?.Invoke();
    }
}
