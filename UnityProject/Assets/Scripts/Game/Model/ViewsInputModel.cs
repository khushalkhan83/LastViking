using System;
using UnityEngine;

namespace Game.Models
{
    public class ViewsInputModel : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private CanvasGroup _defaultCanvas;
        [SerializeField] private CanvasGroup _popupCanvas;
        
        #pragma warning restore 0649
        #endregion

        public bool InputBlocked {get; private set;}

        public void BlockInput(bool block)
        {
            InputBlocked = block;
            BlockTouchInput(_defaultCanvas, block);
            BlockTouchInput(_popupCanvas, block);
        }

        private void BlockTouchInput(CanvasGroup canvas, bool block)
        {
            canvas.blocksRaycasts = !block;
        }
    }
}
