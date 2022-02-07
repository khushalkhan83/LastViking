using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class LoomCursorView : CursorViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _weaveIcon;

        [SerializeField] private Sprite _weaveOn;
        [SerializeField] private Sprite _weaveOff;

#pragma warning restore 0649
        #endregion

        public Sprite WeaveOn => _weaveOn;
        public Sprite WeaveOff => _weaveOff;

        public void SetWeaveIcon(Sprite value) => _weaveIcon.sprite = value;

        //TODO: replace with interact and interact alternative in Prefab
        //UI
        public void ClickOpen() => Interact();

        //UI
        public void ClickWeave() => InteractAlternative();
    }
}
