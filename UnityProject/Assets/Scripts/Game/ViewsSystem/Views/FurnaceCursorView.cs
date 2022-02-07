using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class FurnaceCursorView : CursorViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _fireIcon;

        [SerializeField] private Sprite _fireOn;
        [SerializeField] private Sprite _fireOff;

#pragma warning restore 0649
        #endregion

        public Sprite FireOn => _fireOn;
        public Sprite FireOff => _fireOff;

        public void SetFireIcon(Sprite value) => _fireIcon.sprite = value;

        //TODO: replace with interact and interact alternative in Prefab
        //UI
        public void ClickOpen() => Interact();

        //UI
        public void ClickFire() => InteractAlternative();
    }
}
