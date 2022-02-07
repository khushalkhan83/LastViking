using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Game.Views
{
    public class InventoryPlayerDataView : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _heathValue;
        [SerializeField] private Text _healthValueMax;
        [SerializeField] private Text _eatValue;
        [SerializeField] private Text _eatValueMax;
        [SerializeField] private Text _waterValue;
        [SerializeField] private Text _waterValueMax;

        [SerializeField] private Image _playerImage;
        [SerializeField] private Text _playerName;

        [SerializeField] private Color _loginedPlayerColor;
        [SerializeField] private Color _unloginedPlayerColor;

        //Item Description
        [SerializeField] private RectTransform _container;
        [SerializeField] private GameObject _emptyDescription;
        //Item Description

#pragma warning restore 0649
        #endregion

        //Item Description
        public RectTransform Container => _container;
        //Item Description


        public Color LoginedPlayerColor => _loginedPlayerColor;
        public Color UnloginedPlayerColor => _unloginedPlayerColor;
        public GameObject EmptyDescription => _emptyDescription;

        public void SetHeathValue(string value) => _heathValue.text = value;
        public void SetHeathValueMax(string value) => _healthValueMax.text = value;
        public void SetHealthSize(int size) => _heathValue.fontSize = size;

        public void SetEatValue(string value) => _eatValue.text = value;
        public void SetEatValueMax(string value) => _eatValueMax.text = value;
        public void SetEatSize(int size) => _eatValue.fontSize = size;

        public void SetWaterValue(string value) => _waterValue.text = value;
        public void SetWaterValueMax(string value) => _waterValueMax.text = value;
        public void SetWaterSize(int size) => _waterValue.fontSize = size;

        public void SetImagePlayer(Sprite sprite) => _playerImage.sprite = sprite;
        public void SetNamePlayer(string name) => _playerName.text = name;
        public void SetColorPlayerName(Color color) => _playerName.color = color;
        public void SetEmptyDescriptionVisible(bool isVisible) => _emptyDescription.SetActive(isVisible);

        //UI
        public event Action OnEditName;
        public void ActionEditName() => OnEditName?.Invoke();

        public void SetHealth(int current, int max)
        {
            SetHeathValue($"{current}");
            SetHeathValueMax($"/{max}");
        }

        public void SetEat(int current, int max)
        {
            SetEatValue($"{current}");
            SetEatValueMax($"/{max}");
        }

        public void SetWater(int current, int max)
        {
            SetWaterValue($"{current}");
            SetWaterValueMax($"/{max}");
        }
    }
}
