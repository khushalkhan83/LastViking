using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class MineableCursorView : CursorViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _toolIcon;
        [SerializeField] private Image _secondToolMiniIcon;
        [SerializeField] private GameObject _secondMiniIconHolder;
        [SerializeField] private GameObject _toolIconHolder;

        [SerializeField] private Sprite _toolWood;
        [SerializeField] private Sprite _toolStone;
        [SerializeField] private Sprite _toolShovel;
        [SerializeField] private Sprite _pickMiniIconSprite;
        [SerializeField] private Sprite _axeMiniIconSprite;
        [SerializeField] private Sprite _shovelMiniIconSprite;

        [SerializeField] private GameObject _hookMiniIcon;

#pragma warning restore 0649
        #endregion

        public void SetWoodToolIcon() => _toolIcon.sprite = _toolWood;
        public void SetStoneToolIcon() => _toolIcon.sprite = _toolStone;
        public void SetShovelToolIcon() => _toolIcon.sprite = _toolStone;
        public void SetActiveToolIconHolder(bool active) => _toolIconHolder.SetActive(active);
        
        
        public void SetActiveSecondMiniIcon(bool active) => _secondMiniIconHolder.SetActive(active);
        public void SetSecondMiniIconAsAxe() => _secondToolMiniIcon.sprite = _axeMiniIconSprite;
        public void SetSecondMiniIconAsPick() => _secondToolMiniIcon.sprite = _pickMiniIconSprite;
        public void SetSecondMiniIconAsShovel() => _secondToolMiniIcon.sprite = _shovelMiniIconSprite;

        public void SetActiveHookMiniIcon(bool active) => _hookMiniIcon.SetActive(active);
    }
}