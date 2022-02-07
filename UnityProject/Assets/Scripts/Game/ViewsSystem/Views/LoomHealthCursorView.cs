using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class LoomHealthCursorView : CursorViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _weaveIcon;

        [SerializeField] private Sprite _weaveOn;
        [SerializeField] private Sprite _weaveOff;

        [SerializeField] private Image _amountImage;
        [SerializeField] private Text _text;

#pragma warning restore 0649
        #endregion

        public Sprite WeaveOn => _weaveOn;
        public Sprite WeaveOff => _weaveOff;

        public void SetWeaveIcon(Sprite value) => _weaveIcon.sprite = value;

        public void SetFillAmount(float value) => _amountImage.fillAmount = value;

        public void SetText(string value) => _text.text = value;

        
        //TODO: replace with interact and interact alternative in Prefab
        //UI
        public void ClickOpen() => Interact();

        //UI
        public void ClickWeave() => InteractAlternative();
    }
}
