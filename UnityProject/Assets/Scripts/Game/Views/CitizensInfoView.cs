using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Views
{
    public class CitizensInfoView : ViewBase
    {
        [SerializeField] private TextMeshProUGUI countText = default;

        public void SetCountText(string text) => countText.text = text;
    }
}
