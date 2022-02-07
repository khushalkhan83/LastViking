using System;

namespace Game.Models
{
    public class InventoryLootViewModel : InventoryCellsViewModelBase
    {
        public bool IsShow { get; private set; }
        public event Action OnShowChanged;

        public void SetShow(bool value)
        {
            IsShow = value;
            OnShowChanged?.Invoke();
        }

        public bool IsTakeAllButtonHilight { get; private set; }
        public event Action OnTakeAllButtonClickedChanged;

        public void SetTakeAllButtonHilight(bool value)
        {
            IsTakeAllButtonHilight = value;
            OnTakeAllButtonClickedChanged?.Invoke();
        }

        public event Action OnTakeAllButtonClicked;

        public void TakeAllButtonClicked()
        {
            OnTakeAllButtonClicked?.Invoke();
        }


    }
}
