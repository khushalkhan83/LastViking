using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class CraftView : ViewBase
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private RectTransform _container;

        [VisibleObject]
        [SerializeField] private GameObject _cancelButtonObject;

        [VisibleObject]
        [SerializeField] private GameObject _craftButtonObject;

        [VisibleObject]
        [SerializeField] private GameObject _craftDisableButtonObject;

        [VisibleObject]
        [SerializeField] private GameObject _studyButtonObject;

        [SerializeField] private Image _studyResourceIcon;
        [SerializeField] private Text _studyResourceCountText;

        [VisibleObject]
        [SerializeField] private GameObject _studyButtonDisableObject;

        [SerializeField] private Image _studyResourceDisableIcon;
        [SerializeField] private Text _studyResourceCountDisableText;

        [SerializeField] private Transform _containerBoostButton;
        [VisibleObject]
        [SerializeField] private GameObject _boostButtonObject;

        [SerializeField] private ResourceCellView[] _resourceCellViews;

        [SerializeField] private QueueCellView[] _queueCellViews;
        [SerializeField] private GameObject _firstqueueCellView;

        [SerializeField] private Transform _containerAddCraftCellButton;

        [SerializeField] private Transform _containerCraftItems;

        [SerializeField] private Image _categoryAllIcon;
        [SerializeField] private Image _categoryToolsIcon;
        [SerializeField] private Image _categoryItemsIcon;
        [SerializeField] private Image _categoryWeaponsIcon;
        [SerializeField] private Image _categoryDefenceIcon;
        [SerializeField] private Image _categoryMedicalIcon;

        [SerializeField] private Color _categoryColorSelected;
        [SerializeField] private Color _categoryColorDefault;

        [SerializeField] private Text _coinsValue;
        [SerializeField] private Text _blueprintsValue;

        [SerializeField] private Text _title;
        [SerializeField] private Text _resourcesTitle;
        [SerializeField] private Text _resourcesQueueTitle;
        [SerializeField] private Text _enabledCraftButtonTitle;
        [SerializeField] private Text _disabledCraftButtonTitle;
        [SerializeField] private Text _enabledStudyButtonTitle;
        [SerializeField] private Text _disabledStudyButtonTitle;
        [SerializeField] private Text _cancelButtonTitle;
        [SerializeField] private GameObject _emptyDescription;

        [SerializeField] private GameObject _itemsContainer;
#pragma warning restore 0649
        #endregion

        public Color CategoryColorSelected => _categoryColorSelected;
        public Color CategoryColorDefault => _categoryColorDefault;
        public Transform ContainerCraftItems => _containerCraftItems;
        public Transform ContainerAddCraftCellButton => _containerAddCraftCellButton;
        public Transform ContainerBoostButtonView => _containerBoostButton;
        public ResourceCellView[] ResourceCells => _resourceCellViews;
        public QueueCellView[] QueueCells => _queueCellViews;
        public RectTransform Container => _container;
        public GameObject EmptyDescription => _emptyDescription;

        public GameObject CraftButton => _craftButtonObject;
        public GameObject BoostButton => _boostButtonObject;

        //

        public void SetIsVisibleBoostButton(bool isVisible) => _boostButtonObject.SetActive(isVisible);
        public void SetIsVisibleCancelButton(bool isVisible) => _cancelButtonObject.SetActive(isVisible);
        public void SetIsVisibleCraftButton(bool isVisible) => _craftButtonObject.SetActive(isVisible);
        public void SetIsVisibleStudyButton(bool isVisible) => _studyButtonObject.SetActive(isVisible);
        public void SetIsVisibleStudyDisableButton(bool isVisible) => _studyButtonDisableObject.SetActive(isVisible);
        public void SetIsVisibleFirstQueueCellView(bool isVisible) => _firstqueueCellView.SetActive(isVisible);
        public void SetIsVisibleCraftDisableButton(bool isVisible) => _craftDisableButtonObject.SetActive(isVisible);
        public void SetCoins(string value) => _coinsValue.text = value;
        public void SetBluePrints(string value) => _blueprintsValue.text = value;
        public void SetStudyResourceCount(string value) => _studyResourceCountText.text = value;
        public void SetStudyResourceDisableCount(string value) => _studyResourceCountDisableText.text = value;
        public void SetStudyResourceIcon(Sprite value) => _studyResourceIcon.sprite = value;
        public void SetStudyResourceDisableIcon(Sprite value) => _studyResourceDisableIcon.sprite = value;
        public void SetCategoryAllColor(Color value) => _categoryAllIcon.color = value;
        public void SetCategoryToolsColor(Color value) => _categoryToolsIcon.color = value;
        public void SetCategoryItemsColor(Color value) => _categoryItemsIcon.color = value;
        public void SetCategoryWeaponsColor(Color value) => _categoryWeaponsIcon.color = value;
        public void SetCategoryDefenceColor(Color value) => _categoryDefenceIcon.color = value;
        public void SetCategoryMedicalColor(Color value) => _categoryMedicalIcon.color = value;
        public void SetTextTitle(string text) => _title.text = text;
        public void SetTextResourcesTitle(string text) => _resourcesTitle.text = text;
        public void SetTextResourcesQueueTitle(string text) => _resourcesQueueTitle.text = text;
        public void SetTextCancelButton(string text) => _cancelButtonTitle.text = text;
        public void SetIsVisibleDefaultDescription(bool isVisible) => _emptyDescription.SetActive(isVisible);
        public void SetItemsContainerSize(int value) => _itemsContainer.GetComponent<GridLayoutGroup>().constraintCount = value;

        //

        //UI
        public event Action OnClose;
        public void ActionClose() => OnClose?.Invoke();

        //UI
        public event Action OnCraft;
        public void ActionCraft() => OnCraft?.Invoke();

        //UI
        public event Action OnCancel;
        public void ActionCancel() => OnCancel?.Invoke();

        //UI
        public event Action OnAddCoins;
        public void ActionAddCoins() => OnAddCoins?.Invoke();

        //UI
        public event Action OnStudy;
        public void ActionStudy() => OnStudy?.Invoke();

        //UI
        public event Action OnClickCategoryAll;
        public void ActionSelectCategoryAll() => OnClickCategoryAll?.Invoke();

        //UI
        public event Action OnClickCategoryTools;
        public void ActionSelectCategoryTools() => OnClickCategoryTools?.Invoke();

        //UI
        public event Action OnClickCategoryItems;
        public void ActionSelectCategoryItems() => OnClickCategoryItems?.Invoke();

        //UI
        public event Action OnClickCategoryWeapons;
        public void ActionSelectCategoryWeapons() => OnClickCategoryWeapons?.Invoke();

        //UI
        public event Action OnClickCategoryDefence;
        public void ActionSelectCategoryDefence() => OnClickCategoryDefence?.Invoke();

        //UI
        public event Action OnClickCategoryMedical;
        public void ActionSelectCategoryMedical() => OnClickCategoryMedical?.Invoke();

        public void SetTextCraftButton(string text)
        {
            _enabledCraftButtonTitle.text = text;
            _disabledCraftButtonTitle.text = text;
        }

        public void SetTextStudyButton(string text)
        {
            _enabledStudyButtonTitle.text = text;
            _disabledStudyButtonTitle.text = text;
        }
    }
}
