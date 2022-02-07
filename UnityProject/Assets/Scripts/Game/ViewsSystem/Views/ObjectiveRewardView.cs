using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class ObjectiveRewardView : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _countItemsText;

        [SerializeField] private Image _itemIcon;
        [SerializeField] private Image _backgroundImage;

#pragma warning restore 0649
        #endregion

        public void SetTextCountItems(string text) => _countItemsText.text = text;
        public void SetImageItem(Sprite sprite) => _itemIcon.sprite = sprite;
        public void SetIsVisible(bool isVisible) => gameObject.SetActive(isVisible);
        public void SetBackgroundColor(Color color) => _backgroundImage.color = color;
    }
}
