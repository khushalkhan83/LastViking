using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UltimateSurvival.GUISystem;
using System.Linq;

namespace UltimateSurvival.Debugging
{
    public class ItemAddHandler : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Dropdown m_ItemDropdown;

        [SerializeField] private InputField m_AmountInput;

        [SerializeField] private Button m_AddButton;

#pragma warning restore 0649
        #endregion

        //private void Start()
        //{
        //    m_AddButton.onClick.AddListener(TryAddItem);
        //    CreateItemOptions();
        //}

        //private void CreateItemOptions()
        //{
        //    m_ItemDropdown.options = InventoryController.Instance.Database.GetAllItemNames().Select(x => new Dropdown.OptionData(x)).ToList();
        //    m_ItemDropdown.RefreshShownValue();
        //}

        //private void TryAddItem()
        //{
        //    int amount;
        //    int added;
        //    if (int.TryParse(m_AmountInput.text, out amount))
        //        InventoryController.Instance.AddItemToCollection(m_ItemDropdown.value, amount, "Inventory", out added);
        //}
    }
}
