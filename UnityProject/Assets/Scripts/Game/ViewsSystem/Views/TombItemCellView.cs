using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class TombItemCellView : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [VisibleObject]
        [SerializeField] private GameObject _countItemsObject;

        [SerializeField] private Text _countItemsText;

        [SerializeField] private Image _itemIcon;

#pragma warning restore 0649
        #endregion

        public void SetTextCountItems(string text) => _countItemsText.text = text;
        public void SetImageItem(Sprite sprite) => _itemIcon.sprite = sprite;
        public void SetIsVisibleCount(bool isVisible) => _countItemsObject.SetActive(isVisible);
        public void SetIsVisible(bool isVisible) => gameObject.SetActive(isVisible);
    }
}
