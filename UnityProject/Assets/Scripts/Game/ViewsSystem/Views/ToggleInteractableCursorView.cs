using Core.Views;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views
{
    public class ToggleInteractableCursorView : CursorViewExtended
    {
        [SerializeField] private Image _fireIcon;

        [SerializeField] private Sprite _fireOn;
        [SerializeField] private Sprite _fireOff;

        public void SetFireCursor(bool on)
        {
            var targetSprite = on ? _fireOn: _fireOff;
            _fireIcon.sprite = targetSprite;
        }
    }
}
