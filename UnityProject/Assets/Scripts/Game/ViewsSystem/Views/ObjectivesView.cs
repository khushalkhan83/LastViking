using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;
using Extensions;

namespace Game.Views
{
    public class ObjectivesView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _description1;
        [SerializeField] private Text _description2;
        [SerializeField] private Text _description3;
        [SerializeField] private Text _description4;
        [SerializeField] private Text _description5;
        [SerializeField] private Text _description6;
        [SerializeField] private Text _description7;
        [SerializeField] private Text _description8;

        [SerializeField] private Text _descriptionButton1;
        [SerializeField] private Text _descriptionButton2;
        [SerializeField] private Text _descriptionButton3;
        [SerializeField] private Text _descriptionButton4;
        [SerializeField] private Text _descriptionButton5;
        [SerializeField] private Text _descriptionButton6;
        [SerializeField] private Text _descriptionButton7;
        [SerializeField] private Text _descriptionButton8;

        [SerializeField] private Text _titleText;

        [SerializeField] private Text _coinsValue;
        [SerializeField] private Text _blueprintsValue;

        [SerializeField] private Text _freeButtonText;
        [SerializeField] private Text _freeWatchButtonText;
        [SerializeField] private Text _freeGoldButtonText;
        [SerializeField] private Text _freeGoldButtonValueText;
        [SerializeField] private Text _reminingTimeTaskText;
        [SerializeField] private Text _reminingTimeTimerText;

        [SerializeField] private Text _remainingDescription0Text;
        [SerializeField] private Text _remainingDescription1Text;
        [SerializeField] private Text _globalObjectivesDescriptionText;

        [VisibleObject] [SerializeField] private GameObject _object1;
        [VisibleObject] [SerializeField] private GameObject _object2;
        [VisibleObject] [SerializeField] private GameObject _object3;
        [VisibleObject] [SerializeField] private GameObject _object4;
        [VisibleObject] [SerializeField] private GameObject _object5;
        [VisibleObject] [SerializeField] private GameObject _object6;
        [VisibleObject] [SerializeField] private GameObject _object7;
        [VisibleObject] [SerializeField] private GameObject _object8;

        [SerializeField] private Canvas _object1Canvas;
        [SerializeField] private Canvas _object2Canvas;
        [SerializeField] private Canvas _object3Canvas;
        [SerializeField] private Canvas _object4Canvas;
        [SerializeField] private Canvas _object5Canvas;
        [SerializeField] private Canvas _object6Canvas;

        [SerializeField] private Scrollbar _tasksScrollbar;

        [SerializeField] private Transform _tasksContainer;

        [SerializeField] [VisibleObject] private GameObject _remainingTimePanel;
        [SerializeField] [VisibleObject] private GameObject _tasksPanel;
        [SerializeField] [VisibleObject] private GameObject _timerPanel;
        [SerializeField] [VisibleObject] private GameObject _noInternetTimePanel;
        [SerializeField] [VisibleObject] private GameObject _noInternetContentPanel;

        [SerializeField] [VisibleObject] GameObject _layer1;
        [SerializeField] [VisibleObject] GameObject _layer2;

        [SerializeField] RectTransform _headerLayer0Object;
        [SerializeField] RectTransform _globalObjectivesLayer0Object;
        [SerializeField] RectTransform _space0Layer0Object;
        [SerializeField] RectTransform _content0Layer0Object;

        [SerializeField] RectTransform _headerLayer2Object;
        [SerializeField] RectTransform _globalObjectivesLayer2Object;
        [SerializeField] RectTransform _objectivesLayer2Object;

        [SerializeField] Transform _titleObject;
        [SerializeField] Transform _globalObjectivesHeaderObject;
        [SerializeField] Transform _globalObjectivesObject;
        [SerializeField] Transform _darkObject;

        [SerializeField] Transform _globalObjectiveHolderLayer0Container;
        [SerializeField] Transform _globalObjectiveHolderLayer1Container;
        [SerializeField] Transform _globalObjectiveHolderLayer2Container;

        [SerializeField] Transform _titleHolderLayer0Container;
        [SerializeField] Transform _titleHolderLayer1Container;
        [SerializeField] Transform _titleHolderLayer2Container;

        [SerializeField] Transform _darkHolderLayer1Container;
        [SerializeField] Transform _darkHolderLayer2Container;

        [SerializeField] Transform _objectivesHolderLayer1Container;
        [SerializeField] Transform _objectivesHolderLayer2Container;

        [SerializeField] RectTransform _objectivesLayer2Background;

        [SerializeField] [VisibleObject] GameObject _getFreebutton;
        [SerializeField] [VisibleObject] GameObject _getWatchbutton;
        [SerializeField] [VisibleObject] GameObject _getGoldbutton;

        [SerializeField] private Image _mainIcon;
        [SerializeField] private Text _shipLevelText;
        [SerializeField] private Text _descriptionQuestText;
        [SerializeField] private ResourceCellView[] _resourceCells;
        [SerializeField] private ResourceCellView _questItemCells;

#pragma warning restore 0649
        #endregion

        public Transform TasksContainer => _tasksContainer;

        public RectTransform HeaderLayer0Object => _headerLayer0Object;
        public RectTransform GlobalObjectivesLayer0Object => _globalObjectivesLayer0Object;
        public RectTransform Space0Layer0Object => _space0Layer0Object;
        public RectTransform Content0Layer0Object => _content0Layer0Object;

        public RectTransform HeaderLayer2Object => _headerLayer2Object;
        public RectTransform GlobalObjectivesLayer2Object => _globalObjectivesLayer2Object;
        public RectTransform ObjectivesLayer2Object => _objectivesLayer2Object;
        public RectTransform ObjectivesLayer2Background => _objectivesLayer2Background;
        public ResourceCellView[] ResourceCells => _resourceCells;
        public ResourceCellView QuestItemCells => _questItemCells;

        //UI
        public event Action OnClose;
        public void ActionClose() => OnClose?.Invoke();

        //UI
        public event Action OnPickUpFirst;
        public void ActionPickUpFirst() => OnPickUpFirst?.Invoke();

        //UI
        public event Action OnPickUpSecond;
        public void ActionPickUpSecond() => OnPickUpSecond?.Invoke();

        //UI
        public event Action OnPickUpThird;
        public void ActionPickUpThird() => OnPickUpThird?.Invoke();

        //UI
        public event Action OnPickUpFourth;
        public void ActionPickUpFourth() => OnPickUpFourth?.Invoke();

        //UI
        public event Action OnPickUpFifth;
        public void ActionPickUpFifth() => OnPickUpFifth?.Invoke();

        //UI
        public event Action OnPickUpSixth;
        public void ActionPickUpSixth() => OnPickUpSixth?.Invoke();
        
        //UI
        public event Action OnPickUpSeven;
        public void ActionPickUpSeven() => OnPickUpSeven?.Invoke();

        //UI
        public event Action OnPickUpEight;
        public void ActionPickUpEight() => OnPickUpEight?.Invoke();

        //UI
        public event Action OnGetFree;
        public void ActionGetFree() => OnGetFree?.Invoke();

        //UI
        public event Action OnGetWatch;
        public void ActionGetWatch() => OnGetWatch?.Invoke();

        //UI
        public event Action OnGetGold;
        public void ActionGetGold() => OnGetGold?.Invoke();

        //UI
        public event Action OnClickGlobalObjectives;
        public void ActionClickGlobalObjectives() => OnClickGlobalObjectives?.Invoke();

        public void ScrollTasksTo(float scroll) => _tasksScrollbar.value = scroll;

        public void SetTextFirstTask(string text) => _description1.text = text;
        public void SetTextSecondTask(string text) => _description2.text = text;
        public void SetTextThirdTask(string text) => _description3.text = text;
        public void SetTextFourthTask(string text) => _description4.text = text;
        public void SetTextFifthTask(string text) => _description5.text = text;
        public void SetTextSixthTask(string text) => _description6.text = text;
        public void SetTextSevenTask(string text) => _description7.text = text;
        public void SetTextEightTask(string text) => _description8.text = text;
        public void SetTextCoins(string text) => _coinsValue.text = text;
        public void SetTextBluePrints(string value) => _blueprintsValue.text = value;
        public void SetTextFreeButton(string text) => _freeButtonText.text = text;
        public void SetTextFreeWatchButton(string text) => _freeWatchButtonText.text = text;
        public void SetTextFreeGoldButton(string text) => _freeGoldButtonText.text = text;
        public void SetTextFreeGoldValueButton(string text) => _freeGoldButtonValueText.text = text;
        public void SetTextReminingTimeTask(string text) => _reminingTimeTaskText.text = text;
        public void SetTextReminingTimeTimer(string text) => _reminingTimeTimerText.text = text;
        public void SetTextRemainingDescription0(string text) => _remainingDescription0Text.text = text;
        public void SetTextRemainingDescription1(string text) => _remainingDescription1Text.text = text;
        public void SetTextGlobalObjectivesDescription(string text) => _globalObjectivesDescriptionText.text = text;

        public void SetIsVisibleObject1(bool isVisible) => _object1.SetActive(isVisible);
        public void SetIsVisibleObject2(bool isVisible) => _object2.SetActive(isVisible);
        public void SetIsVisibleObject3(bool isVisible) => _object3.SetActive(isVisible);
        public void SetIsVisibleObject4(bool isVisible) => _object4.SetActive(isVisible);
        public void SetIsVisibleObject5(bool isVisible) => _object5.SetActive(isVisible);
        public void SetIsVisibleObject6(bool isVisible) => _object6.SetActive(isVisible);
        public void SetIsVisibleObject7(bool isVisible) => _object7.SetActive(isVisible);
        public void SetIsVisibleObject8(bool isVisible) => _object8.SetActive(isVisible);

        public void SetOverrideSortingObject1(bool value)
        {
            _object1Canvas.SetOverrideSorting(value);
        }
        public void SetOverrideSortingObject2(bool value)
        {
            _object2Canvas.SetOverrideSorting(value);
        }
        public void SetOverrideSortingObject3(bool value)
        {
            _object3Canvas.SetOverrideSorting(value);
        }
        public void SetOverrideSortingObject4(bool value)
        {
            _object4Canvas.SetOverrideSorting(value);
        }
        public void SetOverrideSortingObject5(bool value)
        {
            _object5Canvas.SetOverrideSorting(value);
        }
        public void SetOverrideSortingObject6(bool value)
        {
            _object6Canvas.SetOverrideSorting(value);
        }

        private void SetOverrideSortingObject(Canvas canvas, bool value)
        {
            canvas.overrideSorting = value;
            canvas.sortingOrder = value ? 100 : 0;
        }

        public void SetIsVisibleTaskPanel(bool isVisible) => _tasksPanel.SetActive(isVisible);
        public void SetIsVisibleTimerPanel(bool isVisible) => _timerPanel.SetActive(isVisible);
        public void SetIsVisibleRemainingTimePanel(bool isVisible) => _remainingTimePanel.SetActive(isVisible);
        public void SetIsVisibleNoInternetTimePanel(bool isVisible) => _noInternetTimePanel.SetActive(isVisible);
        public void SetIsVisibleNoInternetContentPanel(bool isVisible) => _noInternetContentPanel.SetActive(isVisible);
        public void SetIsVisibleLayer1(bool isVisible) => _layer1.SetActive(isVisible);
        public void SetIsVisibleLayer2(bool isVisible) => _layer2.SetActive(isVisible);
        public void SetIsVisibleGetFreeButton(bool isVisible) => _getFreebutton.SetActive(isVisible);
        public void SetIsVisibleGetWatchButton(bool isVisible) => _getWatchbutton.SetActive(isVisible);
        public void SetIsVisibleGetGoldButton(bool isVisible) => _getGoldbutton.SetActive(isVisible);
        public void SetShipLevelText(string text) => _shipLevelText.text = text;
        public void SetTextDescriptionQuest(string text) => _descriptionQuestText.text = text;
       

        public void MoveTitleToLayer0() => _titleObject.SetParent(_titleHolderLayer0Container);
        public void MoveTitleToLayer1() => _titleObject.SetParent(_titleHolderLayer1Container);
        public void MoveTitleToLayer2() => _titleObject.SetParent(_titleHolderLayer2Container);

        public void MoveGlobalObjectivesHeaderToLayer0() {
            _globalObjectivesHeaderObject.SetParent(_globalObjectiveHolderLayer0Container);
            RectTransform rectTransfrom = _globalObjectivesHeaderObject.GetComponent<RectTransform>();
            float y = 0;
            rectTransfrom.anchoredPosition = new Vector2(rectTransfrom.anchoredPosition.x, y);
        }
        public void MoveGlobalObjectivesHeaderToLayer1() {
            _globalObjectivesHeaderObject.SetParent(_globalObjectiveHolderLayer1Container);
            RectTransform rectTransfrom = _globalObjectivesHeaderObject.GetComponent<RectTransform>();
            float y = 0;
            rectTransfrom.anchoredPosition = new Vector2(rectTransfrom.anchoredPosition.x, y);
        }
        public void MoveGlobalObjectivesHeaderToLayer2() {
            _globalObjectivesHeaderObject.SetParent(_globalObjectiveHolderLayer2Container);
            RectTransform rectTransfrom = _globalObjectivesHeaderObject.GetComponent<RectTransform>();
            float y = 0;
            rectTransfrom.anchoredPosition = new Vector2(rectTransfrom.anchoredPosition.x, y);
        }

        public void MoveGlobalObjectivesToLayer1() => _globalObjectivesObject.SetParent(_objectivesHolderLayer1Container);
        public void MoveGlobalObjectivesToLayer2() => _globalObjectivesObject.SetParent(_objectivesHolderLayer2Container);

        public void MoveDarkToLayer1() => _darkObject.SetParent(_darkHolderLayer1Container);
        public void MoveDarkToLayer2() => _darkObject.SetParent(_darkHolderLayer2Container);

        public void SetPickUpText(string text)
        {
            _descriptionButton1.text = text;
            _descriptionButton2.text = text;
            _descriptionButton3.text = text;
            _descriptionButton4.text = text;
            _descriptionButton5.text = text;
            _descriptionButton6.text = text;
            _descriptionButton7.text = text;
            _descriptionButton8.text = text;
        }

        public void SetTitleText(string text) => _titleText.text = text;
    }
}
