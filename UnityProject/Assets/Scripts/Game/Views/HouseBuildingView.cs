using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class HouseBuildingView : ViewBase
    {
        [SerializeField] private Image mainIcon = default;
        [SerializeField] private Image healthFillImage = default;
        [SerializeField] private GameObject descriptionObject = default;
        [SerializeField] private GameObject buildButtonObject = default;
        [SerializeField] private GameObject skipButtonObject = default;
        [SerializeField] private GameObject restoreButton = default;
        [SerializeField] private GameObject healthBar = default;
        [SerializeField] private Text headerText = default;
        [SerializeField] private Text descriptionText = default;
        [SerializeField] private Text timerText = default;
        [SerializeField] private Text buildButtonText = default;
        [SerializeField] private Text skipButtonText = default;
        [SerializeField] private Text skipCoinsText = default;
        [SerializeField] private Text citizensCountText = default;
        [SerializeField] private Text healthText = default;
        [SerializeField] private ResourceCellView[] resourceCells = default;
        [SerializeField] private ResourceCellView citizensCell = default;
        [SerializeField] private HouseBuildingCellView[] buildingCells = default;
        [SerializeField] private Text selectedResourceDescription = default;
        [SerializeField] private Sprite citizensIconSprite = default;

        public Image MainIcon => mainIcon;
        public Image HealthFillImage => healthFillImage;
        public GameObject DescriptionObject => descriptionObject;
        public GameObject BuildButtonObject => buildButtonObject;
        public GameObject SkipButtonObject => skipButtonObject;
        public GameObject RestoreButton => restoreButton;
        public GameObject HealthBar => healthBar;
        public Text HeaderText => headerText;
        public Text DescriptionText => descriptionText;
        public Text TimerText => timerText;
        public Text BuildButtonText => buildButtonText;
        public Text SkipButtonText => skipButtonText;
        public Text SkipCoinsText => skipCoinsText;
        public Text CitizensCountText => citizensCountText;
        public Text BuildingHealthText => healthText;
        public ResourceCellView[] ResourceCells => resourceCells;
        public ResourceCellView CitizensCell => citizensCell;
        public HouseBuildingCellView[] BuildingCells => buildingCells;
        public Text SelectedResourceDescription => selectedResourceDescription;

        public event Action OnCloseButtonClick;
        public void CloseButtonClick() => OnCloseButtonClick?.Invoke();

        public event Action OnBuildButtonClick;
        public void BuildButtonClick() => OnBuildButtonClick?.Invoke();

        public event Action OnSkipButtonClick;
        public void SkipButtonClick() => OnSkipButtonClick?.Invoke();

        public event Action OnRestoreButtonClick;
        public void RestoreButtonClick() => OnRestoreButtonClick?.Invoke();
    }
}
