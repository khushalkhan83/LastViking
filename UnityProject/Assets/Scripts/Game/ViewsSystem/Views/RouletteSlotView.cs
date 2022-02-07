using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UltimateSurvival;
using System.Collections;
using System;

namespace Game.Views
{
    public class RouletteSlotView : MonoBehaviour
    {
        public event Action<RouletteSlotView> Click;
        #region Data
#pragma warning disable 0649
        [SerializeField] RouletteCellView _cell;
        [SerializeField] Button _spinButton;
        [SerializeField] Transform _cellsContainer;
        [SerializeField] Image _slotBackground;
        [SerializeField] GameObject _plusImage;
        [SerializeField] float _cellHeight;
        [SerializeField] float _spacing;
        [SerializeField] float _maxSpeed;
        [SerializeField] float _minSpeed;
        [SerializeField] float _startAcceleration;
        [SerializeField] float _stopAcceleration;
        [SerializeField] Color _normalColor;
        [SerializeField] Color _speciallColor;
        [SerializeField] Text _respinGoldPrice;
#pragma warning restore 0649
        #endregion

        private float _speed;
        private float _topBound;
        private float _bottomBound;
        private List<RectTransform> _cells;
        private RectTransform _targetSlot;
        private bool _needToStop = false;
        private bool _isSpecial;

        public RollState State { get; private set; }

        public bool IsSpecial {
            get { return _isSpecial; }
            set {
                _isSpecial = value;
                SetIsSpinButtonVisible(!_isSpecial);
                _plusImage.SetActive(_isSpecial);
                if (_isSpecial)
                    _slotBackground.color = _speciallColor;
                else
                    _slotBackground.color = _normalColor;
            }
        }
        public void SetIsVisible(bool isVisible) => gameObject.SetActive(isVisible);

        public void SetIsSpinButtonInteractable(bool interactable) => _spinButton.interactable = interactable;

        public void SetIsSpinButtonVisible(bool isVisible) => _spinButton.gameObject.SetActive(isVisible);

        public void SetItems(List<SavableItem> items, SavableItem targetItem, SavableItem firstItem = null)
        {
            SetIsSpinButtonInteractable(true);
            _cell.SetIsVisible(false);
            float cellFullHeight = _cellHeight + _spacing;
            _bottomBound = -cellFullHeight * 1.5f;
            _topBound = _bottomBound + cellFullHeight * items.Count;
            _needToStop = false;
            State = RollState.NOT_MOVING;

            if (firstItem != null) {
                items.Remove(firstItem);
                items.Insert(items.Count - 1, firstItem);
            }

            ClearCells();
            if (_cells == null)
            {
                _cells = new List<RectTransform>();
            }

            float placePosition = _topBound - cellFullHeight/2f;
            foreach (var item in items) {
                RectTransform cellTransform = Instantiate(_cell.gameObject, _cellsContainer).GetComponent<RectTransform>();
                cellTransform.gameObject.SetActive(true);
                cellTransform.anchoredPosition = new Vector2(0, placePosition);
                _cells.Add(cellTransform);
                placePosition -= _cellHeight + _spacing;
                SetupCell(cellTransform.GetComponent<RouletteCellView>(), item);

                if (item == targetItem) {
                    _targetSlot = cellTransform;
                }
            }
        }

        public void SetItem(SavableItem item) {
            SetIsSpinButtonInteractable(true);
            State = RollState.NOT_MOVING;
            ClearCells();
            _cell.SetIsVisible(true);
            SetupCell(_cell, item);
        }

        public void SetRespinGoldPrice(int price) 
        {
            _respinGoldPrice.text = price.ToString();
        }

        public void OnClick() {
            if (Click != null) {
                Click(this);
            }
        }

        private void ClearCells()
        {
            if (_cells != null && _cells.Count > 0)
            {
                foreach (var cell in _cells)
                {
                    Destroy(cell.gameObject);
                }
                _cells.Clear();
            }
        }

        private void SetupCell(RouletteCellView cell, SavableItem item) {
            cell.SetData(GetData(item));
        }

        private RouletteCellData GetData(SavableItem item)
        {
            return new RouletteCellData
            {
                Icon = item.ItemData.Icon,
                Count = item.Count,
                ItemRarity = item.ItemData.ItemRarity,
                IsComponent = item.ItemData.Category == "Components",
                IsSpecial = IsSpecial,
            };
        }

        public void StartSpin(float stopDelay = 0) {
            if (_cells.Count < 3 || _targetSlot == null || State != RollState.NOT_MOVING)
            {
                return;
            }
            else {
                _speed = 0;
                State = RollState.RUN_UP;
                SetIsSpinButtonInteractable(false);
                if (stopDelay != 0) {
                    StartCoroutine(StopCoroutine(stopDelay));
                }
            }
        }

        private IEnumerator StopCoroutine(float delay) {
            yield return new WaitForSeconds(delay);
            StopSpin();
        }

        public void StopSpin() {
            if (State == RollState.ROLLING || State == RollState.RUN_UP) {
                _needToStop = true;
            }
        }

        private void Update()
        {
            switch (State)
            {
                case RollState.NOT_MOVING:
                    break;
                case RollState.RUN_UP:
                    IncreaseSpeed();
                    Roll();
                    break;
                case RollState.ROLLING:
                    Roll();
                    break;
                case RollState.SLOW_DOWN:
                    SlowDown();
                    Roll();
                    break;
            }
        }

        private void Roll()
        {
            float distance = _speed * Time.deltaTime;
            foreach (var cell in _cells) {
                float newPosition = cell.anchoredPosition.y - distance;
                if (newPosition < _bottomBound) {
                    float delta = newPosition - _bottomBound;
                    newPosition = _topBound + delta;
                    if (cell == _targetSlot) {
                        CheckStop();
                    }
                }
                cell.anchoredPosition = new Vector2(0, newPosition);
            }
        }

        private void IncreaseSpeed()
        {
            _speed += _startAcceleration * Time.deltaTime;
            if (_speed >= _maxSpeed) {
                _speed = _maxSpeed;
                State = RollState.ROLLING;
            }
        }

        private void CheckStop() {
            if (_needToStop) 
            {
                _needToStop = false;
                State = RollState.SLOW_DOWN;
            }
        }

        private void SlowDown() 
        {
            _speed -= _stopAcceleration * Time.deltaTime;
            if (_speed <= _minSpeed) {
                _speed = _minSpeed;
            }

            if (_targetSlot.anchoredPosition.y <= 0)
            {
                SetIsSpinButtonInteractable(true);
                float distance = -_targetSlot.anchoredPosition.y;
                foreach (var cell in _cells)
                {
                    float newPosition = cell.anchoredPosition.y + distance;
                    cell.anchoredPosition = new Vector2(0, newPosition);
                }
                _speed = 0;
                State = RollState.NOT_MOVING;
            }
        }

        public enum RollState { 
            NOT_MOVING,
            RUN_UP,
            ROLLING,
            SLOW_DOWN,
        }
        
    }

    public struct RouletteCellData
    {
        public Sprite Icon;
        public int? Count;
        public ItemRarity ItemRarity;
        public bool IsComponent;
        public bool IsSpecial;
    }
}
