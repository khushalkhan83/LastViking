using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class ItemsRelatedInteractableCursorView : CursorViewExtended
    {
        [SerializeField] private RequiredItemView requiredItemView = default;
        [SerializeField] private Transform _itemsContainer;
        [SerializeField] private Color enoughColor = default;
        [SerializeField] private Color notEnoughColor = default;

        public RequiredItemView RequiredItemView => requiredItemView;
        public Transform ItemsContainer => _itemsContainer;

        public  Color EnoughColor => enoughColor;
        public  Color NotEnoughColor => notEnoughColor;
    }
}
