using Game.Views;
using Core.Controllers;
using Game.Models;
using UltimateSurvival;
using Core;

namespace Game.Controllers
{
    public class InfoPanelEquipmentViewController : ViewControllerBase<InfoPanelEquipmentView, InfoPanelEquipmentViewData>
    {
        [Inject] public AbilityProvider AbilityProvider { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public EquipmentModel EquipmentModel { get; private set; }

        protected override void Show() 
        {
            SetupView();
        }

        protected override void Hide() 
        {

        }

        private void SetupView()
        {
            var item = Data.Item;

            View.ItemName.text = LocalizationModel.GetString(item.DisplayNameKeyID);

            SetupItemBonus(item);
            SetupSetBonus(item);
        }

        private void SetupItemBonus(ItemData item)
        {
            item.TryGetProperty("FirstAbilityID", out var firstAbilityID);
            if(firstAbilityID == null)
            {
                View.BonusGO.SetActive(false);
                View.DescriptionGO.SetActive(false);
                View.Decor.SetActive(true);
                return;
            }
            else
            {
                View.BonusGO.SetActive(true);
                View.DescriptionGO.SetActive(true);
                View.Decor.SetActive(false);
            }

            var firstAbility = AbilityProvider[firstAbilityID.AbilityID];

            View.BonusIcon.sprite = firstAbility.Sprite;
            View.BonusValueText.text = GetValueByItemAbilityID(item, firstAbilityID.AbilityID);
            View.DescriptioText.text = LocalizationModel.GetString(firstAbility.LocalizationKeyID);
        }

        private void SetupSetBonus(ItemData item)
        {
            if(item.TryGetProperty("EquipmentSet", out var setProp) && setProp.EquipmentSet != EquipmentSet.None)
            {
                View.SetBonusGO.SetActive(true);
                View.SetDescriptionGO.SetActive(true);

                int equipedSetItemsCount = 0;
                if(Data.UpdateEquipedSetCount) EquipmentModel.EquipmentSetItemCounts.TryGetValue(setProp.EquipmentSet, out equipedSetItemsCount);
                var setInfo = EquipmentModel.EquipmentSetInfoProvider[setProp.EquipmentSet];

                bool setActive = equipedSetItemsCount >= setInfo.FullSetCount;
                View.SetEquipmentSetBonusActive(setActive);

                var setAbility = AbilityProvider[setInfo.AbilityID];
                View.SetBonusIcon.sprite = setAbility.Sprite;
                View.SetBonusValueText.text = GetValueBySetAbilityID(setInfo); 
                View.SetDescriptionText.text = $"{LocalizationModel.GetString(setAbility.LocalizationKeyID)} ({equipedSetItemsCount}/{setInfo.FullSetCount})";
            }
            else
            {
                View.SetBonusGO.SetActive(false);
                View.SetDescriptionGO.SetActive(false);
            }
        }

        private string GetValueByItemAbilityID(ItemData item, AbilityID abilityID)
        {
            switch(abilityID)
            {
                case AbilityID.IncreaseHealth:
                    return item.TryGetProperty("HealthBonus", out var healthProp) ? "+" + healthProp.Int.Current.ToString() : string.Empty;
                case AbilityID.IncreaseFood:
                    return item.TryGetProperty("FoodBonus", out var foodProp) ? "+" + foodProp.Int.Current.ToString() : string.Empty;
                case AbilityID.IncreaseWater:
                    return item.TryGetProperty("WaterBonus", out var waterProp) ? "+" + waterProp.Int.Current.ToString() : string.Empty;
                case AbilityID.IncreaseStamina:
                    return item.TryGetProperty("StaminaPercentageBonus", out var staminaProp) ? "+" + staminaProp.Int.Current.ToString() + "%" : string.Empty;
                case AbilityID.IncreaseSpeed:
                    return item.TryGetProperty("SpeedPercentageBonus", out var speedProp) ? "+" + speedProp.Int.Current.ToString() + "%" : string.Empty;
                default:
                    return string.Empty;
            }
        }

        private string GetValueBySetAbilityID(EquipmentSetInfo setInfo)
        {
            switch(setInfo.AbilityID)
            {
                case AbilityID.IncreaseHealth:
                    return "+" + setInfo.BonusProperty.Int.Current;
                case AbilityID.IncreaseFood:
                    return "+" + setInfo.BonusProperty.Int.Current;
                case AbilityID.IncreaseWater:
                    return "+" + setInfo.BonusProperty.Int.Current;
                case AbilityID.IncreaseStamina:
                    return "+" + setInfo.BonusProperty.Int.Current + "%";
                case AbilityID.IncreaseSpeed:
                    return "+" + setInfo.BonusProperty.Int.Current + "%";
                default:
                    return string.Empty;
            }
        }

    }

    public class InfoPanelEquipmentViewData : IDataViewController
    {
        public ItemData Item { get; private set; }
        public bool UpdateEquipedSetCount {get; private set;}

        public InfoPanelEquipmentViewData(ItemData item, bool updateEquipedSetCount)
        {
            Item = item;
            UpdateEquipedSetCount = updateEquipedSetCount;
        }
    }
}
