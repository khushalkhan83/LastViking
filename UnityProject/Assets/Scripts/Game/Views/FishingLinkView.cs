using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class FishingLinkView : ViewBase
    {
        [SerializeField] private GameObject coinsObject = default;

        public void ShowCoins(bool showCoins) => coinsObject.SetActive(showCoins);

        public event Action OnClick;
        public void Click() => OnClick?.Invoke();

    }
}
